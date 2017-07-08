// -----------------------------------------------------------------------
// <copyright file="InputState.cs" company="">
// Copyright (C) Will Graham & Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Contains a snapshot of the state of input devices.
    /// </summary>
    public class InputState
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InputState"/> class.
        /// </summary>
        internal InputState()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the elapsed game time for the current Update.
        /// </summary>
        public GameTime GameTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current state of the mouse.
        /// </summary>
        /// <value>
        /// The current state of the mouse.
        /// </value>
        public MouseState CurrentMouseState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last state of the mouse.
        /// </summary>
        /// <value>
        /// The last state of the mouse.
        /// </value>
        public MouseState LastMouseState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state of the current keyboard.
        /// </summary>
        /// <value>
        /// The state of the current keyboard.
        /// </value>
        public KeyboardState CurrentKeyboardState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last state of the keyboard.
        /// </summary>
        /// <value>
        /// The last state of the keyboard.
        /// </value>
        public KeyboardState LastKeyboardState
        {
            get;
            private set;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines whether the specified key is down (pressed).
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///   <c>true</c> if key is down; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKeyDown(Keys key)
        {
            return this.CurrentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Determines whether the specified key is down for the first sequential update
        /// (new key press).
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///   <c>true</c> if key is pressed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFirstKeyPress(Keys key)
        {
            return this.CurrentKeyboardState.IsKeyDown(key) && this.LastKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Determines whether the left mouse is down.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the left mouse is down; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLeftMouseDown()
        {
            return this.CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Determines whether the left mouse is clicked.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the left mouse is clicked; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLeftMouseClick()
        {
            return this.CurrentMouseState.LeftButton == ButtonState.Pressed && this.LastMouseState.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Determines whether the right mouse is down.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the right mouse is down; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightMouseDown()
        {
            return this.CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Determines whether the right mouse is clicked.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the right mouse is clicked; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightMouseClick()
        {
            return this.CurrentMouseState.RightButton == ButtonState.Pressed && this.LastMouseState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Determines whether the scroll wheel has moved.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the scroll wheel has moved; otherwise, <c>false</c>.
        /// </returns>
        public bool IsScrollWheelChanged()
        {
            return this.CurrentMouseState.ScrollWheelValue != this.LastMouseState.ScrollWheelValue;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Updates the state of input devices.
        /// </summary>
        internal void Update(GameTime gameTime)
        {
            this.GameTime = gameTime;

            this.LastKeyboardState = this.CurrentKeyboardState;
            this.CurrentKeyboardState = Keyboard.GetState();

            this.LastMouseState = this.CurrentMouseState;
            this.CurrentMouseState = Mouse.GetState();
        }
        #endregion
    }
}
