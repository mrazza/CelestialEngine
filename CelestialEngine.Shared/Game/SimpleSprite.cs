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
    using Microsoft.Xna.Framework.Graphics;
    using FarseerPhysics.Common;
    using FarseerPhysics.Common.Decomposition;
    using System.Collections.Generic;
    using System.Linq;

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
        /// The dimensions, in world space, of the sprite's texture bound box.
        /// </summary>
        private Vector2 spriteTextureWorldDimensions;

        /// <summary>
        /// The shader used to apply pixel options to the option map.
        /// </summary>
        private OptionsMapFlagsShader optionMapFlagsShader;

        /// <summary>
        /// The color to use when rendering this sprite (default is White).
        /// </summary>
        private Color renderColor;

        /// <summary>
        /// If true, the sprite's shape is computed from its image data
        /// </summary>
        private bool computeSpriteShape;

        /// <summary>
        /// Vertices of the sprite in world units.
        /// </summary>
        protected Vertices spriteVertices;

        /// <summary>
        /// Collection of convex shapes containing vertices that make up this sprite.
        /// </summary>
        protected List<Vertices> spriteShapes;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="computeSpriteShape">If true, the sprite's shape is computed based on the sprite data; otherwise, the sprite's bounding box is used.</param>
        public SimpleSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, bool computeSpriteShape)
            : this(world, spriteTexturePath, spriteNormalTexturePath, null, Vector2.Zero, Vector2.Zero, computeSpriteShape)
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
        /// <param name="computeSpriteShape">If true, the sprite's shape is computed based on the sprite data; otherwise, the sprite's bounding box is used.</param>
        public SimpleSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Rectangle? spriteTextureBoundingBox, Vector2 position, Vector2 velocity, bool computeSpriteShape)
            : base(world, position, velocity)
        {
            this.spriteTexturePath = spriteTexturePath ?? throw new ArgumentNullException("spriteTexturePath", "You must specify the texture to render.");
            this.spriteNormalTexturePath = spriteNormalTexturePath;
            this.spriteTextureBoundingBox = spriteTextureBoundingBox;
            this.optionMapFlagsShader = new OptionsMapFlagsShader();
            this.NormalMapShader = new NormalMapShader();
            this.renderColor = Color.White;
            this.computeSpriteShape = computeSpriteShape;
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
        /// <remarks>
        /// This is an approximate bounding rectangle for the area impacted by this sprite. The actual area impacted may
        /// be smaller than this bounding rectangle and must NEVER be greater. For more percise bounding shapes, use the
        /// <see cref="SpriteWorldShape"/> property.
        /// </remarks>
        public override RectangleF SpriteWorldBounds
        {
            get
            {
                Transform spriteTransform;
                this.Body.GetTransform(out spriteTransform);
                if (this.spriteTextureBoundingBox == null)
                {
                    return new RectangleF(
                        spriteTransform.p.X,
                        spriteTransform.p.Y,
                        0,
                        0,
                        this.Rotation);
                }
                else
                {
                    return new RectangleF(
                        spriteTransform.p.X,
                        spriteTransform.p.Y,
                        this.spriteTextureWorldDimensions.X * this.RenderScale.X,
                        this.spriteTextureWorldDimensions.Y * this.RenderScale.Y,
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

        /// <summary>
        /// Gets the normal map shader.
        /// </summary>
        protected NormalMapShader NormalMapShader
        {
            get;
            set;
        }
        #endregion

        #region SimBase Overrides
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // TODO: Optimize this.
            if (this.spriteVertices != null)
            {
                this.SpriteWorldVertices = new Vertices(this.spriteVertices.Select(v => (v * this.RenderScale * this.World.WorldPerPixelRatio).Rotate(this.Rotation) + this.Position));
            }

            if (this.spriteShapes != null)
            {
                this.SpriteWorldShapes = this.spriteShapes.Select(shape => new Vertices(shape.Select(v => (v * this.RenderScale * this.World.WorldPerPixelRatio).Rotate(this.Rotation) + this.Position)));
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

            if (this.spriteTextureBoundingBox == null)
            {
                this.spriteTextureBoundingBox = this.spriteTexture.Bounds;
            }

            this.spriteTextureWorldDimensions = new Vector2(this.World.GetWorldFromPixel(this.spriteTextureBoundingBox.Value.Width), this.World.GetWorldFromPixel(this.spriteTextureBoundingBox.Value.Height));

            uint[] pixelData = null;

            if (this.computeSpriteShape)
            {
                pixelData = new uint[this.spriteTextureBoundingBox.Value.Width * this.spriteTextureBoundingBox.Value.Height];
                this.spriteTexture.GetData(0, spriteTextureBoundingBox, pixelData, 0, pixelData.Length);
                this.spriteVertices = PolygonTools.CreatePolygon(pixelData, this.spriteTextureBoundingBox.Value.Width);
                this.SpriteWorldVertices = this.spriteVertices;
                this.spriteShapes = Triangulate.ConvexPartition(this.SpriteWorldVertices, TriangulationAlgorithm.Bayazit);
                this.SpriteWorldShapes = this.spriteShapes;
            }

            this.optionMapFlagsShader.LoadContent(contentManager);
            this.NormalMapShader.LoadContent(contentManager);

            if (this.spriteNormalTexturePath != null)
            {
                this.spriteNormalTexture = contentManager.Load<Texture2D>(this.spriteNormalTexturePath);
            }
            else
            {
                this.spriteNormalTexture = new Texture2D(contentManager.Game.GraphicsDevice, this.spriteTextureBoundingBox.Value.Width, this.spriteTextureBoundingBox.Value.Height);

                if (pixelData == null)
                {
                    pixelData = new uint[this.spriteTextureBoundingBox.Value.Width * this.spriteTextureBoundingBox.Value.Height];
                    this.spriteTexture.GetData(0, spriteTextureBoundingBox, pixelData, 0, pixelData.Length);
                }

                // Convert the image to a normal map with all vectors pointing UP (saving Alpha channel)
                for (int i = 0; i < pixelData.Length; i++)
                {
                    pixelData[i] = (255U << 24 & pixelData[i]) + (255U << 16) + (128U << 8) + 128U;
                }

                this.spriteNormalTexture.SetData(pixelData);
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
            renderSystem.DrawSprite(this.spriteTexture, this.Position, drawBoundingBox, this.renderColor, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring);
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
            renderSystem.BeginRender(this.NormalMapShader);
            this.NormalMapShader.ConfigureShaderAndApplyPass(renderSystem, this);
            renderSystem.DrawSprite(this.spriteNormalTexture, this.Position, drawBoundingBox, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring);
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
            renderSystem.DrawSprite(this.spriteTexture, this.Position, drawBoundingBox, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring);
            renderSystem.EndRender();
        }
        #endregion
    }
}
