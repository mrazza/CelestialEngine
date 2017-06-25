// -----------------------------------------------------------------------
// <copyright file="NormalMapShader.cs" company="">
// Copyright (C) Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Shaders
{
    using System;
    using CelestialEngine.Game;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Internal normal map shader that handles transformations in texture normals
    /// </summary>
    public class NormalMapShader : Shader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalMapShader"/> class.
        /// </summaryNormalMapShader
        public NormalMapShader()
            : base(Content.Shaders.Core.NormalMapShader)
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
            base.ConfigureShader(renderSystem, "RenderNormalMap");

            this.ShaderAsset.Parameters["spriteRotationMatrix"].SetValue(Matrix.CreateRotationZ(sprite.Rotation));
            this.ApplyPass(0);
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
            if (!technique.Name.Equals("RenderNormalMap"))
            {
                throw new ArgumentException("You dun goofed.");
            }

            this.ShaderAsset.Parameters["viewProjection"].SetValue(renderSystem.GameCamera.GetViewProjectionMatrix(renderSystem));
            this.ShaderAsset.Parameters["cameraPosition"].SetValue(renderSystem.GameCamera.PixelPosition);

            base.ConfigureShader(renderSystem, technique);
        }
        #endregion
    }
}
