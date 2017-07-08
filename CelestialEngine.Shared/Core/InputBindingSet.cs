// -----------------------------------------------------------------------
// <copyright file="InputBindingSet.cs" company="">
// Copyright (C) Will Graham & Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Collections;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides for registration and invocation of a set of callbacks on input state (selective push).
    /// </summary>
    public class InputBindingSet
    {
        /// <summary>
        /// A list of conditional bindings that contains input state responses of game objects.
        /// </summary>
        private ThrottledCollection<List<ConditionalInputBinding>, ConditionalInputBinding> inputBindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBindingSet"/> class.
        /// </summary>
        public InputBindingSet()
        {
            this.inputBindings = new ThrottledCollection<List<ConditionalInputBinding>, ConditionalInputBinding>(new List<ConditionalInputBinding>());
        }

        /// <summary>
        /// Adds a binding to the input manager.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>Returns the conditional binding instance.</returns>
        public ConditionalInputBinding AddBinding(Action<InputState> action)
        {
            ConditionalInputBinding binding = new ConditionalInputBinding(this, (s) => { return true; }, action);
            this.inputBindings.Add(binding);
            return binding;
        }

        /// <summary>
        /// Adds a conditional binding to the input manager.
        /// </summary>
        /// <param name="condition">The condition upon which the action will be invoked.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>Returns the conditional binding instance.</returns>
        public ConditionalInputBinding AddConditionalBinding(Func<InputState, bool> condition, Action<InputState> action)
        {
            ConditionalInputBinding binding = new ConditionalInputBinding(this, condition, action);
            this.inputBindings.Add(binding);
            return binding;
        }

        /// <summary>
        /// Processes the pending updates against the input binding collection and executes
        /// callbacks based on the provided input state.
        /// </summary>
        internal void ExecuteAgainst(InputState inputState)
        {
            this.inputBindings.ProcessUpdates();

            foreach (var binding in this.inputBindings)
            {
                if (binding.Condition(inputState))
                {
                    binding.Action(inputState);
                }
            }
        }

        /// <summary>
        /// Removes a conditional binding from the set.
        /// </summary>
        /// <param name="binding">The binding instance to remove.</param>
        internal void Remove(ConditionalInputBinding binding)
        {
            this.inputBindings.Remove(binding);
        }
    }
}
