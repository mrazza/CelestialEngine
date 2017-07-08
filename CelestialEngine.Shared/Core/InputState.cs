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
        #region Private Members
        /// <summary>
        /// The elapsed game time for the current Update.
        /// </summary>
        private GameTime gameTime;

        /// <summary>
        /// The state of the keyboard as of the previous update.
        /// </summary>
        private KeyboardState lastKeyboardState;

        /// <summary>
        /// The state of the keyboard as of the current update.
        /// </summary>
        private KeyboardState currentKeyboardState;

        /// <summary>
        /// The state of the mouse as of the previous update.
        /// </summary>
        private MouseState lastMouseState;

        /// <summary>
        /// The state of the mouse as of the current update.
        /// </summary>
        private MouseState currentMouseState;
        #endregion

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
        /// Gets the current state of the mouse.
        /// </summary>
        /// <value>
        /// The current state of the mouse.
        /// </value>
        public MouseState CurrentMouseState
        {
            get
            {
                return this.currentMouseState;
            }
        }

        /// <summary>
        /// Gets the last state of the mouse.
        /// </summary>
        /// <value>
        /// The last state of the mouse.
        /// </value>
        public MouseState LastMouseState
        {
            get
            {
                return this.lastMouseState;
            }
        }

        /// <summary>
        /// Gets the state of the current keyboard.
        /// </summary>
        /// <value>
        /// The state of the current keyboard.
        /// </value>
        public KeyboardState CurrentKeyboardState
        {
            get
            {
                return this.currentKeyboardState;
            }
        }

        /// <summary>
        /// Gets the last state of the keyboard.
        /// </summary>
        /// <value>
        /// The last state of the keyboard.
        /// </value>
        public KeyboardState LastKeyboardState
        {
            get
            {
                return this.lastKeyboardState;
            }
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
            return this.currentKeyboardState.IsKeyDown(key);
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
            return this.currentKeyboardState.IsKeyDown(key) && this.lastKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Determines whether the left mouse is down.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the left mouse is down; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLeftMouseDown()
        {
            return this.currentMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Determines whether the left mouse is clicked.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the left mouse is clicked; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLeftMouseClick()
        {
            return this.currentMouseState.LeftButton == ButtonState.Pressed && this.lastMouseState.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Determines whether the right mouse is down.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the right mouse is down; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightMouseDown()
        {
            return this.currentMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Determines whether the right mouse is clicked.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the right mouse is clicked; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightMouseClick()
        {
            return this.currentMouseState.RightButton == ButtonState.Pressed && this.lastMouseState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Determines whether the scroll wheel has moved.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the scroll wheel has moved; otherwise, <c>false</c>.
        /// </returns>
        public bool IsScrollWheelChanged()
        {
            return this.currentMouseState.ScrollWheelValue != this.lastMouseState.ScrollWheelValue;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Updates the state of input devices.
        /// </summary>
        internal void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;

            this.lastKeyboardState = this.currentKeyboardState;
            this.currentKeyboardState = Keyboard.GetState();

            this.lastMouseState = this.currentMouseState;
            this.currentMouseState = Mouse.GetState();
        }
        #endregion
    }
}
