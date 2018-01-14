// -----------------------------------------------------------------------
// <copyright file="PointLight.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game.PostProcess.Lights
{
    using CelestialEngine.Core;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// A rectangle of light that exists as a plane 3D space and applies the specified lighting
    /// completely within the rectangle with light fading away from the bounds of the rectangle.
    /// </summary>
    /// <remarks>
    /// For a beam of light, set the height (or width) of the rectangle to 0 (or near-0) making
    /// the fully lit portion of the light source infintely thin. An example use of this light
    /// type is laser weapons.
    /// 
    /// A <see cref="RectangularLight"/> with dimensions of {0, 0} is functionally the same as a
    /// <see cref="PointLight"/>. However, there is additional computational overhead.
    /// </remarks>
    public class RectangularLight : PointLight
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangularLight"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        public RectangularLight(World world)
            : base(world, new Shader(Content.Shaders.Lights.RectangularLight))
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The dimensions of the rectangle that is the source of the light.
        /// </summary>
        public Vector2 Dimensions
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
            Vector2 halfDimensions = this.Dimensions / 2.0f;
            float rangeFactor = this.Range / (float)Math.Sqrt(this.Decay);
            return new RectangleF(this.Position.X - rangeFactor - halfDimensions.X, this.Position.Y - rangeFactor - halfDimensions.Y, 2.0f * rangeFactor + this.Dimensions.X, 2.0f * rangeFactor + this.Dimensions.Y).RotateAbout(this.Position.ToVector2(), this.Rotation);
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
            this.PostProcessEffect.GetParameter("lightRotationMatrix").SetValue(Matrix.CreateRotationZ(-this.Rotation));
            this.PostProcessEffect.GetParameter("lightCenterPosition").SetValue(this.PixelPosition);
            this.PostProcessEffect.GetParameter("lightRectangleHalfDimensions").SetValue(this.World.GetPixelFromWorld(this.Dimensions / 2.0f));
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
