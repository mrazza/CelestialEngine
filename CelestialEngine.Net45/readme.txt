!! IMPORTANT !!
Add the shader files in Content/Shaders/Core, Content/Shaders/Lights, and Content/Shaders/Simple to your MonoGame Content Builder project.

**
**
** Starter Base Game Class (with FPS counter)
**
**

    public class MyGame : BaseGame
    {
        private float secFrames;
        private double elapsed;
        private double lastFramesPerSec;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyGame"/> class.
        /// </summary>
        public MyGame()
        {
            Window.Title = "Celestial Engine Game";
        }

        /// <summary>
        /// Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the camera and/or input manager as needed
            base.Initialize();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content for the game or screen
            base.LoadContent();
        }

        /// <summary>
        /// Draws the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            Window.Title = String.Format("Celestial Engine Tech Demo [{0} lights @ {1} FPS(real), {2} FPS(avg)]", lightCount, Math.Round(1000.0f / gameTime.ElapsedGameTime.TotalMilliseconds), lastFramesPerSec);

            base.Draw(gameTime);

            secFrames++;
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsed > 1.0)
            {
                lastFramesPerSec = Math.Round(secFrames / elapsed);
                elapsed = 0;
                secFrames = 0;
            }
        }
    }