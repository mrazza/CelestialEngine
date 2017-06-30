namespace VoxerGame
{
    using Microsoft.Xna.Framework;
    using System.IO;
    using System.Xml;

    internal class TextureAtlasReader
    {
        public TextureAtlas FromXmlStream(Stream inputStream)
        {
            var result = new TextureAtlas();

            using (var xmlReader = XmlReader.Create(inputStream))
            {
                if (!xmlReader.IsStartElement("TextureAtlas"))
                    throw new XmlException($"Unexpected root element {xmlReader.Name}, expecting TextureAtlas");

                xmlReader.ReadStartElement();

                while (xmlReader.IsStartElement())
                {
                    if (!xmlReader.IsStartElement("SubTexture"))
                        throw new XmlException($"Unexpected element {xmlReader.Name}, expecting SubTexture");

                    var isEmpty = xmlReader.IsEmptyElement;
                    var name = xmlReader.GetAttribute("name");
                    var x = int.Parse(xmlReader.GetAttribute("x"));
                    var y = int.Parse(xmlReader.GetAttribute("y"));
                    var width = int.Parse(xmlReader.GetAttribute("width"));
                    var height = int.Parse(xmlReader.GetAttribute("height"));

                    var subtexture = new SubTexture() { Name = name, Location = new Rectangle(x, y, width, height) };
                    result.SubTextures[name] = subtexture;

                    xmlReader.ReadStartElement();

                    if (!isEmpty)
                        xmlReader.ReadEndElement();
                }

                xmlReader.ReadEndElement();
            }

            return result;
        }
    }
}
