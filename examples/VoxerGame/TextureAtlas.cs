namespace VoxerGame
{
    using System.Collections.Generic;

    internal class TextureAtlas
    {
        public Dictionary<string, SubTexture> SubTextures { get; private set; }

        public TextureAtlas()
        {
            this.SubTextures = new Dictionary<string, SubTexture>();
        }
    }
}
