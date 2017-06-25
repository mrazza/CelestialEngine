// -----------------------------------------------------------------------
// <copyright file="TiledSprite.cs" company="">
// Copyright (C) 2012 Matthew Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using System;
    using CelestialEngine.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Creates an object that will tile the specified sprite over the specified area.
    /// </summary>
    public class TiledSprite : SimpleSprite
    {
        #region Memebers
        /// <summary>
        /// Area (width/height) of the world to display the tiled sprite.
        /// </summary>
        private Vector2 tileArea;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="position">The position of the tiled sprite.</param>
        /// <param name="tileArea">Area (width/height) of the world to display the tiled sprite.</param>
        public TiledSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Vector2 position, Vector2 tileArea)
            : this(world, spriteTexturePath, spriteNormalTexturePath, null, position, Vector2.Zero, tileArea)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="spriteTextureBoundingBox">The bounding box that selects what area of the texture to tile.</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The starting velocity of the object.</param>
        /// <param name="tileArea">Area (width/height) of the world to display the tiled sprite.</param>
        public TiledSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Rectangle? spriteTextureBoundingBox, Vector2 position, Vector2 velocity, Vector2 tileArea)
            : base(world, spriteTexturePath, spriteNormalTexturePath, spriteTextureBoundingBox, position, velocity, false)
        {
            this.tileArea = tileArea;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the area (width/height) of the world to display the tiled sprite.
        /// </summary>
        /// <value>
        /// The tile area (width/height) in world space.
        /// </value>
        public Vector2 TileArea
        {
            get
            {
                return this.tileArea;
            }

            set
            {
                this.tileArea = value;
            }
        }

        /// <summary>
        /// Gets the sprite's image bounds (where the image appears when rendered) in world units.
        /// </summary>
        public override RectangleF SpriteWorldBounds
        {
            get
            {
                return new RectangleF(this.Position.X, this.Position.Y, this.TileArea.X, this.TileArea.Y, this.Rotation);
            }
        }
        #endregion

        #region SpriteBase Overrides
        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to be drawn.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            renderSystem.BeginRender();

            this.ForEachTile((pos, rect) => renderSystem.DrawSprite(this.SpriteTexture, pos, rect, this.RenderColor, this.Rotation, Vector2.Zero, this.RenderScale, SpriteEffects.None));
            
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
            renderSystem.BeginRender(this.NormalMapShader);
            this.NormalMapShader.ConfigureShaderAndApplyPass(renderSystem, this);

            this.ForEachTile((pos, rect) => renderSystem.DrawSprite(this.SpriteNormalTexture, pos, rect, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, SpriteEffects.None));

            renderSystem.EndRender();
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawOptionsMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            renderSystem.BeginRender(this.OptionMapFlagsShader);
            this.OptionMapFlagsShader.ConfigureShaderAndApplyPass(renderSystem, this);

            this.ForEachTile((pos, rect) => renderSystem.DrawSprite(this.SpriteTexture, pos, rect, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, SpriteEffects.None));

            renderSystem.EndRender();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Executes the specified draw call for each tile.
        /// </summary>
        /// <param name="drawCall">The draw call to exectute.</param>
        protected void ForEachTile(Action<Vector2, Rectangle> drawCall)
        {
            // Loop across each column and down each row (in world space)
            for (float widthCount = 0; widthCount < this.tileArea.X; widthCount += this.World.GetWorldFromPixel(this.SpriteTextureBoundingBox.Value.Width * this.RenderScale.X))
            {
                for (float heightCount = 0; heightCount < this.tileArea.Y; heightCount += this.World.GetWorldFromPixel(this.SpriteTextureBoundingBox.Value.Height * this.RenderScale.Y))
                {
                    Vector2 pos = new Vector2(widthCount + this.Position.X, heightCount + this.Position.Y); // Get the position of the next tile (world space)
                    
                    // Construct the bounding rectangle
                    Rectangle boundingRect = new Rectangle(
                                                        this.SpriteTextureBoundingBox.Value.X, // Pixel space starting point (X) for the tile in the texture
                                                        this.SpriteTextureBoundingBox.Value.Y, // Pixel space starting point (Y) for the tile in the texture
                                                        (int)MathHelper.Min(this.SpriteTextureBoundingBox.Value.Width, this.World.GetPixelFromWorld((this.tileArea.X - widthCount)) / this.RenderScale.X), // Make sure we don't render outside the bounds of the tile area
                                                        (int)MathHelper.Min(this.SpriteTextureBoundingBox.Value.Height, this.World.GetPixelFromWorld((this.tileArea.Y - heightCount)) / this.RenderScale.Y)); // Make sure we don't render outside the bounds of the tile area
                    
                    drawCall(pos, boundingRect); // Render it
                }
            }
        }
        #endregion
    }
}
