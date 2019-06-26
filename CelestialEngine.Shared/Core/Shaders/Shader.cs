// -----------------------------------------------------------------------
// <copyright file="Shader.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Shaders
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents and provides management of a shader used when rendering.
    /// </summary>
    public class Shader
    {
        #region Members
        /// <summary>
        /// The name of the shader asset to load.
        /// </summary>
        private string shaderAssetPath;

        /// <summary>
        /// The actual shader asset/effect itself.
        /// </summary>
        private Effect shaderAsset;

        /// <summary>
        /// Denotes whether or not the shader has gone through initial configuration.
        /// </summary>
        private bool isConfigured;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class.
        /// </summary>
        /// <param name="assetPath">Name of the shader asset.</param>
        public Shader(string assetPath)
        {
            this.shaderAssetPath = assetPath;
            this.shaderAsset = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the path to the shader asset.
        /// </summary>
        public string ShaderAssetPath
        {
            get
            {
                return this.shaderAssetPath;
            }
        }

        /// <summary>
        /// Gets the shader effect.
        /// </summary>
        internal Effect ShaderAsset
        {
            get
            {
                return this.shaderAsset;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the specified shader parameter.
        /// </summary>
        /// <param name="parameter">The parameter name.</param>
        /// <returns>The specified shader/effect parameter.</returns>
        public EffectParameter GetParameter(string parameter)
        {
            if (!this.isConfigured)
            {
                Logger.Log(Logger.Level.Warning, "Attempted to set shader parameter \"{0}\" before configuring the shader.", parameter);
                return null;
            }
            else
            {
                return this.shaderAsset.Parameters[parameter];
            }
        }

        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        public virtual void LoadContent(ExtendedContentManager contentManager)
        {
            if (this.shaderAsset == null)
            {
                this.shaderAsset = contentManager.Load<Effect>(this.shaderAssetPath);
            }
        }

        /// <summary>
        /// Called when the shader parameters are to be configured before rendering.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        public void ConfigureShader(DeferredRenderSystem renderSystem)
        {
            this.ConfigureShader(renderSystem, 0);
        }

        /// <summary>
        /// Called when the shader parameters are to be configured before rendering.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        /// <param name="technique">Technique to use</param>
        public void ConfigureShader(DeferredRenderSystem renderSystem, string technique)
        {
            this.ConfigureShaderInternal(renderSystem, this.shaderAsset.Techniques[technique]);
        }

        /// <summary>
        /// Called when the shader parameters are to be configured before rendering.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        /// <param name="techniqueIndex">Index of the technique to use</param>
        public void ConfigureShader(DeferredRenderSystem renderSystem, int techniqueIndex)
        {
            this.ConfigureShaderInternal(renderSystem, this.shaderAsset.Techniques[techniqueIndex]);
        }

        /// <summary>
        /// Schedules a new shader refresh on the next draw call
        /// </summary>
        public void ScheduleFullRefresh()
        {
            this.isConfigured = false;
        }

        /// <summary>
        /// Begins the specified shader pass.
        /// </summary>
        /// <param name="passIndex">Index of the shader pass.</param>
        public void ApplyPass(int passIndex)
        {
            this.shaderAsset.CurrentTechnique.Passes[passIndex].Apply();
        }

        /// <summary>
        /// Begins the specified shader pass.
        /// </summary>
        /// <param name="passName">Name of the shader pass.</param>
        public void ApplyPass(string passName)
        {
            this.shaderAsset.CurrentTechnique.Passes[passName].Apply();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Called when the shader parameters are to be configured before rendering.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        /// <param name="technique">Technique to use</param>
        protected virtual void ConfigureShaderInternal(DeferredRenderSystem renderSystem, EffectTechnique technique)
        {
            if (!this.isConfigured)
            {
                this.InitConfigure(renderSystem);
            }

            this.shaderAsset.CurrentTechnique = technique;
        }

        /// <summary>
        /// Called when the shader parameters are to be configured initially.
        /// </summary>
        /// <param name="renderSystem">Instance of the <see cref="DeferredRenderSystem"/> we are using to render</param>
        protected virtual void InitConfigure(DeferredRenderSystem renderSystem)
        {
            this.isConfigured = true;
        }
        #endregion
    }
}
