// -----------------------------------------------------------------------
// <copyright file="AmbientLight.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game.PostProcess.Lights
{
    using CelestialEngine.Core;
    using CelestialEngine.Core.PostProcess;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A simple ambient light post process that applies the specified ambient light to the entire scene.
    /// </summary>
    public class AmbientLight : StaticPostProcess
    {
        #region Members
        /// <summary>
        /// The color of the ambient light
        /// </summary>
        private Color lightColor;

        /// <summary>
        /// Light intensity, value between 0 (no light) and 1 (full light).
        /// </summary>
        private float lightIntensity;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientLight"/> class.
        /// </summary>
        /// <param name="lightColor">The color of the ambient light.</param>
        /// <param name="lightIntensity">The a <see cref="float"/> representing the intensity of the light between 0 (no light) and 1 (full light).</param>
        /// <param name="isEnabled">if set to <c>true</c> the effect will be enabled.</param>
        /// <param name="renderPriority">The render priority.</param>
        public AmbientLight(Color lightColor, float lightIntensity, bool isEnabled, float renderPriority)
            : base(new Shader(Content.Shaders.Lights.AmbientLight), isEnabled, renderPriority)
        {
            this.lightColor = lightColor;
            this.lightIntensity = lightIntensity;
        }
        #endregion

        #region StaticPostProcess Overrides
        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="StaticPostProcess"/> component needs render itself.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Core.DeferredRenderSystem renderSystem)
        {
            renderSystem.SetRenderTargets(RenderTargetTypes.LightMap, 1); // Request light map
            this.PostProcessEffect.ConfigureShader(renderSystem);

            this.PostProcessEffect.GetParameter("lightColor").SetValue(this.lightColor.ToVector3());
            this.PostProcessEffect.GetParameter("lightIntensity").SetValue(this.lightIntensity);
            this.PostProcessEffect.GetParameter("optionsMap").SetValue(renderSystem.RenderTargets.OptionsMap);

            this.PostProcessEffect.ApplyPass(0);
            renderSystem.DirectScreenPaint();
        }
        #endregion
    }
}
