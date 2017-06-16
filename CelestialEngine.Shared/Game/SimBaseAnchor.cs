// -----------------------------------------------------------------------
// <copyright file="SimBaseAnchor.cs" company="">
// Copyright (C) 2012 Matthew Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Forces a <see cref="SimBase"/> to follow another <see cref="SimBase"/> object.
    /// </summary>
    public class SimBaseAnchor : SimBase
    {
        #region Members
        /// <summary>
        /// The master <see cref="SimBase"/> object this <see cref="SimBase"/> follows.
        /// </summary>
        private SimBase masterObject;

        /// <summary>
        /// The offset from the attached object's position.
        /// </summary>
        private Vector2 attachmentPositionalOffset;

        /// <summary>
        /// The offset from the attached object's rotation.
        /// </summary>
        private float attachmentRotationalOffset;

        /// <summary>
        /// The <see cref="SimBase"/> to be attached.
        /// </summary>
        private SimBase slaveObject;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimBaseAnchor"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="masterObject">The <see cref="SimBase"/> object we are attached to.</param>
        /// <param name="slaveObject">The slave object.</param>
        public SimBaseAnchor(World world, SimBase masterObject, SimBase slaveObject)
            : this(world, masterObject, slaveObject, Vector2.Zero, 0.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimBaseAnchor"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="masterObject">The <see cref="SimBase"/> object we are attached to.</param>
        /// <param name="slaveObject">The slave object.</param>
        /// <param name="attachmentPositionalOffset">The attachment offset for the position.</param>
        /// <param name="attachmentRotationalOffset">The attachment offset for the rotation.</param>
        public SimBaseAnchor(World world, SimBase masterObject, SimBase slaveObject, Vector2 attachmentPositionalOffset, float attachmentRotationalOffset)
            : base(world)
        {
            this.masterObject = masterObject;
            this.slaveObject = slaveObject;
            this.attachmentPositionalOffset = attachmentPositionalOffset;
            this.attachmentRotationalOffset = attachmentRotationalOffset;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the attachment positional offset.
        /// </summary>
        /// <value>
        /// The attachment positional offset.
        /// </value>
        public Vector2 AttachmentPositionalOffset
        {
            get
            {
                return this.attachmentPositionalOffset;
            }

            set
            {
                this.attachmentPositionalOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the attachment rotational offset.
        /// </summary>
        /// <value>
        /// The attachment rotational offset.
        /// </value>
        public float AttachmentRotationalOffset
        {
            get
            {
                return this.attachmentRotationalOffset;
            }

            set
            {
                this.attachmentRotationalOffset = value;
            }
        }
        #endregion

        #region SimBase Overrides
        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public override void Update(GameTime gameTime)
        {
            this.slaveObject.Position = this.masterObject.Position + this.attachmentPositionalOffset;
            this.slaveObject.Rotation = this.masterObject.Rotation + this.attachmentRotationalOffset;
        }
        #endregion
    }
}
