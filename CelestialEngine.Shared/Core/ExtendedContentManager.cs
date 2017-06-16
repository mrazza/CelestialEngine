// -----------------------------------------------------------------------
// <copyright file="ExtendedContentManager.cs" company="">
// Copyright (C) Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Provides extended functionality on top of the default <see cref="ContentManager"/> implementation.
    /// </summary>
    public class ExtendedContentManager : ContentManager
    {
        #region Private Members
        /// <summary>
        /// The game that owns this instance.
        /// </summary>
        private BaseGame game;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedContentManager" /> class.
        /// </summary>
        /// <param name="game">The game that owns this instance.</param>
        public ExtendedContentManager(BaseGame game)
            : base(game.Services)
        {
            this.game = game;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedContentManager" /> class.
        /// </summary>
        /// <param name="game">The game that owns this instance.</param>
        /// <param name="rootDirectory">The root directory to search for content.</param>
        public ExtendedContentManager(BaseGame game, string rootDirectory)
            : base(game.Services, rootDirectory)
        {
            this.game = game;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the game that owns this ContentManager.
        /// </summary>
        /// <value>
        /// The game that owns this ContentManager.
        /// </value>
        public BaseGame Game
        {
            get
            {
                return this.game;
            }
        }
        #endregion
    }
}
