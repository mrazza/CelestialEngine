// -----------------------------------------------------------------------
// <copyright file="IComponent.cs" company="">
//  Copyright (C) 2011-2013 Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Interface that describes and is implemented by all components.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Raised when the UpdateOrder property changes.
        /// </summary>
        event EventHandler<EventArgs> UpdateOrderChanged;

        /// <summary>
        /// Indicates the order in which the IComponent should be updated relative to other IComponent instances. Lower values are updated first.
        /// </summary>
        /// <value>
        /// The order in which the IComponent should be updated.
        /// </value>
        int UpdateOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether IComponent.Update should be called when Game.Update is called.
        /// </summary>
        /// <value>
        ///   <c>true</c> if IComponent.Update should be called; <c>false</c> otherwise.
        /// </value>
        bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Called when the IComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        void Update(GameTime gameTime);
    }
}
