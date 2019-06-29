// -----------------------------------------------------------------------
// <copyright file="DebugSprite.cs" company="">
// Copyright (C) 2012 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Renders debug information around the specified <see cref="SpriteBase"/> instance.
    /// </summary>
    public class DebugSprite : SpriteBase
    {
        #region Members
        /// <summary>
        /// The sprite that we're drawing debug information for.
        /// </summary>
        private SpriteBase targetSprite;

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
        /// Initializes a new instance of the <see cref="DebugSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="targetSprite">The sprite that we're drawing debug information for.</param>
        public DebugSprite(World world, SpriteBase targetSprite)
            : base(world)
        {
            this.targetSprite = targetSprite;
            this.lineWidth = this.World.GetWorldFromPixel(1.0f); // Default to 1 pixel in width
            this.optionMapFlagsShader = new OptionsMapFlagsShader();
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
        /// Gets or sets the sprite instance that we're drawing debug information for.
        /// </summary>
        /// <value>
        /// The sprite instance that we're drawing debug information for.
        /// </value>
        public SpriteBase TargetSprite
        {
            get
            {
                return this.targetSprite;
            }

            set
            {
                this.targetSprite = value;
            }
        }

        /// <summary>
        /// Gets the sprite's image bounds in world units.
        /// </summary>
        public override RectangleF SpriteWorldRenderBounds
        {
            get
            {
                return this.targetSprite.SpriteWorldRenderBounds;
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
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public override void Update(GameTime gameTime)
        {
            this.LayerDepth = (byte)(this.targetSprite.LayerDepth + 1);
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

            RectangleF posRect = new RectangleF(this.targetSprite.Position.X - this.lineWidth * 2.0f, this.targetSprite.Position.Y - this.lineWidth * 2.0f, this.lineWidth * 4.0f, this.lineWidth * 4.0f);

            renderSystem.DrawFilledRectangle(posRect, Color.White, this.Rotation); // Draw position

            // Draw shape outline if it exists
            if (this.targetSprite.SpriteWorldVertices != null)
            {
                for (int index = 0; index < this.targetSprite.SpriteWorldVertices.Count - 1; index++)
                {
                    renderSystem.DrawLine(this.targetSprite.SpriteWorldVertices[index], this.targetSprite.SpriteWorldVertices[index + 1], Color.Red, this.lineWidth);
                }
            }

            renderSystem.DrawRectangleBorder(this.targetSprite.SpriteWorldRenderBounds, Color.Green, this.lineWidth, 0.0f); // Draw bounding rectangle
            renderSystem.DrawLine(this.targetSprite.Position, this.targetSprite.Position + this.targetSprite.Velocity, Color.Blue, this.lineWidth); // Draw velocity line

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

            RectangleF posRect = new RectangleF(this.targetSprite.Position.X - this.lineWidth * 2.0f, this.targetSprite.Position.Y - this.lineWidth * 2.0f, this.lineWidth * 4.0f, this.lineWidth * 4.0f);

            renderSystem.DrawFilledRectangle(posRect, Color.Black, this.Rotation); // Draw position
            renderSystem.DrawRectangleBorder(this.targetSprite.SpriteWorldRenderBounds, Color.Black, this.lineWidth, 0.0f); // Draw bounding rectangle
            renderSystem.DrawLine(this.targetSprite.Position, this.targetSprite.Position + this.targetSprite.Velocity, Color.Black, this.lineWidth); // Draw velocity line

            renderSystem.EndRender();
        }
        #endregion
    }
}
