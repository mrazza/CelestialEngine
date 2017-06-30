using CelestialEngine.Core;
using CelestialEngine.Game;
using Microsoft.Xna.Framework;

namespace VoxerGame
{
    public class Tile
    {
        private SimpleSprite currentSprite;

        public int X { get; set; }

        public int Y { get; set; }

        public uint Type { get; set; }

        public TileFlags Flags { get; set; }

        public void CreateSprite(World gameWorld, Rectangle textureLocation, Vector2 position)
        {
            this.currentSprite = new SimpleSprite(gameWorld, "Content/spritesheet_tiles", "Content/spritesheet_tiles_normals", textureLocation, position, Vector2.Zero, false)
            {
                RenderOptions = SpriteRenderOptions.IsLit,
                RenderScale = new Vector2(0.5f, 0.5f),
                SpecularReflectivity = 1.0f,
                LayerDepth = 2,
            };
            this.currentSprite.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Static;
        }
    }
}
