// -----------------------------------------------------------------------
// <copyright file="DebugSimulatedPostProcess.cs" company="">
// Copyright (C) 2011-2013 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using CelestialEngine.Core.PostProcess;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Renders debug information around the specified <see cref="SimulatedPostProcess"/> instance.
    /// </summary>
    public class DebugSimulatedPostProcess : SpriteBase
    {
        #region Members
        /// <summary>
        /// The SimulatedPostProcess that we're drawing debug information for.
        /// </summary>
        private SimulatedPostProcess targetPostProcess;

        /// <summary>
        /// The width of the line (in world units) to use when rendering.
        /// </summary>
        private float lineWidth;

        /// <summary>
        /// The shader used to apply pixel options to the option map.
        /// </summary>
        private OptionsMapFlagsShader optionMapFlagsShader;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSimulatedPostProcess"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="targetPostProcess">The post process that we're drawing debug information for.</param>
        public DebugSimulatedPostProcess(World world, SimulatedPostProcess targetPostProcess)
            : base(world)
        {
            this.targetPostProcess = targetPostProcess;
            this.lineWidth = this.World.GetWorldFromPixel(1.0f); // Default to 1 pixel in width
            this.optionMapFlagsShader = new OptionsMapFlagsShader();
            this.LayerDepth = float.MaxValue; // Arbitrarily large layer depth
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width of the line (in world units) to use when rendering.
        /// </summary>
        public float LineWidth
        {
            get
            {
                return this.lineWidth;
            }

            set
            {
                this.lineWidth = MathHelper.Max(0, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the post process instance that we're drawing debug information for.
        /// </summary>
        /// <value>
        /// The post process instance that we're drawing debug information for.
        /// </value>
        public SimulatedPostProcess TargetPostProcess
        {
            get
            {
                return this.targetPostProcess;
            }

            set
            {
                this.targetPostProcess = value;
            }
        }

        /// <summary>
        /// Gets the sprite's image bounds in world units.
        /// </summary>
        public override RectangleF SpriteWorldBounds
        {
            get
            {
                return this.targetPostProcess.GetWorldDrawBounds();
            }
        }
        #endregion

        #region IDrawableComponent Overrides
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        public override void LoadContent(ExtendedContentManager contentManager)
        {
            this.optionMapFlagsShader.LoadContent(contentManager);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its color map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            renderSystem.BeginRender();

            RectangleF posRect = new RectangleF(this.targetPostProcess.Position.X - this.lineWidth * 2.0f, this.targetPostProcess.Position.Y - this.lineWidth * 2.0f, this.lineWidth * 4.0f, this.lineWidth * 4.0f);
            float lineMag = this.targetPostProcess.Velocity.Length();

            renderSystem.DrawFilledRectangle(posRect, Color.White, this.targetPostProcess.Rotation); // Draw position
            renderSystem.DrawRectangleBorder(this.targetPostProcess.GetWorldDrawBounds(), Color.Green, this.lineWidth, 0.0f); // Draw bounding rectangle
            renderSystem.DrawLine(this.targetPostProcess.Position, this.targetPostProcess.Position + this.targetPostProcess.Velocity, Color.Blue, this.lineWidth); // Draw velocity line
            renderSystem.DrawLine(this.targetPostProcess.Position, this.targetPostProcess.Position + (lineMag * this.targetPostProcess.RotationVector), Color.OrangeRed, this.lineWidth);
            
            renderSystem.EndRender();
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its normal map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawNormalMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            return;
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            renderSystem.BeginRender(this.optionMapFlagsShader);
            this.optionMapFlagsShader.ConfigureShaderAndApplyPass(renderSystem, this);

            RectangleF posRect = new RectangleF(this.targetPostProcess.Position.X - this.lineWidth * 2.0f, this.targetPostProcess.Position.Y - this.lineWidth * 2.0f, this.lineWidth * 4.0f, this.lineWidth * 4.0f);
            float lineMag = this.targetPostProcess.Velocity.Length();

            renderSystem.DrawFilledRectangle(posRect, Color.Black, this.targetPostProcess.Rotation); // Draw position
            renderSystem.DrawRectangleBorder(this.targetPostProcess.GetWorldDrawBounds(), Color.Black, this.lineWidth, 0.0f); // Draw bounding rectangle
            renderSystem.DrawLine(this.targetPostProcess.Position, this.targetPostProcess.Position + this.targetPostProcess.Velocity, Color.Black, this.lineWidth); // Draw velocity line
            renderSystem.DrawLine(this.targetPostProcess.Position, this.targetPostProcess.Position + (lineMag * this.targetPostProcess.RotationVector), Color.Black, this.lineWidth);
            
            renderSystem.EndRender();
        }
        #endregion
    }
}
