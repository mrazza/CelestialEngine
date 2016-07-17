// -----------------------------------------------------------------------
// <copyright file="RenderTargetManager.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using CelestialEngine.Core.Collections;
    using System;

    /// <summary>
    /// Contains all the render targets used in the deferred rendering process
    /// </summary>
    public sealed class RenderTargetManager
    {
        #region Members
        /// <summary>
        /// The color map for the scene (diffuse map).
        /// </summary>
        private RenderTarget2D colorMap;

        /// <summary>
        /// The normal map for the scene (normal vectors off scene objects).
        /// </summary>
        private RenderTarget2D normalMap;

        /// <summary>
        /// The light map for the scene (calculated light map).
        /// </summary>
        private RenderTarget2D lightMap;

        /// <summary>
        /// The map used to specify per-pixel options for shaders.
        /// </summary>
        private RenderTarget2D optionsMap;

        /// <summary>
        /// Array of the render targets used for setting the active targets.
        /// </summary>
        private RenderTargetBinding[] renderTargetArray;

        /// <summary>
        /// A rectangle the size of the back buffer.
        /// </summary>
        private Rectangle screenRectangle;

        /// <summary>
        /// The pool of temporary render targets
        /// </summary>
        private ResourcePool<RenderTarget2D> renderTargetPool;

        /// <summary>
        /// The graphics device we're working with.
        /// </summary>
        private GraphicsDevice gfxDevice;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetManager"/> class and generates the render targets.
        /// </summary>
        /// <param name="gfxDevice">The GFX device.</param>
        internal RenderTargetManager(GraphicsDevice gfxDevice)
        {
            this.gfxDevice = gfxDevice;
            this.GenerateRenderTargets(gfxDevice);
            this.renderTargetPool = new ResourcePool<RenderTarget2D>(new TimeSpan(0, 0, 30));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the color map.
        /// </summary>
        public RenderTarget2D ColorMap
        {
            get
            {
                return this.colorMap;
            }
        }

        /// <summary>
        /// Gets the normal map.
        /// </summary>
        public RenderTarget2D NormalMap
        {
            get
            {
                return this.normalMap;
            }
        }

        /// <summary>
        /// Gets the light map.
        /// </summary>
        public RenderTarget2D LightMap
        {
            get
            {
                return this.lightMap;
            }
        }

        /// <summary>
        /// Gets the options map.
        /// </summary>
        public RenderTarget2D OptionsMap
        {
            get
            {
                return this.optionsMap;
            }
        }

        /// <summary>
        /// Gets a rectangle the size of the back buffer and render targets.
        /// </summary>
        public Rectangle ScreenRectangle
        {
            get
            {
                return this.screenRectangle;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a temporary render target to be used for intermediate effects.
        /// </summary>
        /// <remarks>
        /// If a <see cref="RenderTarget2D"/> with the surface format you requested does not exist in the pool,
        /// it will be created for you. If the system fails to create a render target with the requested surface
        /// format it throws an InvalidOperationException.
        /// 
        /// The state of the <see cref="RenderTarget2D"/> that you receive is undetermined. You can NOT assume
        /// that is has been initialized to any state.
        /// </remarks>
        /// <param name="format">The <see cref="SurfaceFormat"/> of the render target.</param>
        /// <returns>A render target fitting the requested specifications.</returns>
        public RenderTarget2D GetTemporaryRenderTarget(SurfaceFormat format)
        {
            RenderTarget2D pooledRT = this.renderTargetPool.Get((rt) => rt.Format == format, () => this.CreateRenderTarget(this.gfxDevice, format));

            if (pooledRT.Format != format)
            {
                throw new InvalidOperationException("The system does not support " + format.ToString() + ".");
            }

            return pooledRT;
        }

        /// <summary>
        /// Releases the unneeded temporary render target back into the pool.
        /// </summary>
        /// <remarks>
        /// The released render target can be in any state. You are not responsible for clearing
        /// the render target before you return it to the pool.
        /// </remarks>
        /// <param name="rt">The render target to release</param>
        public void ReleaseTemporaryRenderTarget(RenderTarget2D rt)
        {
            this.renderTargetPool.Release(rt);
        }

        /// <summary>
        /// Gets an array of render targets based on the specified types.
        /// </summary>
        /// <remarks>
        /// Render targets are added to the array in the default order:
        /// Color Map
        /// Normal Map
        /// Light Map
        /// Options Map
        /// Shadow Map
        /// </remarks>
        /// <param name="renderTargetTypes">The render target types.</param>
        /// <param name="outTargets">A <see cref="RenderTargetBinding"/> array to populate.</param>
        /// <returns>The number of render targets added to the array.</returns>
        public int GetRenderTargets(RenderTargetTypes renderTargetTypes, ref RenderTargetBinding[] outTargets)
        {
            int arrayOffset = 0;

            if ((renderTargetTypes & RenderTargetTypes.ColorMap) != 0)
            {
                outTargets[arrayOffset++] = this.colorMap;
            }

            if ((renderTargetTypes & RenderTargetTypes.NormalMap) != 0)
            {
                outTargets[arrayOffset++] = this.normalMap;
            }

            if ((renderTargetTypes & RenderTargetTypes.LightMap) != 0)
            {
                outTargets[arrayOffset++] = this.lightMap;
            }

            if ((renderTargetTypes & RenderTargetTypes.OptionsMap) != 0)
            {
                outTargets[arrayOffset++] = this.optionsMap;
            }

            return arrayOffset;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Generates the render targets.
        /// </summary>
        /// <param name="gfxDevice">The GFX device.</param>
        internal void GenerateRenderTargets(GraphicsDevice gfxDevice)
        {
            this.colorMap = this.CreateRenderTarget(gfxDevice, SurfaceFormat.Color);
            this.normalMap = this.CreateRenderTarget(gfxDevice, SurfaceFormat.Color);
            this.lightMap = this.CreateRenderTarget(gfxDevice, SurfaceFormat.Color);
            this.optionsMap = this.CreateRenderTarget(gfxDevice, SurfaceFormat.Color);

            this.renderTargetArray = new RenderTargetBinding[]
            {
                new RenderTargetBinding(this.colorMap),
                new RenderTargetBinding(this.normalMap),
                new RenderTargetBinding(this.lightMap),
                new RenderTargetBinding(this.optionsMap)
            };

            this.screenRectangle = new Rectangle(0, 0, gfxDevice.PresentationParameters.BackBufferWidth, gfxDevice.PresentationParameters.BackBufferHeight);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a render target with the specified <see cref="SurfaceFormat"/>.
        /// </summary>
        /// <param name="gfxDevice">The GFX device.</param>
        /// <param name="surfaceFormat">The surface format.</param>
        /// <returns>The newly created render target.</returns>
        private RenderTarget2D CreateRenderTarget(GraphicsDevice gfxDevice, SurfaceFormat surfaceFormat)
        {
            return new RenderTarget2D(gfxDevice, gfxDevice.PresentationParameters.BackBufferWidth, gfxDevice.PresentationParameters.BackBufferHeight, false, surfaceFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }
        #endregion
    }
}
