// -----------------------------------------------------------------------
// <copyright file="SimpleShadedSprite.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Game
{
    using CelestialEngine.Core;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A simple <see cref="SpriteBase"/> implementation that renders a simple color sprite and applies a single shader.
    /// </summary>
    public class SimpleShadedSprite : SimpleSprite
    {
        #region Members
        /// <summary>
        /// Shader to use when rendering this sprite
        /// </summary>
        private Shader shaderAsset;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleShadedSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="shader">The shader to use when rendering the sprite.</param>
        /// <param name="computeSpriteShape">If true, the sprite's shape is computed based on the sprite data; otherwise, the sprite's bounding box is used.</param>
        public SimpleShadedSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Shader shader, bool computeSpriteShape)
            : this(world, spriteTexturePath, spriteNormalTexturePath, shader, Vector2.Zero, Vector2.Zero, computeSpriteShape)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleShadedSprite"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="spriteTexturePath">The path to the sprite texture.</param>
        /// <param name="spriteNormalTexturePath">The path to the sprite's normal texture (can be null).</param>
        /// <param name="shader">The shader to use when rendering the sprite.</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The starting velocity of the object.</param>
        /// <param name="computeSpriteShape">If true, the sprite's shape is computed based on the sprite data; otherwise, the sprite's bounding box is used.</param>
        public SimpleShadedSprite(World world, string spriteTexturePath, string spriteNormalTexturePath, Shader shader, Vector2 position, Vector2 velocity, bool computeSpriteShape)
            : base(world, spriteTexturePath, spriteNormalTexturePath, null, position, velocity, computeSpriteShape)
        {
            this.shaderAsset = shader;
        }
        #endregion

        #region SpriteBase Overrides
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        public override void LoadContent(ExtendedContentManager contentManager)
        {
            this.shaderAsset.LoadContent(contentManager);

            base.LoadContent(contentManager);
        }

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to be drawn.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public override void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            renderSystem.BeginRender(this.shaderAsset);
            this.shaderAsset.ConfigureShader(renderSystem);
            this.shaderAsset.ApplyPass(0);
            renderSystem.DrawSprite(this.SpriteTexture, this.Position, null, this.RenderColor, this.Rotation, Vector2.Zero, this.RenderScale, this.SpriteMirroring);
            renderSystem.EndRender();
        }
        #endregion
    }
}
