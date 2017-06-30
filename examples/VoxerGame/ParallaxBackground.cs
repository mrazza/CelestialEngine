namespace VoxerGame
{
    using CelestialEngine.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ParallaxBackground : SpriteBase
    {
        private Texture2D spriteTexture;
        private string spriteTexturePath;
        private Rectangle? spriteTextureBounds;
        private RectangleF spriteWorldBounds;
        private float speed;

        public ParallaxBackground(World world, string spriteTexturePath, Rectangle? spriteTextureBounds = null, float speed = 0.5f)
            : base(world)
        {
            this.spriteTexturePath = spriteTexturePath;
            this.spriteTextureBounds = spriteTextureBounds;
            this.speed = speed;
        }

        public override RectangleF SpriteWorldBounds
        {
            get
            {
                return this.spriteWorldBounds;
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
        }

        public override void DrawNormalMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
        }

        public override void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem)
        {
        }

        public override void LoadContent(ExtendedContentManager contentManager)
        {
            this.spriteTexture = contentManager.Load<Texture2D>(spriteTexturePath);

            if (!this.spriteTextureBounds.HasValue)
            {
                this.spriteTextureBounds = this.spriteTexture.Bounds;
            }

            this.spriteWorldBounds = new RectangleF(this.spriteTextureBounds.Value);
        }
    }
}
