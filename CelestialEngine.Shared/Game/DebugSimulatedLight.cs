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
            this.RenderShadowMapBounds = true;
            this.ShadowMapBoundsLineColor = Color.Red;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether or not to render the bounds of the shadow map used when lighting.
        /// </summary>
        public bool RenderShadowMapBounds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line <see cref="Color"/> to use when rendering the shadow map boundaries.
        /// </summary>
        public Color ShadowMapBoundsLineColor
        {
            get;
            set;
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

            // Render the lines around all the triangles in the primitive triangle strips
            if (this.RenderShadowMapBounds)
            {
                renderSystem.BeginRender();
                foreach (var shadowArea in targetLight.GetAllShadowPrimitives())
                {
                    for (int index = 0; index < shadowArea.Count - 2; index++)
                    {
                        renderSystem.DrawLine(shadowArea[index], shadowArea[index + 1], this.ShadowMapBoundsLineColor, this.lineWidth);
                        renderSystem.DrawLine(shadowArea[index + 2], shadowArea[index + 1], this.ShadowMapBoundsLineColor, this.lineWidth);
                        renderSystem.DrawLine(shadowArea[index], shadowArea[index + 2], this.ShadowMapBoundsLineColor, this.lineWidth);
                    }
                }
                renderSystem.EndRender();
            }
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

            // Turn off lighting for the lines around all the triangles in the primitive triangle strips
            if (this.RenderShadowMapBounds)
            {
                renderSystem.BeginRender(this.optionMapFlagsShader);
                this.optionMapFlagsShader.ConfigureShaderAndApplyPass(renderSystem, this);
                foreach (var shadowArea in targetLight.GetAllShadowPrimitives())
                {
                    for (int index = 0; index < shadowArea.Count - 2; index++)
                    {
                        renderSystem.DrawLine(shadowArea[index], shadowArea[index + 1], Color.Black, this.lineWidth);
                        renderSystem.DrawLine(shadowArea[index + 2], shadowArea[index + 1], Color.Black, this.lineWidth);
                        renderSystem.DrawLine(shadowArea[index], shadowArea[index + 2], Color.Black, this.lineWidth);
                    }
                }
                renderSystem.EndRender();
            }
        }
        #endregion
    }
}
