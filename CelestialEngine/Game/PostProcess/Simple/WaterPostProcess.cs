// -----------------------------------------------------------------------
// <copyright file="WaterPostProcess.cs" company="">
// Copyright (C) 2011-2012 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game.PostProcess.Lights
{
    using System;
    using CelestialEngine.Core;
    using CelestialEngine.Core.PostProcess;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A water-like fullscreen post process effect
    /// </summary>
    public class WaterPostProcess : StaticPostProcess
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WaterPostProcess"/> class.
        /// </summary>
        public WaterPostProcess()
            : base(new Shader(Content.Shaders.Simple.WaterEffect))
        {
        }
        #endregion

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem" /> when this <see cref="StaticPostProcess" /> component needs render itself.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem" /> to render with.</param>
        public override void Draw(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            RenderTarget2D intermediateColorMap = renderSystem.RenderTargets.GetTemporaryRenderTarget(SurfaceFormat.Color); // Intermediate map used for selective sampling
            renderSystem.GraphicsDevice.SetRenderTarget(intermediateColorMap);
            this.PostProcessEffect.ConfigureShader(renderSystem);
            this.PostProcessEffect.GetParameter("spriteTexture").SetValue(renderSystem.RenderTargets.ColorMap);
            this.PostProcessEffect.ApplyPass(0);
            renderSystem.DirectScreenPaint();

            renderSystem.SetRenderTargets(RenderTargetTypes.ColorMap, 1);
            this.PostProcessEffect.ConfigureShader(renderSystem);
            this.PostProcessEffect.GetParameter("spriteTexture").SetValue(intermediateColorMap);
            this.PostProcessEffect.ApplyPass(0);
            renderSystem.DirectScreenPaint();
            renderSystem.RenderTargets.ReleaseTemporaryRenderTarget(intermediateColorMap);
        }
    }
}