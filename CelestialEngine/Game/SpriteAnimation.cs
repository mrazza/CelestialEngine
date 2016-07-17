// -----------------------------------------------------------------------
// <copyright file="Animation.cs" company="">
// Copyright (C) 2012 Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A single animation within an <see cref="AnimatedSprite"/>.
    /// </summary>
    public class SpriteAnimation
    {
        /// <summary>
        /// The bounds of the animation sheet relative to the sprite texture bounding box.
        /// </summary>
        private Rectangle animationSheet;

        /// <summary>
        /// The width of a single frame in the animation.
        /// </summary>
        private int frameWidth;

        /// <summary>
        /// The number of frames per second at which the sprite should be animated.
        /// </summary>
        private float framesPerSecond;

        /// <summary>
        /// A modifier applied to affect the speed at which the sprite is animated.
        /// </summary>
        private float framesPerSecondModifier;

        /// <summary>
        /// The current time elapsed since the frame was changed.
        /// </summary>
        private double currentElapsedTime;

        /// <summary>
        /// The current frame number within the animation.
        /// </summary>
        private int currentFrameNumber;

        /// <summary>
        /// The number of frames in the animation.
        /// </summary>
        private int numberOfFrames;

        /// <summary>
        /// Specifies whether this animation should be looped.
        /// </summary>
        private bool isLooping;

        /// <summary>
        /// Specifies whether this animation is stopped.
        /// </summary>
        private bool isStopped;

        private Action onAnimationComplete; //HACK: Implement this properly

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimation"/> class.
        /// </summary>
        /// <param name="animationSheet">The bounds animation sheet relative to the sprite texture bounding box.</param>
        /// <param name="numberOfFrames">The number of frames in the animation.</param>
        /// <param name="framesPerSecond">The default frames per second of the animation.</param>
        public SpriteAnimation(Rectangle animationSheet, int numberOfFrames, float framesPerSecond)
        {
            this.animationSheet = animationSheet;
            this.numberOfFrames = numberOfFrames;
            this.framesPerSecond = framesPerSecond;
            this.framesPerSecondModifier = 1.0f;
            this.frameWidth = animationSheet.Width / numberOfFrames; //This should *always* return a whole number...
        }

        /// <summary>
        /// Gets or sets the modifier applied to affect the speed at which the sprite is animated.
        /// </summary>
        /// <value>
        /// The frames per second modifier.
        /// </value>
        public float FramesPerSecondModifier
        {
            get
            {
                return this.framesPerSecondModifier;
            }

            set
            {
                this.framesPerSecondModifier = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation should loop.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the animation should loop; otherwise, <c>false</c>.
        /// </value>
        public bool IsLooping
        {
            get
            {
                return this.isLooping;
            }

            set
            {
                this.isLooping = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the animation is currently stopped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the animation is stopped; otherwise, <c>false</c>.
        /// </value>
        public bool IsStopped
        {
            get
            {
                return this.isStopped;
            }
        }

        // HACK: Implement this properly
        public Action OnAnimationComplete
        {
            get
            {
                return this.onAnimationComplete;
            }

            set
            {
                this.onAnimationComplete = value;
            }
        }

        /// <summary>
        /// Gets or sets the current frame number.
        /// </summary>
        /// <value>
        /// The current frame number.
        /// </value>
        public int CurrentFrameNumber
        {
            get
            {
                return this.currentFrameNumber;
            }

            set
            {
                this.currentFrameNumber = (int)MathHelper.Clamp(value, 0, this.numberOfFrames - 1);
            }
        }

        /// <summary>
        /// Resets the animation state.
        /// </summary>
        internal void Reset()
        {
            this.currentElapsedTime = 0.0f;
            this.currentFrameNumber = 0;
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        internal void Play()
        {
            this.Reset();
            this.isStopped = false;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        internal void Stop()
        {
            this.isStopped = true;
        }

        /// <summary>
        /// Gets the bounding box for the current frame within the sprite texture bounding box.
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetCurrentFrameBounds()
        {
            return new Rectangle(this.animationSheet.Left + (this.currentFrameNumber * this.frameWidth), this.animationSheet.Top, this.frameWidth, this.animationSheet.Height);
        }

        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        internal void Update(GameTime gameTime)
        {
            // Only update if we are stopped
            if (!this.isStopped)
            {
                // Increment the elapsed time for the current frame.
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                this.currentElapsedTime += elapsedTime;

                // Calculate the duration that each frame should be shown.
                float secondsPerFrame = 1000.0f / (this.framesPerSecond * this.framesPerSecondModifier);
                if (this.currentElapsedTime > secondsPerFrame)
                {
                    // Calculate the number of frames to increment
                    int framesToIncrement = (int)Math.Floor(this.currentElapsedTime / secondsPerFrame);

                    // Update the elapsed time based on the number of frames we are incrementing
                    this.currentElapsedTime -= secondsPerFrame * framesToIncrement; //Keep any remainder in time 'supposed' to be spent

                    // Jump to the correct frame if we have reached the end of the animation
                    if (this.currentFrameNumber + framesToIncrement >= this.numberOfFrames)
                    {
                        if (this.isLooping)
                        {
                            // Account for the fact that update may be *very* behind.
                            this.currentFrameNumber += framesToIncrement;
                            this.currentFrameNumber %= this.numberOfFrames;
                        }
                        else
                        {
                            // Force to last frame of the animation
                            this.currentFrameNumber = this.numberOfFrames - 1;
                            this.isStopped = true;
                            // HACK: Implement this properly
                            this.OnAnimationComplete();
                        }
                    }

                    // Increment the current frame number
                    this.currentFrameNumber += framesToIncrement;
                }
            }
        }
    }
}
