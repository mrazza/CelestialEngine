// -----------------------------------------------------------------------
// <copyright file="DebugTargetsShader.cs" company="">
// Copyright (C) Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Shaders
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Internal deferred renderer shader that merges the render targets onto the back buffer
    /// </summary>
    internal sealed class DebugTargetsShader : Shader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugTargetsShader"/> class.
        /// </summary>
        public DebugTargetsShader()
            : base(Content.Shaders.Core.DebugTargets)
        {
        }
        #endregion

        #region Shader Overrides
        /// <summary>
        /// Called when the shader parameters are to be configured before rendering.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        /// <param name="technique">Technique to use (can be null)</param>
        protected override void ConfigureShader(DeferredRenderSystem renderSystem, EffectTechnique technique)
        {
            base.ConfigureShader(renderSystem, technique);

            this.ShaderAsset.Parameters["colorMap"].SetValue(renderSystem.RenderTargets.ColorMap);
            this.ShaderAsset.Parameters["optionsMap"].SetValue(renderSystem.RenderTargets.OptionsMap);
            //this.ShaderAsset.Parameters["shadowMap"].SetValue(renderSystem.RenderTargets.Shad);
            this.ShaderAsset.Parameters["lightMap"].SetValue(renderSystem.RenderTargets.LightMap);
        }
        #endregion
    }
}
