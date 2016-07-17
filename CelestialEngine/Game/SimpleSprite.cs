// -----------------------------------------------------------------------
// <copyright file="SimpleSprite.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using System;
    using CelestialEngine.Core;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A simple <see cref="SpriteBase"/> implementation that renders just a color sprite.
    /// </summary>
    public class SimpleSprite : SpriteBase
    {
        #region Members
        /// <summary>
        /// Path to the sprite texture to render
        /// </summary>
        private string spriteTexturePath;

        /// <summary>
        /// Path to the normal texture to render
        /// </summary>
        private string spriteNormalTexturePath;

        /// <summary>
        /// Sprite texture asset
        /// </summary>
        private Texture2D spriteTexture;

        /// <summary>
        /// Sprite normal texture asset
        /// </summary>
        private Texture2D spriteNormalTexture;

        /// <summary>
        /// Represents an area within the texture that contains the sprite; used for sprites that are within a sprite map.
        /// </summary>
        private Rectangle? spriteTextureBoundingBox;

        /// <summary>
        /// The shader used to apply pixel options to the option map.
        /// </summary>
        private OptionsMapFlagsShader optionMapFlagsShader;

        /// <summary>
        /// The color to use when rendering this sprite (default is White).
        /// </summary>
        private Color renderColor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        public SimpleSprite(World world, string spriteTexturePath, string spriteNormalTexturePath)
            : this(world, spriteTexturePath, spriteNormalTexturePath, null, Vector2.Zero, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="spriteTextureBoundingBox">The bounding box of the sprite within the texture (used with sprite sheets).</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The starting velocity of the object.</param>
        public SimpleSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Rectangle? spriteTextureBoundingBox, Vector2 position, Vector2 velocity)
            : base(world, position, velocity)
        {
            if (spriteTexturePath == null)
            {
                throw new ArgumentNullException("spriteTexturePath", "You must specify the texture to render.");
            }

            this.spriteTexturePath = spriteTexturePath;
            this.spriteNormalTexturePath = spriteNormalTexturePath;
            this.spriteTextureBoundingBox = spriteTextureBoundingBox;
            this.optionMapFlagsShader = new OptionsMapFlagsShader();
            this.renderColor = Color.White;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color to tint the sprite with when rendering.
        /// </summary>
        public Color RenderColor
        {
            get
            {
                return this.renderColor;
            }

            set
            {
                this.renderColor = value;
            }
        }

        /// <summary>
        /// Gets the sprite's image bounds (where the image appears when rendered) in world units.
        /// </summary>
        public override RectangleF SpriteWorldBounds
        {
            get
            {
                if (this.spriteTextureBoundingBox == null)
                {
                    return new RectangleF(
                        this.Position.X,
                        this.Position.Y,
                        0,
                        0,
                        this.Rotation);
                }
                else
                {
                    return new RectangleF(
                        this.Position.X,
                        this.Position.Y,
                        this.World.GetWorldFromPixel(this.spriteTextureBoundingBox.Value.Width) * this.RenderScale.X,
                        this.World.GetWorldFromPixel(this.spriteTextureBoundingBox.Value.Height) * this.RenderScale.Y,
                        this.Rotation);
                }
            }
        }

        /// <summary>
        /// Gets the sprite texture asset
        /// </summary>
        protected Texture2D SpriteTexture
        {
            get
            {
                return this.spriteTexture;
            }
        }

        /// <summary>
        /// Gets the sprite normal-map texture asset.
        /// </summary>
        protected Texture2D SpriteNormalTexture
        {
            get
            {
                return this.spriteNormalTexture;
            }
        }

        /// <summary>
        /// Gets the sprite texture bounding box (box that represents where in the sprite to grab data for rendering).
        /// </summary>
        protected Rectangle? SpriteTextureBoundingBox
        {
            get
            {
                return this.spriteTextureBoundingBox;
            }
        }

        /// <summary>
        /// Gets the option map flags shader.
        /// </summary>
        protected OptionsMapFlagsShader OptionMapFlagsShader
        {
            get
            {
                return this.optionMapFlagsShader;
            }
        }
        #endregion

        #region SpriteBase Overrides
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        public override void LoadContent(ExtendedContentManager contentManager)
        {
            this.spriteTexture = contentManager.Load<Texture2D>(this.spriteTexturePath);
            this.optionMapFlagsShader.LoadContent(contentManager);

            if (this.spriteTextureBoundingBox == null)
            {
                this.spriteTextureBoundingBox = this.spriteTexture.Bounds;
            }

            if (this.spriteNormalTexturePath != null)
            {
                this.spriteNormalTexture = contentManager.Load<Texture2D>(this.spriteNormalTexturePath);
            }
            else
            {
                this.spriteNormalTexture = new Texture2D(contentManager.Game.GraphicsDevice, this.spriteTextureBoundingBox.Value.Width, this.spriteTextureBoundingBox.Value.Height);
                Color[] pixelData = new Color[this.spriteTexture.Width * this.spriteTexture.Height];
                this.spriteTexture.GetData<Color>(pixelData);

                // Convert the image to black (saving Alpha channel)
                for (int i = 0; i < pixelData.Length; i++)
                {
                    pixelData[i].R = pixelData[i].G = pixelData[i].B = 0;
                }

                this.spriteNormalTexture.SetData<Color>(pixelData);
            }
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to be drawn.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            this.DrawColorMap(gameTime, renderSystem, this.spriteTextureBoundingBox.Value);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its normal map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawNormalMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            this.DrawNormalMap(gameTime, renderSystem, this.spriteTextureBoundingBox.Value);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawOptionsMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            this.DrawOptionsMap(gameTime, renderSystem, this.spriteTextureBoundingBox.Value);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to be drawn.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        /// <param name="drawBoundingBox">The bounding box of the sprite within the texture.</param>
        protected void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem, Rectangle drawBoundingBox)
        {
            renderSystem.BeginRender();
            renderSystem.DrawSprite(this.spriteTexture, this.Position, drawBoundingBox, this.renderColor, this.Rotation, Vector2.Zero, this.RenderScale, SpriteEffects.None);
            renderSystem.EndRender();
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its normal map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        /// <param name="drawBoundingBox">The bounding box of the sprite within the texture.</param>
        protected void DrawNormalMap(GameTime gameTime, DeferredRenderSystem renderSystem, Rectangle drawBoundingBox)
        {
            renderSystem.BeginRender();
            renderSystem.DrawSprite(this.spriteNormalTexture, this.Position, drawBoundingBox, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, SpriteEffects.None);
            renderSystem.EndRender();
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawOptionsMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        /// <param name="drawBoundingBox">The bounding box of the sprite within the texture.</param>
        protected void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem, Rectangle drawBoundingBox)
        {
            renderSystem.BeginRender(this.optionMapFlagsShader);
            this.optionMapFlagsShader.ConfigureShaderAndApplyPass(renderSystem, this);
            renderSystem.DrawSprite(this.spriteTexture, this.Position, drawBoundingBox, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, SpriteEffects.None);
            renderSystem.EndRender();
        }
        #endregion
    }
}
