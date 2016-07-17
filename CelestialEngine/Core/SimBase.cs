// -----------------------------------------------------------------------
// <copyright file="SimBase.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Base class for any object that requires simulation within the world
    /// but will not be drawn.
    /// </summary>
    /// <seealso cref="SpriteBase"/>
    public abstract class SimBase : IComponent, IComparable<SimBase>, IDisposable
    {
        #region Members
        /// <summary>
        /// The body representing this sim base object.
        /// </summary>
        private Body body;

        /// <summary>
        /// The update order of the SimBase.
        /// </summary>
        private int updateOrder;

        /// <summary>
        /// Indicates whether IComponent.Update should be called when Game.Update is called.
        /// </summary>
        private bool enabled;

        /// <summary>
        /// The world in which this SimBase lives.
        /// </summary>
        private World world;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimBase" /> class.
        /// </summary>
        /// <param name="world">The world in which the SimBase lives.</param>
        public SimBase(World world)
            : this(world, Vector2.Zero, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimBase"/> class.
        /// </summary>
        /// <param name="world">The world in which the SimBase lives.</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The starting velocity of the object.</param>
        public SimBase(World world, Vector2 position, Vector2 velocity)
        {
            this.enabled = true;
            this.world = world;
            this.body = new Body(world.PhysicsWorld);
            this.body.BodyType = BodyType.Kinematic;
            this.Position = position;
            this.Velocity = velocity;
            this.world.AddSimObject(this);
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when the UpdateOrder property changes.
        /// </summary>
        public event EventHandler<EventArgs> UpdateOrderChanged;
        #endregion

        #region Properties
        #region Positional
        /// <summary>
        /// Gets or sets the <see cref="Vector2"/> calculated position of the object in absolute 2D world space.
        /// </summary>
        /// <value>
        /// The position of the object in absolute 2D world space.
        /// </value>
        public Vector2 Position
        {
            get
            {
                return this.body.Position;
            }

            set
            {
                this.body.Position = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Vector2"/> calculated position of the object in absolute 2D pixel space.
        /// </summary>
        /// <value>
        /// The position of the object in absolute 2D pixel space.
        /// </value>
        public Vector2 PixelPosition
        {
            get
            {
                return this.World.GetPixelFromWorld(this.Position);
            }
        }

        /// <summary>
        /// Gets or sets the relative velocity (relative to the object's rotation) of the object in object space units/sec.
        /// </summary>
        /// <value>
        /// The relative velocity of the object in units/sec.
        /// </value>
        public Vector2 RelativeVelocity
        {
            get
            {
                float yValue = Vector2.Dot(Vector2.Normalize(this.Velocity), this.RotationVector);
                return new Vector2((float)Math.Sqrt(1 - yValue) * this.Velocity.Length(), yValue * this.Velocity.Length());
            }

            set
            {
                float yValue = Vector2.Dot(Vector2.Normalize(value), new Vector2(-1.0f, 0.0f));
                this.Velocity = new Vector2((float)Math.Sqrt(1 - yValue) * value.Length(), yValue * value.Length());
            }
        }

        /// <summary>
        /// Gets or sets the velocity of the object in world space units/sec.
        /// (delta position over time)
        /// </summary>
        /// <value>
        /// The velocity of the object in units/sec.
        /// </value>
        public Vector2 Velocity
        {
            get
            {
                return this.body.LinearVelocity;
            }

            set
            {
                this.body.LinearVelocity = value;
            }
        }
        #endregion

        #region Rotational
        /// <summary>
        /// Gets or sets the rotation of the object (in radians) clockwise.
        /// </summary>
        /// <value>
        /// The rotation of the object in radians.
        /// </value>
        public float Rotation
        {
            get
            {
                return this.body.Rotation;
            }

            set
            {
                this.body.Rotation = value;
                //this.NormalizeRotation(); // Make sure we're still within acceptable bounds
            }
        }

        /// <summary>
        /// Gets or sets the rotation as a normalized vector.
        /// </summary>
        /// <value>
        /// The rotation as a vector.
        /// </value>
        public Vector2 RotationVector
        {
            get
            {
                return Vector2Helper.VectorFromRotation(this.Rotation);
            }

            set
            {
                this.Rotation = value.AngleBetween(Vector2Helper.Up);
            }
        }

        /// <summary>
        /// Gets or sets the angular velocity of the object (in radians/sec).
        /// </summary>
        /// <value>
        /// The angular velocity of the object in radians/sec.
        /// </value>
        public float AngularVelocity
        {
            get
            {
                return this.body.AngularVelocity;
            }

            set
            {
                this.body.AngularVelocity = value;
            }
        }
        #endregion

        /// <summary>
        /// The Farseer Physics body instance for this object.
        /// </summary>
        public Body Body
        {
            get
            {
                return this.body;
            }
        }

        /// <summary>
        /// Gets the World associated with this instance.
        /// </summary>
        public World World
        {
            get
            {
                return this.world;
            }
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
                    if (this.World.RemoveSimObject(this))
                    {
                        this.World.AddSimObject(this);
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
        #endregion

        #region IComparable Implementation
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(SimBase other)
        {
            float updateDifference = this.UpdateOrder - other.UpdateOrder;

            if (updateDifference == 0)
            {
                // Are these actually the same instance?
                if (this == other)
                {
                    return 0; // Yes; let them be equal!
                }
                else
                {
                    return this.GetHashCode() - other.GetHashCode(); // Update order is the same, find a way to compare them
                }
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
        #endregion

        #region Positional Methods
        /// <summary>
        /// Transforms the object by the specified transformation via absolute, 2D world space.
        /// </summary>
        /// <param name="transformation">The transformation to perform.</param>
        public void Transform(Vector2 transformation)
        {
            this.Position += transformation;
        }

        /// <summary>
        /// Transforms the object by the specified transformation relative to its current rotation.
        /// </summary>
        /// <param name="transformation">The relative transformation to perform.</param>
        public void RelativeTransform(Vector2 transformation)
        {
            float yValue = Vector2.Dot(Vector2.Normalize(transformation), new Vector2(-1.0f, 0.0f));
            this.Position += new Vector2((float)Math.Sqrt(1 - yValue) * transformation.Length(), yValue * transformation.Length());
        }
        #endregion

        #region Rotational Methods
        /// <summary>
        /// Rotates the object relative to its existing rotation (in radians).
        /// </summary>
        /// <param name="rotation">The rotation to perform (in radians).</param>
        public void Rotate(float rotation)
        {
            this.Rotation += rotation;
        }
        #endregion

        #region Simulation Methods
        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Normalizes the rotation values so they do not fall outside the bounds [0, 2Pi].
        /// </summary>
        protected void NormalizeRotation()
        {
            this.Rotation %= MathHelper.TwoPi;
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.body.Dispose();
            this.world.RemoveSimObject(this);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
