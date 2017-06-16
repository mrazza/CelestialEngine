// -----------------------------------------------------------------------
// <copyright file="DeferredRenderSystem.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CelestialEngine.Core.Collections;
    using CelestialEngine.Core.PostProcess;
    using CelestialEngine.Core.Shaders;
    using CelestialEngine.Game;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Linq;
    using Collections.QuadTree;

    /// <summary>
    /// Provides all sprite, lighting, and shader rendering functionality within the Celestial Deferred Render System.
    /// </summary>
    public sealed class DeferredRenderSystem : DrawableGameComponent
    {
        #region Members
        /// <summary>
        /// The vertex collection used when executing a direct screen paint
        /// </summary>
        private static readonly VertexPositionTexture[] screenPaintVerts =
        {
            new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1))
        };

        /// <summary>
        /// A collection of <see cref="RenderTarget2D"/> instances when rendering the scene.
        /// </summary>
        private RenderTargetManager renderTargets;

        /// <summary>
        /// The <see cref="IntelligentSpriteBatch"/> instance used to perform rendering.
        /// </summary>
        private IntelligentSpriteBatch spriteBatch;

        /// <summary>
        /// <see cref="World"/> instance that contains the scene we are rendering.
        /// </summary>
        private World gameWorld;

        /// <summary>
        /// <see cref="Camera"/> instance used for rendering.
        /// </summary>
        private Camera gameCamera;

        /// <summary>
        /// A sorted list of <see cref="StaticPostProcess"/> effects to be rendered on the scene.
        /// </summary>
        private ThrottledCollection<SortedSet<IPostProcess>, IPostProcess> postProcessingEffects;
        
        /// <summary>
        /// A QuadTree of all sprite objects in the world.
        /// </summary>
        private ThrottledCollection<QuadTree<SimulatedPostProcess, List<SimulatedPostProcess>>, SimulatedPostProcess> simulatedPostProcessEffects;

        /// <summary>
        /// Shader used when merging render targets.
        /// </summary>
        private MergeTargetsShader mergeRenderTargets;

        /// <summary>
        /// Shader used when merging render targets.
        /// </summary>
        private DebugTargetsShader debugRenderTargets;

        /// <summary>
        /// Represents a single white pixel.
        /// </summary>
        private Texture2D singlePixel;

        /// <summary>
        /// Specifies whether or not the engine is to render post processing effects
        /// </summary>
        private bool disablePostProcesses;

        /// <summary>
        /// The parent game instance.
        /// </summary>
        private BaseGame parentGame;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DeferredRenderSystem"/> class.
        /// </summary>
        /// <param name="parentGame">The <see cref="BaseGame"/> instance that is instantiating this renderer.</param>
        /// <param name="gameWorld">The <see cref="World"/> instance that contains the scene to be rendered.</param>
        /// <param name="gameCamera">The <see cref="Camera"/> instance that describes the perspective from which to render.</param>
        internal DeferredRenderSystem(BaseGame parentGame, World gameWorld, Camera gameCamera)
            : base(parentGame)
        {
            this.gameWorld = gameWorld;
            this.gameCamera = gameCamera;
            this.postProcessingEffects = new ThrottledCollection<SortedSet<IPostProcess>, IPostProcess>(new SortedSet<IPostProcess>());
            this.simulatedPostProcessEffects = new ThrottledCollection<QuadTree<SimulatedPostProcess, List<SimulatedPostProcess>>, SimulatedPostProcess>(new QuadTree<SimulatedPostProcess, List<SimulatedPostProcess>>(this.gameWorld.Bounds, 4));
            this.mergeRenderTargets = new MergeTargetsShader();
            this.debugRenderTargets = new DebugTargetsShader();
            this.disablePostProcesses = false;
            this.parentGame = parentGame;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the render targets used in the deferred rendering process.
        /// </summary>
        public RenderTargetManager RenderTargets
        {
            get
            {
                return this.renderTargets;
            }
        }

        /// <summary>
        /// Gets the <see cref="Camera"/> instance used for rendering.
        /// </summary>
        public Camera GameCamera
        {
            get
            {
                return this.gameCamera;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to render post processing effects.
        /// </summary>
        /// <remarks>
        /// If this is set to true only the color map is rendered and then is painted directly to the back buffer.
        /// 
        /// This defaults to false.
        /// </remarks>
        /// <value>
        ///   <c>true</c> if not rendering post process effects; otherwise, <c>false</c>.
        /// </value>
        public bool DisablePostProcesses
        {
            get
            {
                return this.disablePostProcesses;
            }

            set
            {
                this.disablePostProcesses = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating which render targets to paint directly to the screen (if any).
        /// </summary>
        /// <remarks>
        /// If this is set to a value, the specified map will be rendered directly to the screen.
        /// 
        /// If this is set to <c>All</c>, all maps will be rendered in a quad on screen.
        /// 
        /// This defaults to <c>Disabled</c>.
        /// </remarks>
        public DeferredRenderSystemDebugDrawMode DebugDrawMode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="World"/> instance that contains the scene we are rendering.
        /// </summary>
        internal World GameWorld
        {
            get
            {
                return this.gameWorld;
            }

            set
            {
                this.gameWorld = value;
            }
        }
        #endregion

        #region Render Methods
        /// <summary>
        /// Prepares the render system for upcoming draw calls.
        /// </summary>
        public void BeginRender()
        {
            this.BeginRender(null);
        }

        /// <summary>
        /// Prepares the render system for upcoming draw calls.
        /// </summary>
        /// <param name="shaderEffect">The shader effect to render with.</param>
        public void BeginRender(Shader shaderEffect)
        {
            this.spriteBatch.Begin(shaderEffect);
        }

        /// <summary>
        /// Finalizes the all draw calls and resets the render system state.
        /// </summary>
        public void EndRender()
        {
            return; // Reserved for future use
        }

        /// <summary>
        /// Gets the position in pixel-space from the specificed screen-space point.
        /// </summary>
        /// <param name="point">The point in screen space.</param>
        /// <returns>The point in pixel-space.</returns>
        public Vector2 GetPixelFromScreen(Vector2 point)
        {
            Vector3 pixelSpace = this.GraphicsDevice.Viewport.Unproject(new Vector3(point, 0), this.gameCamera.GetProjectionMatrix(this), this.gameCamera.GetViewMatrix(this), Matrix.Identity);
            return new Vector2(pixelSpace.X, pixelSpace.Y);
        }

        /// <summary>
        /// Gets the screen-space point from the specified pixel-space point.
        /// </summary>
        /// <param name="point">The point in pixel-space.</param>
        /// <returns>The point in screen-space.</returns>
        public Vector2 GetScreenFromPixel(Vector2 point)
        {
            Vector3 screenSpace = this.GraphicsDevice.Viewport.Project(new Vector3(point, 0), this.gameCamera.GetProjectionMatrix(this), this.gameCamera.GetViewMatrix(this), Matrix.Identity);
            return new Vector2(screenSpace.X, screenSpace.Y);
        }

        /// <summary>
        /// Gets the position in world space from the specified point in screen space.
        /// </summary>
        /// <param name="point">The point in screen space.</param>
        /// <returns>The world space position of the specified point.</returns>
        public Vector2 GetWorldFromScreen(Vector2 point)
        {
            Vector2 worldSpace = this.GetPixelFromScreen(point);
            worldSpace = this.GameWorld.GetWorldFromPixel(worldSpace);
            return worldSpace;
        }

        /// <summary>
        /// Gets the position in screen space from the specified point in world space.
        /// </summary>
        /// <param name="point">The point in world space.</param>
        /// <returns>The screen space position of the specified point.</returns>
        public Vector2 GetScreenFromWorld(Vector2 point)
        {
            Vector2 pixelSpace = this.GameWorld.GetPixelFromWorld(point);
            Vector2 screenSpace = this.GetScreenFromPixel(pixelSpace);
            return screenSpace;
        }

        /// <summary>
        /// Gets a <see cref="RectangleF"/> containing the position and size of the area that the camera will render.
        /// </summary>
        /// <returns>A <see cref="RectangleF"/> containing the position and size of the area that the camera will render.</returns>
        public RectangleF GetCameraRenderBounds()
        {
            Vector2 topLeft = this.GetWorldFromScreen(Vector2.Zero);
            Vector2 bottomRight = this.GetWorldFromScreen(new Vector2(this.RenderTargets.ScreenRectangle.Width, this.RenderTargets.ScreenRectangle.Height));
            bottomRight = bottomRight.RotateAbout(topLeft, -this.gameCamera.Rotation); // Restore bottom right to it's unrotated version

            return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y, this.gameCamera.Rotation);
        }

        /// <summary>
        /// Causes a direct screen paint rendering a fullscreen quad used for post-processing effects.
        /// </summary>
        public void DirectScreenPaint()
        {
            this.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, screenPaintVerts, 0, 2, VertexPositionTexture.VertexDeclaration);
        }

        /// <summary>
        /// Causes a direct screen paint rendering of a quad that covers the specified world area.
        /// </summary>
        /// <param name="area">The area in world space to render.</param>
        public void DirectScreenPaint(VertexPrimitive area)
        {
            VertexPrimitive pixelArea = this.gameWorld.GetPixelFromWorld(area);

            // Render the quad
            this.GraphicsDevice.DrawUserPrimitives(pixelArea.PrimitiveType, pixelArea.GetVertexData(), 0, pixelArea.Count - 2, VertexPositionTexture.VertexDeclaration);
        }

        /// <summary>
        /// Causes a direct screen paint rendering of a quad that covers the specified world area.
        /// </summary>
        /// <param name="area">The area in world space to render.</param>
        public void DirectScreenPaint(RectangleF area)
        {
            this.DirectScreenPaint(area.ToVertexPrimitive());
        }

        /// <summary>
        /// Adds the specified post processing effect to the collection.
        /// </summary>
        /// <param name="postEffect">The post effect to add.</param>
        public void AddPostProcessEffect(IPostProcess postEffect)
        {
            postEffect.LoadContent(this.parentGame.Content);

            if (postEffect is SimulatedPostProcess)
            {
                this.simulatedPostProcessEffects.Add((SimulatedPostProcess)postEffect);
            }
            else
            {
                this.postProcessingEffects.Add(postEffect);
            }
        }

        /// <summary>
        /// Removes the specified post processing effect from the collection.
        /// </summary>
        /// <param name="postEffect">The post effect to remove.</param>
        public void RemovePostProcessEffect(IPostProcess postEffect)
        {
            if (postEffect is SimulatedPostProcess)
            {
                this.simulatedPostProcessEffects.Remove((SimulatedPostProcess)postEffect);
            }
            else
            {
                this.postProcessingEffects.Remove(postEffect);
            }
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, position, source rectangle, color, rotation, origin, scale, effects and layer. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="scale">Scale factor.</param>
        public void DrawSprite(Texture2D texture, Vector2 position, float rotation, Vector2 scale)
        {
            this.DrawSprite(texture, position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None);
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, position, source rectangle, color, rotation, origin, scale, effects and layer. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-Left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
        {
            this.spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects);
        }

        /// <summary>
        /// Adds text to the batch of sprites for rendering using the specified font, position, color, rotation, origin, scale, effects, and layer.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-Left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
        {
            this.spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects);
        }

        /// <summary>
        /// Adds text to the batch of sprites for rendering using the specified font, position, color, rotation, origin, scale, effects, and layer.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-Left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
        {
            this.spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects);
        }

        /// <summary>
        /// Draws the filled, colored rectangle at the specified position.
        /// </summary>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="rotation">The rotation of the rectangle.</param>
        public void DrawFilledRectangle(RectangleF rectangle, Color color, float rotation)
        {
            RectangleF rotatedRectangle = rectangle;
            rotatedRectangle.Rotation += rotation;
            this.spriteBatch.Draw(this.singlePixel, rotatedRectangle.Position, null, color, rotatedRectangle.Rotation, Vector2.Zero, this.gameWorld.GetPixelFromWorld(rotatedRectangle.AreaBounds), SpriteEffects.None);
        }

        /// <summary>
        /// Draws a line between the two specified points in world space with the specified color and thickness.
        /// </summary>
        /// <param name="startPoint">The start point of the line.</param>
        /// <param name="endPoint">The end point of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        public void DrawLine(Vector2 startPoint, Vector2 endPoint, Color color, float thickness)
        {
            Vector2 line = endPoint - startPoint;
            float lineLength = line.Length();
            float lineRotation = line.AngleBetween(Vector2Helper.Right);
            this.DrawLine(startPoint, lineLength, color, thickness, lineRotation);
        }

        /// <summary>
        /// Draws a line from the left to the right starting at the specified point with the specified length and rotation.
        /// </summary>
        /// <param name="startPoint">The start point of the line.</param>
        /// <param name="length">The length of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="rotation">The rotation of the line.</param>
        public void DrawLine(Vector2 startPoint, float length, Color color, float thickness, float rotation)
        {
            this.DrawFilledRectangle(new RectangleF(startPoint.X, startPoint.Y - (thickness / 2.0f), length, thickness), color, rotation);
        }

        /// <summary>
        /// Draws a rectangular border at the specified position with the specified thickness and rotation.
        /// </summary>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="color">The color of the border.</param>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="rotation">The rotation of the rectangle.</param>
        public void DrawRectangleBorder(RectangleF rectangle, Color color, float thickness, float rotation)
        {
            RectangleF rotatedRectangle = rectangle;
            rotatedRectangle.Rotation += rotation;
            this.DrawLine(rotatedRectangle.TopLeft, rectangle.Width, color, thickness, rotatedRectangle.Rotation); // TopLeft to TopRight
            this.DrawLine(rotatedRectangle.TopRight, rectangle.Height, color, thickness, rotatedRectangle.Rotation + MathHelper.PiOver2); // TopRight to BottomRight

            this.DrawLine(rotatedRectangle.BottomLeft, rectangle.Width, color, thickness, rotatedRectangle.Rotation); // BottomLeft to BottomRight
            this.DrawLine(rotatedRectangle.BottomLeft, rectangle.Height, color, thickness, rotatedRectangle.Rotation - MathHelper.PiOver2); // BottomLeft to TopLeft
        }

        /// <summary>
        /// Sets the active render targets in this order (Color, Normal, Light).
        /// </summary>
        /// <param name="renderTargetTypes">The render target types requested.</param>
        /// <param name="count">The number of render targets requested.</param>
        public void SetRenderTargets(RenderTargetTypes renderTargetTypes, int count)
        {
            if (renderTargetTypes == RenderTargetTypes.None)
            {
                this.GraphicsDevice.SetRenderTarget(null);
            }
            else
            {
                RenderTargetBinding[] renderTargets = new RenderTargetBinding[count];

                if (this.renderTargets.GetRenderTargets(renderTargetTypes, ref renderTargets) != count)
                {
                    Logger.Throw(Logger.Level.Fatal, new InvalidOperationException("Specified render target count differs from count received."));
                }

                this.GraphicsDevice.SetRenderTargets(renderTargets);
            }
        }

        /// <summary>
        /// Clears the current render target.
        /// </summary>
        /// <param name="clearColor">Color to clear the target with.</param>
        public void ClearCurrentRenderTarget(Color clearColor)
        {
            this.GraphicsDevice.Clear(clearColor);
        }
        #endregion

        #region DrawableGameComponent Overrides
        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            this.postProcessingEffects.ProcessUpdates();
            this.simulatedPostProcessEffects.ProcessUpdates();

            foreach (var curr in this.simulatedPostProcessEffects.ToArray())
            {
                this.simulatedPostProcessEffects.GetBackingCollection().Reposition(curr);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Called when the <see cref="SpriteBase"/> components need to be drawn. Override this method with component-specific drawing code.
        /// </summary>
        /// <remarks>
        /// You should never call this function; it is called by XNA.
        /// </remarks>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            // Get the items that need rendering
            var spritesToDraw = this.gameWorld.GetSpriteObjectsInArea(this.GetCameraRenderBounds()).OrderBy(s => s.LayerDepth);

            if (!this.disablePostProcesses)
            {
                // Render options map
                this.GraphicsDevice.SetRenderTarget(this.renderTargets.OptionsMap); // Set the correct render target
                this.GraphicsDevice.Clear(Color.Transparent); // Initialize the render target

                this.gameWorld.DrawSpriteOptions(spritesToDraw, gameTime, this); // Render on options map
                this.spriteBatch.End();

                this.GraphicsDevice.SetRenderTarget(null); // Resolve the render target
            }
            
            // Render color map
            this.GraphicsDevice.SetRenderTarget(this.renderTargets.ColorMap); // Set the correct render target
            this.GraphicsDevice.Clear(Color.Transparent); // Initialize the render target

            this.gameWorld.DrawSpriteColor(spritesToDraw, gameTime, this); // Render on color map
            this.spriteBatch.End();

            this.GraphicsDevice.SetRenderTarget(null); // Resolve the render target

            if (!this.disablePostProcesses)
            {
                // Render normal map
                this.GraphicsDevice.SetRenderTarget(this.renderTargets.NormalMap); // Set the correct render target
                this.GraphicsDevice.Clear(Color.Transparent); // Initialize the render target

                this.gameWorld.DrawSpriteNormal(spritesToDraw, gameTime, this); // Render on normal map
                this.spriteBatch.End();

                this.GraphicsDevice.SetRenderTarget(null); // Resolve the render target

                // Render lighting and post-processing effects
                this.GraphicsDevice.SetRenderTarget(this.renderTargets.LightMap); // Set the correct render target
                this.GraphicsDevice.Clear(Color.Transparent); // Initialize the render target
                this.GraphicsDevice.SetRenderTarget(null); // Resolve the render target

                this.RenderPostProcesses(gameTime); // Render post processing effects
                this.GraphicsDevice.SetRenderTarget(null); // Resolve the render target
            }
            else
            {
                this.GraphicsDevice.SetRenderTarget(this.renderTargets.LightMap); // Set the correct render target
                this.GraphicsDevice.Clear(Color.White); // Initialize the render target
                this.GraphicsDevice.SetRenderTarget(null); // Resolve the render target
            }

            if (this.DebugDrawMode == DeferredRenderSystemDebugDrawMode.Disabled)
            {
                // Merge render targets
                this.GraphicsDevice.Clear(Color.Transparent);
                this.mergeRenderTargets.ConfigureShader(this);
                this.mergeRenderTargets.ApplyPass(0);
                this.DirectScreenPaint();

                // TODO: Draw any non-lit textures
                var rsdSpriteBatch = new ScreenSpriteBatch(this);
                rsdSpriteBatch.Begin();
                this.gameWorld.DrawScreenDrawableComponents(gameTime, rsdSpriteBatch);
                rsdSpriteBatch.End();
            }
            else if (this.DebugDrawMode == DeferredRenderSystemDebugDrawMode.All)
            {
                this.GraphicsDevice.Clear(Color.Transparent);
                this.debugRenderTargets.ConfigureShader(this);
                this.debugRenderTargets.ApplyPass(1);
                this.DirectScreenPaint();
                //debugRenderTargets
            }
            else
            {
                RenderTarget2D source = null;

                switch (this.DebugDrawMode)
                {
                    case DeferredRenderSystemDebugDrawMode.ColorMap:
                        source = this.RenderTargets.ColorMap;
                        break;

                    case DeferredRenderSystemDebugDrawMode.OptionsMap:
                        source = this.RenderTargets.OptionsMap;
                        break;

                    case DeferredRenderSystemDebugDrawMode.LightMap:
                        source = this.RenderTargets.LightMap;
                        break;
                }

                if (source == null)
                {
                    throw new ArgumentNullException(nameof(this.DebugDrawMode), $"Invalid DeferredRenderSystemDebugDrawMode: {this.DebugDrawMode}");
                }

                this.GraphicsDevice.Clear(Color.Transparent);
                this.debugRenderTargets.ConfigureShader(this);
                this.debugRenderTargets.GetParameter("renderMap").SetValue(source);
                this.debugRenderTargets.ApplyPass(0);
                this.DirectScreenPaint();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.GraphicsDevice.PresentationParameters.DepthStencilFormat = DepthFormat.None;
            this.spriteBatch = new IntelligentSpriteBatch(this);
            this.renderTargets = new RenderTargetManager(this.GraphicsDevice);
            this.mergeRenderTargets.LoadContent(this.parentGame.Content);
            this.debugRenderTargets.LoadContent(this.parentGame.Content);

            // Create the SinglePixel instance
            this.singlePixel = new Texture2D(this.GraphicsDevice, 1, 1);
            this.singlePixel.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// Called when graphics resources need to be unloaded. Override this method to unload any component-specific graphics resources.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
            this.singlePixel.Dispose();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Executes draw calls on all the loaded, enabled, post process effects
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to RenderPostProcesses.</param>
        private void RenderPostProcesses(GameTime gameTime)
        {
            foreach (IPostProcess curr in this.postProcessingEffects.Union(
                                          this.simulatedPostProcessEffects.GetBackingCollection().GetItemsInBounds(this.GetCameraRenderBounds()))
                                          .OrderBy(p => p.RenderPriority))
            {
                if (curr.IsEnabled)
                {
                    curr.Draw(gameTime, this);
                }
            }
        }
        #endregion
    }
}
