// -----------------------------------------------------------------------
// <copyright file="StaticPostProcess.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.PostProcess
{
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Base class and starting point for any static fullscreen post processing effect to be rendered.
    /// </summary>
    public abstract class StaticPostProcess : IPostProcess
    {
        #region Members
        /// <summary>
        /// Effect to render as a post process
        /// </summary>
        private Shader postProcessEffect;

        /// <summary>
        /// Determines whether or not this post process is enabled
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Render priority
        /// </summary>
        private float renderPriority;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticPostProcess"/> class.
        /// </summary>
        /// <param name="postProcessEffect">The post process effect.</param>
        public StaticPostProcess(Shader postProcessEffect)
            : this(postProcessEffect, true, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticPostProcess"/> class.
        /// </summary>
        /// <param name="postProcessEffect">The post process effect.</param>
        /// <param name="isEnabled">if set to <c>true</c> the effect will be enabled.</param>
        /// <param name="renderPriority">The render priority.</param>
        public StaticPostProcess(Shader postProcessEffect, bool isEnabled, float renderPriority)
        {
            this.postProcessEffect = postProcessEffect;
            this.isEnabled = isEnabled;
            this.renderPriority = renderPriority;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.isEnabled = value;
            }
        }

        /// <summary>
        /// Gets the render priority.
        /// </summary>
        /// <value>
        /// The render priority.
        /// </value>
        public float RenderPriority
        {
            get
            {
                return this.renderPriority;
            }
        }

        /// <summary>
        /// Gets the post process effect.
        /// </summary>
        protected Shader PostProcessEffect
        {
            get
            {
                return this.postProcessEffect;
            }
        }
        #endregion

        #region Draw Functions
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        public void LoadContent(ExtendedContentManager contentManager)
        {
            this.postProcessEffect.LoadContent(contentManager);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="StaticPostProcess"/> component needs render itself.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public abstract void Draw(GameTime gameTime, DeferredRenderSystem renderSystem);
        #endregion

        #region IComparable Implementation
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(IPostProcess other)
        {
            return PostProcessComparer.CompareIPostProcess(this, other);
        }
        #endregion
    }
}
