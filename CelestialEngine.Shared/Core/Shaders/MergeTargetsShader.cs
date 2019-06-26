// -----------------------------------------------------------------------
// <copyright file="MergeTargetsShader.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Shaders
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Internal deferred renderer shader that merges the render targets onto the back buffer
    /// </summary>
    internal sealed class MergeTargetsShader : Shader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeTargetsShader"/> class.
        /// </summary>
        public MergeTargetsShader()
            : base(Content.Shaders.Core.MergeTargets)
        {
        }
        #endregion

        #region Shader Overrides
        /// <summary>
        /// Called when the shader parameters are to be configured before rendering.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        /// <param name="technique">Technique to use (can be null)</param>
        protected override void ConfigureShaderInternal(DeferredRenderSystem renderSystem, EffectTechnique technique)
        {
            base.ConfigureShaderInternal(renderSystem, technique);

            this.ShaderAsset.Parameters["colorMap"].SetValue(renderSystem.RenderTargets.ColorMap);
            this.ShaderAsset.Parameters["lightMap"].SetValue(renderSystem.RenderTargets.LightMap);
        }
        #endregion
    }
}
