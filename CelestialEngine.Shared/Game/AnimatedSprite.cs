// -----------------------------------------------------------------------
// <copyright file="AnimatedSprite.cs" company="TaskyMedia LLC">
// Copyright (C) 2012 Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnimatedSprite : SimpleSprite
    {
        private SpriteAnimation currentAnimation;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        public AnimatedSprite(World world, string spriteTexturePath, string spriteNormalTexturePath)
            : this(world, spriteTexturePath, spriteNormalTexturePath, null, Vector2.Zero, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="spriteTextureBoundingBox">The bounding box of the sprite within the texture (used with sprite sheets).</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The starting velocity of the object.</param>
        public AnimatedSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Rectangle? spriteTextureBoundingBox, Vector2 position, Vector2 velocity)
            : base(world, spriteTexturePath, spriteNormalTexturePath, spriteTextureBoundingBox, position, velocity, false)
        {
        }

        /// <summary>
        /// Plays the specified animation.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        public void PlayAnimation(SpriteAnimation animation)
        {
            this.currentAnimation = animation;
            this.currentAnimation.Play();
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to be drawn.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            if (this.currentAnimation != null && !this.currentAnimation.IsStopped)
            {
                base.DrawColorMap(gameTime, renderSystem, this.currentAnimation.GetCurrentFrameBounds());
            }
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its normal map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawNormalMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            if (this.currentAnimation != null && !this.currentAnimation.IsStopped)
            {
                base.DrawNormalMap(gameTime, renderSystem, this.currentAnimation.GetCurrentFrameBounds());
            }
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawOptionsMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            if (this.currentAnimation != null && !this.currentAnimation.IsStopped)
            {
                base.DrawOptionsMap(gameTime, renderSystem, this.currentAnimation.GetCurrentFrameBounds());
            }
        }

        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public override void Update(GameTime gameTime)
        {
            if (this.currentAnimation != null)
            {
                this.currentAnimation.Update(gameTime);
            }

            base.Update(gameTime);
        }
    }
}
