// -----------------------------------------------------------------------
// <copyright file="ParallaxTiledSprite.cs" company="">
// Copyright (C) 2019 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A <see cref="TiledSprite"/> whose perspective moves along with the relative position of
    /// another <see cref="SimBase"/> object.
    /// </summary>
    public class ParallaxTiledSprite : TiledSprite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="tileArea">Area (width/height) of the world to display the tiled sprite.</param>
        /// <param name="relativeObject">The <see cref="SimBase"/> object to track the parallax effect to.</param>
        public ParallaxTiledSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Vector2 position, Vector2 tileArea, SimBase relativeObject)
            : this(world, spriteTexturePath, spriteNormalTexturePath, null, position, Vector2.Zero, tileArea, relativeObject)
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
        /// <param name="relativeObject">The <see cref="SimBase"/> object to track the parallax effect to.</param>
        public ParallaxTiledSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Rectangle? spriteTextureBoundingBox, Vector2 position, Vector2 velocity, Vector2 tileArea, SimBase relativeObject)
            : base(world, spriteTexturePath, spriteNormalTexturePath, spriteTextureBoundingBox, position, velocity, tileArea)
        {
            this.RelativeObject = relativeObject;
            this.ParallaxRatio = Vector2.One;
        }

        /// <summary>
        /// Gets the <see cref="SimBase"/> object whose position our parallax effect is relative to.
        /// </summary>
        public SimBase RelativeObject { get; }

        /// <summary>
        /// Gets or sets the ratio (in X and Y) in perspective shift given position changes of the <see cref="RelativeObject"/>.
        /// </summary>
        /// <remarks>
        /// A <see cref="ParallaxRatio"/> of (0, 0) results in no parallax effect at all.
        /// A <see cref="ParallaxRatio"/> of (0.5, 0.5) results in the tiled sprite moving slower than the <see cref="RelativeObject"/>.
        /// A <see cref="ParallaxRatio"/> of (1, 1) results in the tiled sprite following the movement of the <see cref="RelativeObject"/>.
        /// A <see cref="ParallaxRatio"/> larger than (1, 1) results in the tiled sprite moving faster than the <see cref="RelativeObject"/>.
        /// A <see cref="ParallaxRatio"/> smaller than (0, 0) results in the tiled sprite moving in the opposite direction of the <see cref="RelativeObject"/>.
        /// </remarks>
        public Vector2 ParallaxRatio
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.TilingOffset = (this.Position - this.RelativeObject.Position) * this.ParallaxRatio;
        }
    }
}
