// -----------------------------------------------------------------------
// <copyright file="ScreenSpriteBatch.cs" company="">
// Copyright (C) Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;
    using System.Text;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This class manages <see cref="SpriteBatch"/> so that calls to Begin() and End() will not result in 
    /// InvalidOperation exceptions caused by illegal call sequences. 
    /// </summary>
    public sealed class ScreenSpriteBatch : IDisposable
    {
        #region Members
        /// <summary>
        /// Set to true if Begin() has been called
        /// </summary>
        private bool isBegin;

        /// <summary>
        /// The current shader being used to render
        /// </summary>
        private Shader currentShader;

        /// <summary>
        /// The sprite batch instance we are wrapping.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The render system we're rendering on.
        /// </summary>
        private DeferredRenderSystem renderSystem;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenSpriteBatch"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system.</param>
        public ScreenSpriteBatch(DeferredRenderSystem renderSystem)
        {
            this.renderSystem = renderSystem;
            this.spriteBatch = new SpriteBatch(renderSystem.GraphicsDevice);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        public void Begin()
        {
            this.Begin(null);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="shader">The current rendering shader.</param>
        public void Begin(Shader shader)
        {
            // Are we already started?
            if (this.isBegin)
            {
                // Is the correct shader set?
                if (this.currentShader == shader)
                {
                    return; // We have no work to do
                }

                this.End();
            }

            Effect shaderAsset = shader != null ? shader.ShaderAsset : null;

            // TODO: I changed this from Immediate to deferred? Is this okay?
            this.spriteBatch.Begin(effect: this.currentShader?.ShaderAsset);
            this.isBegin = true;
            this.currentShader = shader;
        }

        /// <summary>
        /// Flushes the sprite batch and restores the device state to how it was before Begin was called.
        /// </summary>
        public void End()
        {
            if (this.isBegin)
            {
                this.isBegin = false;
                this.currentShader = null;
                this.spriteBatch.End();
            }
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture, position, source rectangle, color, rotation, origin, scale, effects and layer.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="position">The position to render at in screen space.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.</param>
        /// <param name="color">The color to tint a sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about the origin.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
        {
            this.spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, 0);
        }

        /// <summary>
        /// Adds text to the batch of sprites for rendering using the specified font, position, color, rotation, origin, scale, effects, and layer.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The location (in screen coordinates) to draw the string.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-Left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
        {
            this.spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, 0);
        }

        /// <summary>
        /// Adds text to the batch of sprites for rendering using the specified font, position, color, rotation, origin, scale, effects, and layer.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The location (in screen coordinates) to draw the string.</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-Left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
        {
            this.spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, 0);
        }
        #endregion

        #region IDisposable Overrides
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        //  unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.spriteBatch.Dispose();
        }
        #endregion
    }
}
