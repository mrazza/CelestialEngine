namespace VoxerGame
{
    using CelestialEngine.Core;
    using CelestialEngine.Game.PostProcess.Lights;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.IO;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class VoxerGame : BaseGame
    {
        public Tile[,] Tiles;
        private PointLight sun;
        private AmbientLight amLight;
        private float secFrames;
        private double elapsed;
        private double lastFramesPerSec;
        private TextureAtlas tilesAtlas;
        private ParallaxBackground background = null;
        private Chunk[] chunks;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoxerGame"/> class.
        /// </summary>
        public VoxerGame()
        {
            Window.Title = "Celestial Engine Game";
        }

        /// <summary>
        /// Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the camera and/or input manager as needed
            this.GameCamera.Position = this.GameWorld.GetWorldFromPixel(new Vector2(this.GraphicsDevice.Viewport.Width / 2.0f, 0.0f) / this.GameCamera.CameraZoom);
            this.GameCamera.Position = new Vector2(this.GameCamera.Position.X, 72f);
            this.InputManager.AddConditionalBinding((s) => { return s.IsKeyDown(Keys.Escape); }, (s) => this.Exit());
            this.InputManager.AddBinding((s) =>
            {
                Vector2 cameraVelocity = new Vector2();

                if (s.IsKeyDown(Keys.A) || s.IsKeyDown(Keys.Left))
                {
                    cameraVelocity.X -= 1.0f;
                }

                if (s.IsKeyDown(Keys.D) || s.IsKeyDown(Keys.Right))
                {
                    cameraVelocity.X += 1.0f;
                }

                if (s.IsKeyDown(Keys.W) || s.IsKeyDown(Keys.Up))
                {
                    cameraVelocity.Y -= 1.0f;
                }

                if (s.IsKeyDown(Keys.S) || s.IsKeyDown(Keys.Down))
                {
                    cameraVelocity.Y += 1.0f;
                }

                if (cameraVelocity != Vector2.Zero)
                {
                    cameraVelocity.Normalize();
                    cameraVelocity *= 4.0f;

                    if (s.IsKeyDown(Keys.LeftShift))
                    {
                        cameraVelocity *= 3.0f;
                    }
                }

                this.GameCamera.Velocity = cameraVelocity;
            });
            
            this.InputManager.AddConditionalBinding((s) => s.IsFirstKeyPress(Keys.F1), (s) => this.RenderSystem.DebugDrawMode = DeferredRenderSystemDebugDrawMode.Disabled);
            this.InputManager.AddConditionalBinding((s) => s.IsFirstKeyPress(Keys.F2), (s) => this.RenderSystem.DebugDrawMode = DeferredRenderSystemDebugDrawMode.ColorMap);
            this.InputManager.AddConditionalBinding((s) => s.IsFirstKeyPress(Keys.F3), (s) => this.RenderSystem.DebugDrawMode = DeferredRenderSystemDebugDrawMode.OptionsMap);
            this.InputManager.AddConditionalBinding((s) => s.IsFirstKeyPress(Keys.F4), (s) => this.RenderSystem.DebugDrawMode = DeferredRenderSystemDebugDrawMode.LightMap);
            this.InputManager.AddConditionalBinding((s) => s.IsFirstKeyPress(Keys.F5), (s) => this.RenderSystem.DebugDrawMode = DeferredRenderSystemDebugDrawMode.NormalMap);

            base.Initialize();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content for the game or screen
            using (var s = File.OpenRead("Content/spritesheet_tiles.xml"))
            {
                tilesAtlas = new TextureAtlasReader().FromXmlStream(s);
            }

            RectangleF bounds = this.RenderSystem.GetCameraRenderBounds();

            //    this.chunks[i].Initialize(r.Next(), this.GameWorld, tilesAtlas, 16 * i, 16, 192, 72, 12, (float)r.NextDouble() * 8.0f + 4.0f);
            var r = new Random();
            var seed = r.Next();
            var width = 256;
            var chunkHeight = 192;
            var surfaceHeight = 72;
            var intensity = (float)r.NextDouble() * 8.0f + 2.0f;

            this.Tiles = new Tile[width, chunkHeight];
            Simplex.Noise.Seed = seed;
            var surfaceMap = Simplex.Noise.Calc2D(width, chunkHeight, 0.075f);
            var spriteCount = 0;

            for (int x = 0; x <= surfaceMap.GetUpperBound(0); x++)
            {
                var height = (int)((surfaceMap[x, 0] / 255.0f) * intensity) + surfaceHeight;

                for (int y = surfaceHeight; y <= height; y++)
                {
                    this.Tiles[x, y] = new Tile()
                    {
                        X = x,
                        Y = y,
                        Flags = TileFlags.None,
                        Type = 0
                    };
                }

                this.Tiles[x, height] = new Tile()
                {
                    X = x,
                    Y = height,
                    Flags = TileFlags.None,
                    Type = 1
                };

                this.Tiles[x, height].CreateSprite(this.GameWorld, tilesAtlas.SubTextures["dirt_grass.png"].Location, new Vector2(x, height));
                spriteCount++;

                for (int y = height + 1; y <= surfaceMap.GetUpperBound(1); y++)
                {
                    this.Tiles[x, y] = new Tile()
                    {
                        X = x,
                        Y = height,
                        Flags = TileFlags.None,
                        Type = 1
                    };
                    this.Tiles[x, y].CreateSprite(this.GameWorld, surfaceMap[x, y - surfaceHeight] < 96 ? tilesAtlas.SubTextures["dirt.png"].Location : tilesAtlas.SubTextures["stone.png"].Location, new Vector2(x, y));
                    spriteCount++;
                }
            }

            //for (int i = 0; i < this.chunks.Length; i++)
            //{
            //    this.chunks[i] = new Chunk();
            //    this.chunks[i].Initialize(r.Next(), this.GameWorld, tilesAtlas, 16 * i, 16, 192, 72, 12, (float)r.NextDouble() * 8.0f + 4.0f);
            //}

            this.amLight = new AmbientLight(Color.White, 0.75f, true, 1);
            this.RenderSystem.AddPostProcessEffect(this.amLight);

            System.Diagnostics.Trace.TraceInformation(spriteCount.ToString());

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            Window.Title = $"Celestial Engine Tech Demo [{Math.Round(1000.0f / gameTime.ElapsedGameTime.TotalMilliseconds)} FPS(real), {lastFramesPerSec} FPS(avg)] [{this.GameCamera.Position.X}, {this.GameCamera.Position.Y}] [{this.GameWorld.SimObjectCount}]";

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
}
