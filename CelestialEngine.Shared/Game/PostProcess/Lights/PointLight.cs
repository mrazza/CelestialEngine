// -----------------------------------------------------------------------
// <copyright file="PointLight.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game.PostProcess.Lights
{
    using CelestialEngine.Core;
    using CelestialEngine.Core.PostProcess;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// A point light that exists in 3D space and applies the specified lighting with specular effects.
    /// </summary>
    public class PointLight : SimulatedLight
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        public PointLight(World world)
            : this(world, new Shader(Content.Shaders.Lights.PointLight))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="shader">The shader to use with the light.</param>
        protected PointLight(World world, Shader shader)
            : base(world, shader)
        {
            this.Decay = 1.0f;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the power of the light.
        /// </summary>
        /// <value>
        /// The power of the light.
        /// </value>
        public float Power
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the range, in world units, of the light.
        /// </summary>
        /// <value>The range of the light.</value>
        public float Range
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the decay of the light (spread factor).
        /// </summary>
        /// <value>
        /// The decay of the light.
        /// </value>
        public float Decay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the light.
        /// </summary>
        /// <value>
        /// The color of the light.
        /// </value>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the strength of the specular effect.
        /// </summary>
        /// <value>
        /// The specular strength.
        /// </value>
        public float SpecularStrength
        {
            get;
            set;
        }
        #endregion

        #region SimulatedPostProcess Overrides
        /// <summary>
        /// Gets a <see cref="RectangleF"/> containing information related to the dimensions of the bounding box in which the post process
        /// affects in world units. Used by the <see cref="DeferredRenderSystem"/> to determine whether the post process is drawn.
        /// </summary>
        /// <returns>
        /// A <see cref="RectangleF"/> containing the position and size of the area that may be affected by this post process (X, Y, Width, Height).
        /// </returns>
        public override RectangleF GetWorldDrawBounds()
        {
            float rangeFactor = this.Range / (float)Math.Sqrt(this.Decay);
            return new RectangleF(this.Position.X - rangeFactor, this.Position.Y - rangeFactor, 2.0f * this.Range, 2.0f * rangeFactor);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SimulatedPostProcess"/> component needs render itself.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        protected override void Render(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            RenderTarget2D shadowMap = renderSystem.RenderTargets.GetTemporaryRenderTarget(SurfaceFormat.Color); // The core shadow map
            this.GenerateShadowMap(renderSystem, shadowMap);

            renderSystem.SetRenderTargets(RenderTargetTypes.LightMap, 1); // Request light map as output
            this.PostProcessEffect.ConfigureShader(renderSystem); // Configure the shader

            // Set parameters
            this.PostProcessEffect.GetParameter("viewProjection").SetValue(renderSystem.GameCamera.GetViewProjectionMatrix(renderSystem));
            this.PostProcessEffect.GetParameter("cameraPosition").SetValue(renderSystem.GameCamera.PixelPosition);
            this.PostProcessEffect.GetParameter("lightPosition").SetValue(this.PixelPosition);
            this.PostProcessEffect.GetParameter("lightPower").SetValue(this.Power);
            this.PostProcessEffect.GetParameter("lightRange").SetValue(this.World.GetPixelFromWorld(this.Range));
            this.PostProcessEffect.GetParameter("lightDecay").SetValue(this.Decay);
            this.PostProcessEffect.GetParameter("lightColor").SetValue(this.Color.ToVector4());
            this.PostProcessEffect.GetParameter("specularStrength").SetValue(this.SpecularStrength);
            this.PostProcessEffect.GetParameter("normalMap").SetValue(renderSystem.RenderTargets.NormalMap);
            this.PostProcessEffect.GetParameter("optionsMap").SetValue(renderSystem.RenderTargets.OptionsMap);
            this.PostProcessEffect.GetParameter("shadowMap").SetValue(shadowMap);
            this.PostProcessEffect.GetParameter("layerDepth").SetValue(this.LayerDepth / 255.0f);

            // Render
            this.PostProcessEffect.ApplyPass(0);
            renderSystem.DirectScreenPaint(this.GetWorldDrawBounds());

            renderSystem.SetRenderTargets(RenderTargetTypes.None, 0); // Resolve the render target

            renderSystem.RenderTargets.ReleaseTemporaryRenderTarget(shadowMap);
        }
        #endregion
    }
}
