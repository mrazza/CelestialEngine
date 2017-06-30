// -----------------------------------------------------------------------
// <copyright file="Camera.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a 2D Camera that can be used when rendering the scene.
    /// </summary>
    public class Camera : SimBase
    {
        #region Members
        /// <summary>
        /// Camera zoom level as a percentage.
        /// </summary>
        private float cameraZoom;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <remarks>
        /// The default zoom level is 1.0 and the default number of pixels per world unit is 64.
        /// </remarks>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        public Camera(World world)
            : this(world, 1.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="cameraZoom">The camera zoom level as a percentage.</param>
        public Camera(World world, float cameraZoom)
            : base(world)
        {
            this.CameraZoom = cameraZoom;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the camera zoom level as a percentage.
        /// </summary>
        /// <value>
        /// The camera zoom level as a percentage.
        /// </value>
        public float CameraZoom
        {
            get
            {
                return this.cameraZoom;
            }

            set
            {
                if (value > 0)
                {
                    this.cameraZoom = value;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Configures the position and zoom of the camera to fit the target rectangle.
        /// </summary>
        /// <param name="targetRectangle">The target rectangle.</param>
        public void ConfigureFromViewRectangle(RectangleF targetRectangle)
        {
            this.ConfigureFromViewRectangle(targetRectangle, AspectRatioScaleMode.None);
        }

        /// <summary>
        /// Configures the position and zoom of the camera to fit the target rectangle by utilizing the specified scale mode.
        /// </summary>
        /// <param name="targetRectangle">The target rectangle.</param>
        /// <param name="scaleMode">The scale mode.</param>
        public void ConfigureFromViewRectangle(RectangleF targetRectangle, AspectRatioScaleMode scaleMode)
        {
            this.Position = targetRectangle.Center;

            // TODO: Fix this shit.

            //if (scaleMode == AspectRatioScaleMode.None)
            //{
            //    if (this.World.Game.RenderSystem.RenderTargets.ScreenRectangle.Height / targetRectangle.Height > this.Game.RenderSystem.RenderTargets.ScreenRectangle.Width / targetRectangle.Width)
            //    {
            //        scaleMode = AspectRatioScaleMode.VerticalDeformation;
            //    }
            //    else
            //    {
            //        scaleMode = AspectRatioScaleMode.HorizontalDeformation;
            //    }
            //}

            //switch (scaleMode)
            //{
            //    case AspectRatioScaleMode.HorizontalDeformation:
            //        this.cameraZoom = this.World.Game.RenderSystem.RenderTargets.ScreenRectangle.Height / targetRectangle.Height;
            //        break;

            //    case AspectRatioScaleMode.VerticalDeformation:
            //        this.cameraZoom = this.Game.RenderSystem.RenderTargets.ScreenRectangle.Width / targetRectangle.Width;
            //        break;
            //}
        }

        /// <summary>
        /// Gets the transform matrix used when rendering the scene.
        /// </summary>
        /// <param name="renderSystem">The render system that requires it.</param>
        /// <returns>The transformation matrix to use when rendering.</returns>
        public Matrix GetViewMatrix(DeferredRenderSystem renderSystem)
        {
            Rectangle screenSize = renderSystem.RenderTargets.ScreenRectangle;
            return Matrix.CreateTranslation(new Vector3(-this.PixelPosition.X, -this.PixelPosition.Y, 0)) * // Translate into screen space
                            Matrix.CreateRotationZ(-this.Rotation) * // Rotate counter to camera rotation
                            Matrix.CreateScale(new Vector3(this.cameraZoom, this.cameraZoom, 1)) * // Scale based on camera zoom
                            Matrix.CreateTranslation(new Vector3(screenSize.Width * 0.5f, screenSize.Height * 0.5f, 0)); // Translate to center on the screen
        }

        /// <summary>
        /// Gets the inverted transform matrix used when rendering the scene.
        /// </summary>
        /// <param name="renderSystem">The render system that requires it.</param>
        /// <returns>The inversion of the transformation matrix to use when rendering.</returns>
        public Matrix GetInvertedViewMatrix(DeferredRenderSystem renderSystem)
        {
            Rectangle screenSize = renderSystem.RenderTargets.ScreenRectangle;
            return Matrix.CreateTranslation(new Vector3(this.PixelPosition.X, this.PixelPosition.Y, 0)) *
                            Matrix.CreateRotationZ(this.Rotation) *
                            Matrix.CreateScale(new Vector3(1 / this.cameraZoom, 1 / this.cameraZoom, 1)) *
                            Matrix.CreateTranslation(new Vector3(-screenSize.Width * 0.5f, -screenSize.Height * 0.5f, 0));
        }

        /// <summary>
        /// Gets the projection matrix for the camera.
        /// </summary>
        /// <param name="renderSystem">The render system that requires it.</param>
        /// <returns>The projection matrix used by SpriteBatch when rendering.</returns>
        public Matrix GetProjectionMatrix(DeferredRenderSystem renderSystem)
        {
            Rectangle screenSize = renderSystem.RenderTargets.ScreenRectangle;
            return Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, screenSize.Width, screenSize.Height, 0, 0, 1);
        }

        /// <summary>
        /// Gets the inverted projection matrix for the camera.
        /// </summary>
        /// <param name="renderSystem">The render system that requires it.</param>
        /// <returns>The inversion of the projection matrix used by SpriteBatch when rendering.</returns>
        public Matrix GetInvertedProjectionMatrix(DeferredRenderSystem renderSystem)
        {
            return Matrix.Invert(this.GetProjectionMatrix(renderSystem));
        }

        /// <summary>
        /// Gets the view projection matrix for the camera.
        /// </summary>
        /// <param name="renderSystem">The render system that requires it.</param>
        /// <returns>The product of the view matrix for the camera and the projection matrix used by SpriteBatch when rendering.</returns>
        public Matrix GetViewProjectionMatrix(DeferredRenderSystem renderSystem)
        {
            return this.GetViewMatrix(renderSystem) * this.GetProjectionMatrix(renderSystem);
        }

        /// <summary>
        /// Gets the inverted view projection matrix for the camera.
        /// </summary>
        /// <param name="renderSystem">The render system that requires it.</param>
        /// <returns>The inversion of the product of the view matrix for the camera and the projection matrix used by SpriteBatch when rendering.</returns>
        public Matrix GetInvertedViewProjectionMatrix(DeferredRenderSystem renderSystem)
        {
            return this.GetInvertedViewMatrix(renderSystem) * this.GetInvertedProjectionMatrix(renderSystem);
        }
        #endregion
    }
}
