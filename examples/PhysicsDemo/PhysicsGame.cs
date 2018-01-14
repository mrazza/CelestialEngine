// -----------------------------------------------------------------------
// <copyright file="PhysicsGame.cs" company="">
// Copyright (C) Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace PhysicsDemo
{
    using CelestialEngine.Core;
    using CelestialEngine.Game.PostProcess.Lights;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PhysicsGame : BaseGame
    {
        /// <summary>
        /// The was key down
        /// </summary>
        public bool wasKeyDown;

        /// <summary>
        /// The rand
        /// </summary>
        private Random rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicsGame"/> class.
        /// </summary>
        public PhysicsGame()
        {
            this.Window.Title = "Celavimus Engine Physics Demo";
            this.wasKeyDown = false;
            this.rand = new Random();
        }

        /// <summary>
        /// Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.
        /// </summary>
        protected override void Initialize()
        {
            // Configure the camera and gravity
            this.GameCamera.ConfigureFromViewRectangle(new RectangleF(5, 2, 40, 40), AspectRatioScaleMode.HorizontalDeformation);
            this.GameCamera.CameraZoom = 0.3f;
            this.GameWorld.PhysicsWorld.Gravity = new Vector2(0, 15f);

            // Set up user input
            this.InputManager.AddConditionalBinding((s) => s.IsKeyDown(Keys.Space), (s) =>
            {
                if (this.wasKeyDown)
                    return;

                // Create a new projectile
                new Projectile(this.GameWorld, new Vector2(60, this.rand.Next(10, 40)), new Vector2(this.rand.Next(-40, -25), this.rand.Next(-15, -5)), this.rand.Next(-30, 30) / 10.0f);
                this.wasKeyDown = true;
            });

            this.InputManager.AddConditionalBinding((s) => !s.IsKeyDown(Keys.Space), (s) => this.wasKeyDown = false);

            base.Initialize();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create the pyramid
            for (int row = 9; row >= 0; row--)
            {
                for (int col = 0; col < row; col++)
                {
                    // Create the sprite
                    new StackedBox(this.GameWorld, new Vector2(col * 4 + (9 - row) * 2, row * 4));
                }
            }

            // Create the floor
            for (int col = 0; col < 9; col++)
            {
                // Create the sprite
                new FloorTile(this.GameWorld, new Vector2(col * 4, 40));
            }

            // Create the point light
            var spotLight = new PointLight(this.GameWorld)
            {
                Position = new Vector3(35, 0, 10f),
                LayerDepth = 2,
                Color = Color.White,
                Power = 10f,
                Range = 55,
                CastsShadows = true,
                SpecularStrength = 4.00f
            };
            this.RenderSystem.AddPostProcessEffect(spotLight);

            // Add the ambient light
            var amLight = new AmbientLight(Color.White, 0.1f, true, 1);
            this.RenderSystem.AddPostProcessEffect(amLight);

            base.LoadContent();
        }
    }
}
