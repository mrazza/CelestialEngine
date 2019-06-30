// -----------------------------------------------------------------------
// <copyright file="TiledSprite.cs" company="">
// Copyright (C) 2012 Matthew Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using System;
    using CelestialEngine.Core;
    using FarseerPhysics.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Creates an object that will tile the specified sprite over the specified area.
    /// </summary>
    public class TiledSprite : SimpleSprite
    {
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
            this.TileArea = tileArea;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the area (width/height) of the world to display the tiled sprite.
        /// </summary>
        /// <value>
        /// The tile area (width/height) in world space.
        /// </value>
        public Vector2 TileArea { get; set; }

        /// <summary>
        /// Gets or sets the offset, in world space, to use for the first column (X) and row (Y) for the sprite tiling.
        /// </summary>
        public Vector2 TilingOffset { get; set; }

        /// <summary>
        /// Gets the sprite's image bounds (where the image appears when rendered) in world units.
        /// </summary>
        public override RectangleF SpriteWorldRenderBounds
        {
            get
            {
                // Using the transform here let's us save some allocations.
                Transform spriteTransform;
                this.Body.GetTransform(out spriteTransform);
                float rotation = spriteTransform.q.GetAngle();
                var offset = this.SpriteOriginOffset.Rotate(rotation);
                return new RectangleF(
                    spriteTransform.p.X - offset.X,
                    spriteTransform.p.Y - offset.Y,
                    this.TileArea.X, this.TileArea.Y,
                    rotation);
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

            // return top left pos
            this.ForEachTile((pos, rect) => renderSystem.DrawSprite(this.SpriteTexture, pos, rect, this.RenderColor, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring));
            
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

            this.ForEachTile((pos, rect) => renderSystem.DrawSprite(this.SpriteNormalTexture, pos, rect, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring));

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

            this.ForEachTile((pos, rect) => renderSystem.DrawSprite(this.SpriteTexture, pos, rect, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring));

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
            var topLeft = this.SpriteWorldRenderBounds.TopLeft;
            var widthOffsetVector = this.RotationVector.Rotate(MathHelper.PiOver2);
            var heightOffsetVector = widthOffsetVector.Rotate(MathHelper.PiOver2);

            float widthDrawn = 0;
            int widthSpriteOffset = this.CalculateTilingPixelWidthOffset();
            for (int widthToDraw; (widthToDraw = this.CalculateWidthToDraw(widthSpriteOffset, widthDrawn)) > 0;)
            {
                var tilePosX = widthDrawn * widthOffsetVector;
                float heightDrawn = 0;
                int heightSpriteOffset = this.CalculateTilingPixelHeightOffset();
                for (int heightToDraw; (heightToDraw = this.CalculateHeightToDraw(heightSpriteOffset, heightDrawn)) > 0;)
                {
                    var tilePosY = heightDrawn * heightOffsetVector;
                    var tilePosition = tilePosX + tilePosY + topLeft;

                    // Construct the bounding rectangle
                    Rectangle boundingRect = new Rectangle(
                                                        this.SpriteTextureBoundingBox.Value.X + widthSpriteOffset, // Pixel space starting point (X) for the tile in the texture
                                                        this.SpriteTextureBoundingBox.Value.Y + heightSpriteOffset, // Pixel space starting point (Y) for the tile in the texture
                                                        widthToDraw, // Make sure we don't render outside the bounds of the tile area
                                                        heightToDraw); // Make sure we don't render outside the bounds of the tile area
                    drawCall(tilePosition, boundingRect); // Render it

                    heightSpriteOffset = 0;
                    heightDrawn += this.World.GetWorldFromPixel(heightToDraw) * this.RenderScale.Y;
                }
                widthSpriteOffset = 0;
                widthDrawn += this.World.GetWorldFromPixel(widthToDraw) * this.RenderScale.X;
            }
        }

        private int CalculateWidthToDraw(int widthOffset, float widthDrawn)
        {
            return (int)MathHelper.Min(this.SpriteTextureBoundingBox.Value.Width - widthOffset, this.World.GetPixelFromWorld(this.TileArea.X - widthDrawn) / this.RenderScale.X);
        }

        private int CalculateHeightToDraw(int heightOffset, float heightDrawn)
        {
            return (int)MathHelper.Min(this.SpriteTextureBoundingBox.Value.Height - heightOffset, this.World.GetPixelFromWorld(this.TileArea.Y - heightDrawn) / this.RenderScale.Y);
        }

        private int CalculateTilingPixelWidthOffset()
        {
            int tilingPixelOffset = (int)(this.World.GetPixelFromWorld(this.TilingOffset.X) / this.RenderScale.X) % this.SpriteTextureBoundingBox.Value.Width;
            if (tilingPixelOffset < 0)
            {
                tilingPixelOffset += this.SpriteTextureBoundingBox.Value.Width;
            }
            return tilingPixelOffset;
        }

        private int CalculateTilingPixelHeightOffset()
        {
            int tilingPixelOffset = (int)(this.World.GetPixelFromWorld(this.TilingOffset.Y) / this.RenderScale.Y) % this.SpriteTextureBoundingBox.Value.Height;
            if (tilingPixelOffset < 0)
            {
                tilingPixelOffset += this.SpriteTextureBoundingBox.Value.Height;
            }
            return tilingPixelOffset;
        }
        #endregion
    }
}
