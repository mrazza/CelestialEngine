using CelestialEngine.Core;
using Microsoft.Xna.Framework;

namespace VoxerGame
{
    internal class Chunk
    {
        public Tile[,] Tiles;

        public void Initialize(int seed, World gameWorld, TextureAtlas tilesAtlas, int xOffset, int width, int chunkHeight, int surfaceHeight, int topLayerDepth, float intensity)
        {
            this.Tiles = new Tile[width, chunkHeight];
            Simplex.Noise.Seed = seed;
            var surfaceMap = Simplex.Noise.Calc2D(width, topLayerDepth, 0.075f);

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

                this.Tiles[x, height].CreateSprite(gameWorld, tilesAtlas.SubTextures["dirt_grass.png"].Location, new Vector2(x + xOffset, height));

                for (int y = height + 1; y <= surfaceMap.GetUpperBound(1) + surfaceHeight; y++)
                {
                    this.Tiles[x, y] = new Tile()
                    {
                        X = x,
                        Y = height,
                        Flags = TileFlags.None,
                        Type = 1
                    };
                    this.Tiles[x, y].CreateSprite(gameWorld, surfaceMap[x,y - surfaceHeight] < 128 ? tilesAtlas.SubTextures["dirt.png"].Location : tilesAtlas.SubTextures["stone.png"].Location, new Vector2(x + xOffset, y));
                }
            }

            var caveMap = Simplex.Noise.Calc2D(width, chunkHeight - surfaceHeight - topLayerDepth, 0, topLayerDepth, 0.075f);
            for(int x = 0; x <= caveMap.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= caveMap.GetUpperBound(1); y++)
                {
                    var tileY = y + surfaceHeight + topLayerDepth;

                    if (caveMap[x, y] > 176)
                    {
                        this.Tiles[x, tileY] = new Tile()
                        {
                            X = x,
                            Y = tileY,
                            Flags = TileFlags.None,
                            Type = 0
                        };
                    }
                    else
                    {
                        this.Tiles[x, tileY] = new Tile()
                        {
                            X = x,
                            Y = tileY,
                            Flags = TileFlags.None,
                            Type = 1
                        };

                        this.Tiles[x, tileY].CreateSprite(gameWorld, tilesAtlas.SubTextures["stone.png"].Location, new Vector2(x + xOffset, tileY));
                    }
                }
            }
        }
    }
}
