// -----------------------------------------------------------------------
// <copyright file="InputManager.cs" company="">
// Copyright (C) Will Graham & Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;
    using System.Collections.Generic;
    using CelestialEngine.Core.Collections;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Maintains the current state of input devices and the list of registered input binding
    /// sets.
    /// </summary>
    public class InputManager : GameComponent
    {
        #region Private Members
        /// <summary>
        /// The state of input devices.
        /// </summary>
        private InputState inputState;

        /// <summary>
        /// The global input binding set.
        /// </summary>
        private InputBindingSet globalInputBindingSet;

        /// <summary>
        /// A list of child input binding sets.
        /// </summary>
        private ThrottledCollection<List<InputBindingSet>, InputBindingSet> childInputBindingSets;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InputManager"/> class.
        /// </summary>
        /// <param name="parentGame">The parent game.</param>
        public InputManager(BaseGame parentGame)
            : base(parentGame)
        {
            this.inputState = new InputState();
            this.globalInputBindingSet = new InputBindingSet();
            this.childInputBindingSets = new ThrottledCollection<List<InputBindingSet>, InputBindingSet>(new List<InputBindingSet>());
            this.childInputBindingSets.Add(globalInputBindingSet);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a binding to the input manager.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>Returns the conditional binding instance.</returns>
        public ConditionalInputBinding AddBinding(Action<InputState> action)
        {
            return this.globalInputBindingSet.AddBinding(action);
        }

        /// <summary>
        /// Adds a conditional binding to the input manager.
        /// </summary>
        /// <param name="condition">The condition upon which the action will be invoked.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>Returns the conditional binding instance.</returns>
        public ConditionalInputBinding AddConditionalBinding(Func<InputState, bool> condition, Action<InputState> action)
        {
            return this.globalInputBindingSet.AddConditionalBinding(condition, action);
        }

        /// <summary>
        /// Adds a set of input bindings to the input manager.
        /// </summary>
        /// <param name="inputBindingSet">The set of input bindings to add to the manager.</param>
        public void AddInputBindingSet(InputBindingSet inputBindingSet)
        {
            this.childInputBindingSets.Add(inputBindingSet);
        }

        /// <summary>
        /// Removes a set of input bindings from the input manager.
        /// </summary>
        /// <param name="inputBindingSet">The set of input bindings to remove from the manager.</param>
        public void RemoveInputBindingSet(InputBindingSet inputBindingSet)
        {
            this.childInputBindingSets.Remove(inputBindingSet);
        }

        #region GameComponent Overrides
        /// <summary>
        /// Called when the GameComponent needs to be updated with the current input device state.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            this.childInputBindingSets.ProcessUpdates();
            this.inputState.Update();

            foreach (var set in this.childInputBindingSets)
            {
                set.ExecuteAgainst(this.inputState);
            }

            base.Update(gameTime);
        }
        #endregion
        #endregion
    }
}
