// -----------------------------------------------------------------------
// <copyright file="OptionsMapFlagsShader.cs" company="">
// Copyright (C) 2012 Will Graham & Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Shaders
{
    using System;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Internal options map shader that applies the per-pixel render options to the options render target
    /// </summary>
    public class OptionsMapFlagsShader : Shader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsMapFlagsShader"/> class.
        /// </summary>
        public OptionsMapFlagsShader()
            : base(Content.Shaders.Core.OptionsMapFlags)
        {
        }
        #endregion

        #region Options Map-specific Methods
        /// <summary>
        /// Called when the shader parameters are to be configured before rendering.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        /// <param name="sprite">The sprite we are rendering.</param>
        public void ConfigureShaderAndApplyPass(DeferredRenderSystem renderSystem, SpriteBase sprite)
        {
            base.ConfigureShader(renderSystem, "ApplyFlagsToOptionsMap");

            this.ShaderAsset.Parameters["pixelOptions"].SetValue((int)sprite.RenderOptions / 255.0f);
            this.ShaderAsset.Parameters["specularReflectivity"].SetValue(sprite.SpecularReflectivity);
            this.ShaderAsset.Parameters["layerDepth"].SetValue(sprite.LayerDepth / 255.0f);
            this.ApplyPass(0);
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
            if (!technique.Name.Equals("ApplyFlagsToOptionsMap"))
            {
                throw new ArgumentException("You dun goofed.");
            }

            this.ShaderAsset.Parameters["viewProjection"].SetValue(renderSystem.GameCamera.GetViewProjectionMatrix(renderSystem));
            this.ShaderAsset.Parameters["cameraPosition"].SetValue(renderSystem.GameCamera.PixelPosition);
            base.ConfigureShaderInternal(renderSystem, technique);
        }
        #endregion
    }
}
