// -----------------------------------------------------------------------
// <copyright file="ScreenDrawableComponent.cs" company="">
// Copyright (C) 2011-2013 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Represents a drawable component rendered on top of the screen (not world space)
    /// </summary>
    /// <seealso cref="CelestialEngine.Core.IComponent" />
    /// <seealso cref="System.IComparable{CelestialEngine.Core.ScreenDrawableComponent}" />
    /// <seealso cref="System.IDisposable" />
    public abstract class ScreenDrawableComponent : IComponent, IComparable<ScreenDrawableComponent>, IDisposable
    {
        /// <summary>
        /// The world in which this ScreenDrawableComponent lives.
        /// </summary>
        private World world;

        /// <summary>
        /// The order in which this drawable component is rendered (relative to other components).
        /// </summary>
        private int renderOrder;

        /// <summary>
        /// The update order of the ScreenDrawableComponent.
        /// </summary>
        private int updateOrder;

        /// <summary>
        /// Indicates whether IComponent.Update should be called when Game.Update is called.
        /// </summary>
        private bool enabled;

        /// <summary>
        /// Raised when the UpdateOrder property changes.
        /// </summary>
        public event EventHandler<EventArgs> UpdateOrderChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenDrawableComponent"/> class.
        /// </summary>
        /// <param name="world">The world instance.</param>
        public ScreenDrawableComponent(World world)
        {
            this.world = world;
            this.world.AddScreenDrawableComponent(this);
            this.enabled = true;
        }

        /// <summary>
        /// Indicates the order in which the IComponent should be updated relative to other IComponent instances. Lower values are updated first.
        /// </summary>
        /// <value>
        /// The order in which the IComponent should be updated.
        /// </value>
        public int UpdateOrder
        {
            get
            {
                return this.updateOrder;
            }

            set
            {
                if (this.updateOrder != value)
                {
                    this.updateOrder = value;

                    // HACK: This is not a long-term solution, RemoveSimObject always returns true
                    if (this.World.RemoveScreenDrawableComponent(this))
                    {
                        this.World.AddScreenDrawableComponent(this);
                    }

                    if (this.UpdateOrderChanged != null)
                    {
                        this.UpdateOrderChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether IComponent.Update should be called when Game.Update is called.
        /// </summary>
        /// <value>
        /// <c>true</c> if IComponent.Update should be called; <c>false</c> otherwise.
        /// </value>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the order in which this drawable component is rendered.
        /// </summary>
        /// <value>
        /// The order in which this drawable component is rendered.
        /// </value>
        public int RenderOrder
        {
            get
            {
                return this.renderOrder;
            }

            set
            {
                if (this.renderOrder != value)
                {
                    this.renderOrder = value;

                    // HACK: This is not a long-term solution, RemoveScreenDrawableComponent always returns true
                    if (this.World.RemoveScreenDrawableComponent(this))
                    {
                        this.World.AddScreenDrawableComponent(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the world instance.
        /// </summary>
        /// <value>
        /// The world instance.
        /// </value>
        protected World World
        {
            get
            {
                return this.world;
            }
        }

        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        public abstract void LoadContent(ExtendedContentManager contentManager);

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="ScreenDrawableComponent"/> 
        /// component needs to draw.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="spriteBatch"><see cref="ScreenSpriteBatch"/> to render with.</param>
        public abstract void Draw(GameTime gameTime, ScreenSpriteBatch spriteBatch);

        /// <summary>
        /// Called when the ScreenDrawableComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public abstract void Update(GameTime gameTime);

        #region IComparable Methods
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(ScreenDrawableComponent other)
        {
            var orderDifference = this.RenderOrder - other.RenderOrder;

            if (orderDifference == 0)
            {
                // Are these actually the same instance?
                if (this == other)
                {
                    return 0; // Yes; let them be equal!
                }
                else
                {
                    // They're different, compare update order
                    int updateDifference = this.UpdateOrder - other.UpdateOrder;

                    if (updateDifference == 0)
                    {
                        return this.GetHashCode() - other.GetHashCode(); // Update order is the same, find a way to compare them
                    }
                    else if (updateDifference < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else if (orderDifference < 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.World.RemoveScreenDrawableComponent(this);
        }
        #endregion
    }
}
