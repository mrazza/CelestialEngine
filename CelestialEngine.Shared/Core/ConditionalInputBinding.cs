// -----------------------------------------------------------------------
// <copyright file="ConditionalInputBinding.cs" company="">
// Copyright (C) Will Graham & Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;

    /// <summary>
    /// Represents a binding to input state that is invoked only upon satisfaction of the given condition.
    /// </summary>
    public class ConditionalInputBinding : IDisposable
    {
        #region Private Members
        /// <summary>
        /// The Input Binding Set that owns this binding.
        /// </summary>
        private InputBindingSet inputBindingSet;

        /// <summary>
        /// The condition upon which the action will be invoked.
        /// </summary>
        private Func<InputState, bool> condition;

        /// <summary>
        /// The action to invoke.
        /// </summary>
        private Action<InputState> action;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalInputBinding"/> class.
        /// </summary>
        /// <param name="condition">The condition upon which the action will be invoked.</param>
        /// <param name="action">The action to invoke.</param>
        internal ConditionalInputBinding(InputBindingSet inputBindingSet, Func<InputState, bool> condition, Action<InputState> action)
        {
            this.inputBindingSet = inputBindingSet;
            this.condition = condition;
            this.action = action;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the condition upon which the action will be invoked.
        /// </summary>
        public Func<InputState, bool> Condition
        {
            get
            {
                return this.condition;
            }
        }

        /// <summary>
        /// Gets the action to invoke.
        /// </summary>
        public Action<InputState> Action
        {
            get
            {
                return this.action;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.inputBindingSet.Remove(this);
        }
        #endregion
    }
}
