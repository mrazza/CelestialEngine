// -----------------------------------------------------------------------
// <copyright file="SimulatedPostProcess.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.PostProcess
{
    using System.Collections.Generic;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Collections.QuadTree;
    using System;

    /// <summary>
    /// Base class and starting point for any post processing effect to be rendered and simulated in the world.
    /// </summary>
    public abstract class SimulatedPostProcess : SimBase, IPostProcess, IQuadTreeItem<SimulatedPostProcess, List<SimulatedPostProcess>>
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
        /// Initializes a new instance of the <see cref="SimulatedPostProcess"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="postProcessEffect">The post process effect.</param>
        public SimulatedPostProcess(World world, Shader postProcessEffect)
            : this(world, postProcessEffect, Vector2.Zero, Vector2.Zero, true, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedPostProcess"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="postProcessEffect">The post process effect.</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The starting velocity of the object.</param>
        /// <param name="isEnabled">if set to <c>true</c> the effect will be enabled.</param>
        /// <param name="renderPriority">The render priority.</param>
        public SimulatedPostProcess(World world, Shader postProcessEffect, Vector2 position, Vector2 velocity, bool isEnabled, float renderPriority)
            : base(world, position, velocity)
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

        /// <summary>
        /// Gets the world bounds.
        /// </summary>
        /// <value>
        /// The world bounds.
        /// </value>
        RectangleF IQuadTreeItem<SimulatedPostProcess, List<SimulatedPostProcess>>.WorldBounds
        {
            get
            {
                return this.GetWorldDrawBounds();
            }
        }

        /// <summary>
        /// Gets or sets the quad tree node.
        /// </summary>
        /// <value>
        /// The quad tree node.
        /// </value>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        QuadTreeNode<SimulatedPostProcess, List<SimulatedPostProcess>> IQuadTreeItem<SimulatedPostProcess, List<SimulatedPostProcess>>.QuadTreeNode
        {
            get;
            set;
        }
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

        #region Draw Functions
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        public virtual void LoadContent(ExtendedContentManager contentManager)
        {
            this.postProcessEffect.LoadContent(contentManager);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SimulatedPostProcess"/> component needs render itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public void Draw(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            if (this.GetWorldDrawBounds().Intersects(renderSystem.GetCameraRenderBounds()))
            {
                this.Render(gameTime, renderSystem);
            }
        }

        /// <summary>
        /// Gets a <see cref="RectangleF"/> containing information related to the dimensions of the bounding box in which the post process
        /// affects in world units. Used by the <see cref="DeferredRenderSystem"/> to determine whether the post process is drawn.
        /// </summary>
        /// <returns>A <see cref="RectangleF"/> containing the position and size of the area that may be affected by this post process (X, Y, Width, Height).</returns>
        public abstract RectangleF GetWorldDrawBounds();

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SimulatedPostProcess"/> component needs render itself.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        protected abstract void Render(GameTime gameTime, DeferredRenderSystem renderSystem);
        #endregion
    }
}
