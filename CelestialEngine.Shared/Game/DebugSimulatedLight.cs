// -----------------------------------------------------------------------
// <copyright file="DebugSimulatedPostProcess.cs" company="">
// Copyright (C) 2017 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using CelestialEngine.Core.PostProcess;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Renders debug information around the specified <see cref="DebugSimulatedLight"/> instance.
    /// </summary>
    public class DebugSimulatedLight : DebugSimulatedPostProcess
    {
        #region Fields
        /// <summary>
        /// The light that we're drawing debug info for.
        /// </summary>
        private SimulatedLight targetLight;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSimulatedLight"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="targetLight">The light that we're drawing debug information for.</param>
        public DebugSimulatedLight(World world, SimulatedLight targetLight)
            : base(world, targetLight)
        {
            this.targetLight = targetLight;
        }
        #endregion

        #region IDrawableComponent Overrides
        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its color map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            base.DrawColorMap(gameTime, renderSystem);

            renderSystem.BeginRender();
            var shadowAreas = targetLight.GetAllShadowPrimitives();
            foreach (var shadowArea in shadowAreas)
            {
                for (int index = 0; index < shadowArea.Count - 2; index++)
                {
                    renderSystem.DrawLine(shadowArea[index], shadowArea[index + 1], Color.Red, 0.05f);
                    renderSystem.DrawLine(shadowArea[index + 2], shadowArea[index + 1], Color.Red, 0.05f);
                    renderSystem.DrawLine(shadowArea[index], shadowArea[index + 2], Color.Red, 0.05f);
                }
            }
            renderSystem.EndRender();
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            base.DrawOptionsMap(gameTime, renderSystem);

            renderSystem.BeginRender();
            var shadowAreas = targetLight.GetAllShadowPrimitives();
            foreach (var shadowArea in shadowAreas)
            {
                for (int index = 0; index < shadowArea.Count - 2; index++)
                {
                    renderSystem.DrawLine(shadowArea[index], shadowArea[index + 1], Color.Black, 0.05f);
                    renderSystem.DrawLine(shadowArea[index + 2], shadowArea[index + 1], Color.Black, 0.05f);
                    renderSystem.DrawLine(shadowArea[index], shadowArea[index + 2], Color.Black, 0.05f);
                }
            }
            renderSystem.EndRender();
        }
        #endregion
    }
}
