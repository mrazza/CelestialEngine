// -----------------------------------------------------------------------
// <copyright file="ConeLight.cs" company="">
// Copyright (C) 2012 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game.PostProcess.Lights
{
    using System;
    using CelestialEngine.Core;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A cone light that exists in 3D space and applies the specified lighting within the
    /// given angular constaints with specular effects.
    /// </summary>
    public class ConeLight : PointLight
    {
        #region Members
        /// <summary>
        /// The angle (breath) that the light will affect (in radians).
        /// </summary>
        private float lightAngle;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConeLight"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        public ConeLight(World world)
            : base(world, new Shader(Content.Shaders.Lights.ConeLight))
        {
            this.lightAngle = MathHelper.PiOver4;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the angle (breath) that the light will affect (in radians) on either side of its directional vector.
        /// A value of 45 degrees will result in 90 degrees of light (45 per side).
        /// </summary>
        /// <value>
        /// The angle (breath) that the light will affect (in radians).
        /// </value>
        public float LightAngle
        {
            get
            {
                return this.lightAngle;
            }

            set
            {
                this.lightAngle = MathHelper.Clamp(value, 0.0f, MathHelper.Pi);
            }
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
            float halfMaxWidth = MathHelper.Min(this.Range, this.Range * (float)Math.Tan(this.lightAngle / 2.0f));
            RectangleF result = new RectangleF(this.Position.X - halfMaxWidth, this.Position.Y - this.Range, 2.0f * halfMaxWidth, this.Range);
            result = result.RotateAbout(new Vector2(this.Position.X, this.Position.Y), this.Rotation);

            return result;
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="CelestialEngine.Core.PostProcess.SimulatedPostProcess"/> component needs render itself.
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
            this.PostProcessEffect.GetParameter("lightAngle").SetValue(this.LightAngle / 2.0f);
            this.PostProcessEffect.GetParameter("lightFacingDirection").SetValue(this.RotationVector);

            // Render
            this.PostProcessEffect.ApplyPass(0);
            renderSystem.DirectScreenPaint(this.GetWorldDrawBounds());

            renderSystem.SetRenderTargets(RenderTargetTypes.None, 0); // Resolve the render target
            renderSystem.RenderTargets.ReleaseTemporaryRenderTarget(shadowMap);
        }
        #endregion
    }
}
