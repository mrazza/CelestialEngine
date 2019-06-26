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
        private readonly string spriteTexturePath;

        /// <summary>
        /// Path to the normal texture to render
        /// </summary>
        private readonly string spriteNormalTexturePath;

        /// <summary>
        /// The dimensions, in world space, of the sprite's texture bound box.
        /// </summary>
        private Vector2 spriteTextureWorldDimensions;

        /// <summary>
        /// If true, the sprite's shape is computed from its image data
        /// </summary>
        private readonly bool computeSpriteShape;

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
            this.SpriteTextureBoundingBox = spriteTextureBoundingBox;
            this.OptionMapFlagsShader = new OptionsMapFlagsShader();
            this.NormalMapShader = new NormalMapShader();
            this.RenderColor = Color.White;
            this.computeSpriteShape = computeSpriteShape;
            this.SpriteOriginOffset = Vector2.Zero;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The offset, in pixel units, from the top left of the sprite to the location to treat as the origin of the sprite.
        /// </summary>
        public Vector2 SpritePixelOriginOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The offset, in world units, from the top left of the sprite to the location to treat as the origin of the sprite.
        /// </summary>
        public Vector2 SpriteOriginOffset
        {
            get
            {
                return this.World.GetWorldFromPixel(this.SpritePixelOriginOffset) * this.RenderScale;
            }

            set
            {
                this.SpritePixelOriginOffset = this.World.GetPixelFromWorld(value / this.RenderScale);
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
                // Using the transform here let's us save some allocations.
                Transform spriteTransform;
                this.Body.GetTransform(out spriteTransform);
                float rotation = spriteTransform.q.GetAngle();
                var offset = this.SpriteOriginOffset.Rotate(rotation);
                if (this.SpriteTextureBoundingBox == null)
                {
                    return new RectangleF(
                        spriteTransform.p.X - offset.X,
                        spriteTransform.p.Y - offset.Y,
                        0,
                        0,
                        rotation);
                }
                else
                {
                    return new RectangleF(
                        spriteTransform.p.X - offset.X,
                        spriteTransform.p.Y - offset.Y,
                        this.spriteTextureWorldDimensions.X * this.RenderScale.X,
                        this.spriteTextureWorldDimensions.Y * this.RenderScale.Y,
                        rotation);
                }
            }
        }

        /// <summary>
        /// Gets the sprite texture asset
        /// </summary>
        protected Texture2D SpriteTexture { get; private set; }

        /// <summary>
        /// Gets the sprite normal-map texture asset.
        /// </summary>
        protected Texture2D SpriteNormalTexture { get; private set; }

        /// <summary>
        /// Gets the sprite texture bounding box (box that represents where in the sprite to grab data for rendering).
        /// </summary>
        protected Rectangle? SpriteTextureBoundingBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the option map flags shader.
        /// </summary>
        protected OptionsMapFlagsShader OptionMapFlagsShader { get; }

        /// <summary>
        /// Gets the normal map shader.
        /// </summary>
        protected NormalMapShader NormalMapShader
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color to use when rendering this sprite (default is White).
        /// </summary>
        public Color RenderColor
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
            RectangleF worldDrawBounds = null;
            if (this.spriteVertices != null || this.spriteShapes != null)
            {
                worldDrawBounds = this.SpriteWorldBounds;
            }

            if (this.spriteVertices != null)
            {
                this.SpriteWorldVertices = new Vertices(this.spriteVertices.Select(v => (v * this.RenderScale * this.World.WorldPerPixelRatio).Rotate(this.Rotation) + worldDrawBounds.Position));
            }

            if (this.spriteShapes != null)
            {
                this.SpriteWorldShapes = this.spriteShapes.Select(shape => new Vertices(shape.Select(v => (v * this.RenderScale * this.World.WorldPerPixelRatio).Rotate(this.Rotation) + worldDrawBounds.Position)));
            }
        }
        #endregion

        #region SpriteBase Overrides
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        public override void LoadContent(ExtendedContentManager contentManager)
        {
            this.SpriteTexture = contentManager.Load<Texture2D>(this.spriteTexturePath);

            if (this.SpriteTextureBoundingBox == null)
            {
                this.SpriteTextureBoundingBox = this.SpriteTexture.Bounds;
            }

            this.spriteTextureWorldDimensions = new Vector2(this.World.GetWorldFromPixel(this.SpriteTextureBoundingBox.Value.Width), this.World.GetWorldFromPixel(this.SpriteTextureBoundingBox.Value.Height));

            uint[] pixelData = null;

            if (this.computeSpriteShape)
            {
                pixelData = new uint[this.SpriteTextureBoundingBox.Value.Width * this.SpriteTextureBoundingBox.Value.Height];
                this.SpriteTexture.GetData(0, this.SpriteTextureBoundingBox, pixelData, 0, pixelData.Length);
                this.spriteVertices = PolygonTools.CreatePolygon(pixelData, this.SpriteTextureBoundingBox.Value.Width);
                this.SpriteWorldVertices = this.spriteVertices;
                this.spriteShapes = Triangulate.ConvexPartition(this.SpriteWorldVertices, TriangulationAlgorithm.Bayazit);
                this.SpriteWorldShapes = this.spriteShapes;
            }

            this.OptionMapFlagsShader.LoadContent(contentManager);
            this.NormalMapShader.LoadContent(contentManager);

            if (this.spriteNormalTexturePath != null)
            {
                this.SpriteNormalTexture = contentManager.Load<Texture2D>(this.spriteNormalTexturePath);
            }
            else
            {
                this.SpriteNormalTexture = new Texture2D(contentManager.Game.GraphicsDevice, this.SpriteTextureBoundingBox.Value.Width, this.SpriteTextureBoundingBox.Value.Height);

                if (pixelData == null)
                {
                    pixelData = new uint[this.SpriteTextureBoundingBox.Value.Width * this.SpriteTextureBoundingBox.Value.Height];
                    this.SpriteTexture.GetData(0, this.SpriteTextureBoundingBox, pixelData, 0, pixelData.Length);
                }

                // Convert the image to a normal map with all vectors pointing UP (saving Alpha channel)
                for (int i = 0; i < pixelData.Length; i++)
                {
                    pixelData[i] = (255U << 24 & pixelData[i]) + (255U << 16) + (128U << 8) + 128U;
                }

                this.SpriteNormalTexture.SetData(pixelData);
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
            this.DrawColorMap(gameTime, renderSystem, this.SpriteTextureBoundingBox.Value);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its normal map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawNormalMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            this.DrawNormalMap(gameTime, renderSystem, this.SpriteTextureBoundingBox.Value);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawOptionsMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            this.DrawOptionsMap(gameTime, renderSystem, this.SpriteTextureBoundingBox.Value);
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
            renderSystem.DrawSprite(this.SpriteTexture, this.SpriteWorldBounds.Position, drawBoundingBox, this.RenderColor, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring);
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
            renderSystem.DrawSprite(this.SpriteNormalTexture, this.SpriteWorldBounds.Position, drawBoundingBox, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring);
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
            renderSystem.BeginRender(this.OptionMapFlagsShader);
            this.OptionMapFlagsShader.ConfigureShaderAndApplyPass(renderSystem, this);
            renderSystem.DrawSprite(this.SpriteTexture, this.SpriteWorldBounds.Position, drawBoundingBox, Color.White, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring);
            renderSystem.EndRender();
        }
        #endregion
    }
}
