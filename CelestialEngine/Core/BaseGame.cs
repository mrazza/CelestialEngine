// -----------------------------------------------------------------------
// <copyright file="BaseGame.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using CelestialEngine.Game;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents the core of the Celestial Engine; it provides
    /// basic graphics device initialization and management, game
    /// logic support, and rendering functionality.
    /// </summary>
    public abstract class BaseGame : Game
    {
        #region Members
        /// <summary>
        /// The <see cref="GraphicsDeviceManager"/> used to manage the <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> instance.
        /// </summary>
        private GraphicsDeviceManager graphicsDeviceManager;

        /// <summary>
        /// Current <see cref="Camera"/> being used when rendering the game.
        /// </summary>
        private Camera gameCamera;

        /// <summary>
        /// Current game <see cref="World"/> where all objects live.
        /// </summary>
        private World gameWorld;

        /// <summary>
        /// Instance of the <see cref="DeferredRenderSystem"/> being used to render the game.
        /// </summary>
        private DeferredRenderSystem renderSystem;

        /// <summary>
        /// The input manager instance.
        /// </summary>
        private InputManager inputManager;

        /// <summary>
        /// The instance of the ExtendedContentManager already cast.
        /// </summary>
        private ExtendedContentManager exContent;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGame"/> class.
        /// </summary>
        /// <param name="worldBounds">The bounds of the world (if null, {-1000, -1000} to {1000, 1000} is used)</param>
        public BaseGame(RectangleF worldBounds = null)
        {
            if (worldBounds == null)
                worldBounds = new RectangleF(-1000, -1000, 2000, 2000);

            this.graphicsDeviceManager = new GraphicsDeviceManager(this);
            this.gameWorld = new World(this, worldBounds);
            this.gameCamera = new Camera(this.gameWorld);
            this.renderSystem = new DeferredRenderSystem(this, this.gameWorld, this.gameCamera);
            this.inputManager = new InputManager(this);
            this.exContent = new ExtendedContentManager(this);
            base.Content = this.exContent;

            this.inputManager.UpdateOrder = -40;
            this.gameCamera.UpdateOrder = -30;
            this.gameWorld.UpdateOrder = -20;
            this.renderSystem.UpdateOrder = -10;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="GraphicsDeviceManager"/> used to manage the <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> instance.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return this.graphicsDeviceManager;
            }
        }

        /// <summary>
        /// Gets the <see cref="DeferredRenderSystem"/> used to render the game.
        /// </summary>
        public DeferredRenderSystem RenderSystem
        {
            get
            {
                return this.renderSystem;
            }
        }

        /// <summary>
        /// Gets or sets the game world.
        /// </summary>
        /// <value>
        /// The game world.
        /// </value>
        public World GameWorld
        {
            get
            {
                return this.gameWorld;
            }

            set
            {
                this.gameWorld = value;
                this.renderSystem.GameWorld = value;
            }
        }

        /// <summary>
        /// Gets the game camera.
        /// </summary>
        /// <value>
        /// The game camera used when rendering.
        /// </value>
        public Camera GameCamera
        {
            get
            {
                return this.gameCamera;
            }
        }

        /// <summary>
        /// Gets the input manager.
        /// </summary>
        public InputManager InputManager
        {
            get
            {
                return this.inputManager;
            }
        }

        /// <summary>
        /// Gets the current ContentManager.
        /// </summary>
        public new ExtendedContentManager Content
        {
            get
            {
                return this.exContent;
            }
        }
        #endregion

        #region Game Overrides
        /// <summary>
        /// Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.
        /// </summary>
        protected override void Initialize()
        {
            this.Components.Add(this.inputManager);
            //this.Components.Add(this.gameCamera);
            this.Components.Add(this.gameWorld);
            this.Components.Add(this.renderSystem);

            this.gameWorld.AddSimObject(this.gameCamera); // HACK: Not a long-term solution.
            base.Initialize();
        }
        #endregion
    }
}
