using FSALib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace EFSAdvent
{
    public class Tiled
    {
        public static void ExportAsTMX(Room room, string filePath, string tilesetSource, string tvFilterSource = null)
        {
            List<Tiled.ILayer> layerGroup = new List<Tiled.ILayer>();
            List<string> layerGroupNames = new List<string>() {
                "TV Layer",
                "GBA Layer 1 (Dark World)",
                "GBA Layer 2",
                "GBA Layer 3",
                "GBA Layer 4",
                "GBA Layer 5",
                "GBA Layer 6",
                "GBA Layer 7",
            };

            var layers = room.Layers;
            int layerID = 1;
            for (int y = 0; y < 8; y++)
            {
                var group = new Tiled.Group()
                {
                    ID = y + 20,
                    Name = layerGroupNames[y],
                    Visible = y == 0,
                    Layers = new List<Tiled.ILayer>()
                };
                layerGroup.Add(group);

                for (int x = 0; x < 2; x++)
                {
                    group.Layers.Add(new Tiled.Layer()
                    {
                        ID = layerID++,
                        Name = $"Layer {y} {(x == 0 ? "Base" : "Top")}",
                        Data = layers[y * x].Tiles.ToArray(),
                        Size = y == 0 ? new Size(32, 24) : new Size(32, 32),

                    });
                }

                if (y == 0 && !string.IsNullOrEmpty(tvFilterSource))
                {
                    group.Layers.Add(new Tiled.Imagelayer()
                    {
                        ID = 18,
                        Name = "Overlay Filter",
                        RepeatX = true,
                        RepeatY = true,
                        ImageSize = new Size(128, 128),
                        ImageSource = tvFilterSource
                    });
                }
            }
            Tiled.ExportAsTMX(filePath, new Size(32, 32), layerGroup, tilesetSource);
        }

        public static Room ImportRoomFromTMX(string filePath)
        {
            List<Tiled.Layer> layers = Tiled.ReadLayersFromTMX(filePath);
            Room room = new Room();
            foreach (var layer in layers)
            {
                if (layer.Size.Width > FSALib.Layer.DIMENSION || layer.Size.Height > FSALib.Layer.DIMENSION)
                    throw new ArgumentException($"Error: Layer ID {layer.ID} exceeds allowed dimensions. Layer Size: {layer.Size.Width}x{layer.Size.Height}, Max: {FSALib.Layer.DIMENSION}x{FSALib.Layer.DIMENSION}");

                int layerID = layer.ID - 1;

                int yIndex = layerID / 2;
                int xIndex = layerID % 2;

                if (yIndex < 8 && xIndex < 2)
                    room.Layers[yIndex * xIndex].SetTiles(layer.Data, layer.Data.Length);
                else
                    throw new ArgumentException($"Layer ID {layer.ID} is out of bounds.");
            }
            return room;
        }

        public class Group : ILayer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public bool Visible { get; set; }
            public List<ILayer> Layers { get; set; }

            public XElement Export()
            {
                XElement group = new XElement("group",
                    new XAttribute("id", ID),
                    new XAttribute("name", Name),
                    new XAttribute("visible", Visible ? 1 : 0)
                );

                foreach (var Layer in Layers)
                    group.Add(Layer.Export());

                return group;
            }
        }

        public class Layer : ILayer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Size Size { get; set; }
            public ushort[] Data { get; set; }

            public XElement Export()
                => new XElement("layer",
                    new XAttribute("id", ID),
                    new XAttribute("name", Name),
                    new XAttribute("width", Size.Width),
                    new XAttribute("height", Size.Height),
                    new XElement("data",
                        new XAttribute("encoding", "csv"),
                        ExportMapDataToCsv(Data, Size.Width, Size.Height)
                    )
                );

            private static string ExportMapDataToCsv(ushort[] mapData, int width, int height)
            {
                int total = height * width;
                var sb = new StringBuilder(total * 2 + height);
                for (int i = 0; i < total; i++)
                {
                    // Tiled uses a different system, so we have to add 1 to adjust the tile ID.
                    sb.Append(mapData[i] + 1);

                    // Add a comma after the number except for the last element.
                    if (i < total - 1)
                        sb.Append(",");

                    // Check if we reached the end of a row and add a newline
                    if ((i + 1) % width == 0)
                        sb.AppendLine();
                }
                return sb.ToString();
            }

            public static Layer Parse(XElement layerElement)
            {
                int layerId = int.Parse(layerElement.Attribute("id").Value);
                string layerName = layerElement.Attribute("name").Value;
                int width = int.Parse(layerElement.Attribute("width").Value);
                int height = int.Parse(layerElement.Attribute("height").Value);
                string csvData = layerElement.Descendants("data").FirstOrDefault()?.Value.Trim();

                // Create a new Layer object
                Layer layer = new Layer
                {
                    ID = layerId,
                    Name = layerName,
                    Size = new Size(width, height),
                    Data = new ushort[width * height]
                };

                ParseCsvData(csvData, layer.Data);
                return layer;
            }

            private static int ParseCsvData(string csvData, ushort[] mapData)
            {
                string[] rows = csvData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                int index = 0;

                foreach (var row in rows)
                {
                    string[] tiles = row.TrimEnd(',').Split(',');
                    foreach (string tile in tiles)
                    {
                        // Tiled uses a different system, so we have to subtract 1 to adjust the tile ID.
                        mapData[index++] = (ushort)(ushort.Parse(tile) - 1);
                    }
                }
                return index;
            }
        }

        public class Imagelayer : ILayer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string ImageSource { get; set; }
            public Size ImageSize { get; set; }
            public bool RepeatX { get; set; }
            public bool RepeatY { get; set; }

            public XElement Export()
            {
                XElement imageLayerElement = new XElement("imagelayer",
                    new XAttribute("id", ID),
                    new XAttribute("name", Name),
                    new XAttribute("repeatx", RepeatX ? 1 : 0),
                    new XAttribute("repeaty", RepeatY ? 1 : 0)
                );

                XElement imageElement = new XElement("image",
                    new XAttribute("source", ImageSource)
                );

                if (ImageSize.Width > 0 && ImageSize.Height > 0)
                {
                    imageElement.Add(
                        new XAttribute("width", ImageSize.Width),
                        new XAttribute("height", ImageSize.Height)
                    );
                }

                imageLayerElement.Add(imageElement);
                return imageLayerElement;
            }
        }


        public interface ILayer
        {
            int ID { get; set; }
            string Name { get; set; }

            XElement Export();
        }

        public static void ExportAsTMX(string filePath, Size mapSize, List<ILayer> layers, string tilesetSource, int tileWidth = 16, int tileHeight = 16)
        {
            XElement mapElement = new XElement("map",
                new XAttribute("version", "1.0"),
                new XAttribute("tiledversion", "1.9.2"),
                new XAttribute("orientation", "orthogonal"),
                new XAttribute("renderorder", "right-down"),
                new XAttribute("width", mapSize.Width),
                new XAttribute("height", mapSize.Height),
                new XAttribute("tilewidth", tileWidth),
                new XAttribute("tileheight", tileHeight),
                new XElement("tileset",
                    new XAttribute("firstgid", 1),
                    new XAttribute("source", tilesetSource)
                )
            );

            foreach (var Layer in layers)
                mapElement.Add(Layer.Export());

            XDocument tmxDoc = new XDocument(mapElement);
            tmxDoc.Save(filePath);
        }

        public static List<Layer> ReadLayersFromTMX(string filePath)
        {
            var layers = new List<Layer>();

            XDocument tmxDoc = XDocument.Load(filePath);
            XElement mapElement = tmxDoc.Element("map");

            // Handle layers if no groups exist (or additional layers outside of groups)
            foreach (var layerElement in mapElement.Descendants("layer"))
            {
                layers.Add(Layer.Parse(layerElement));
            }
            return layers;
        }

        public static void UpdateTilesetImage(string baseTsxPath, string newImagePath, string outputTsxPath)
        {
            XDocument tsxDoc = XDocument.Load(baseTsxPath);

            XElement tilesetElement = tsxDoc.Root;
            if (tilesetElement == null || tilesetElement.Name != "tileset")
                throw new InvalidOperationException("The TSX file does not contain a valid <tileset> root element.");

            tilesetElement.SetAttributeValue("name", Path.GetFileNameWithoutExtension(newImagePath));

            XElement imageElement = tsxDoc.Root.Element("image");
            if (imageElement != null)
                imageElement.SetAttributeValue("source", newImagePath);
            else
                throw new InvalidOperationException("The <image> element was not found in the TSX document.");

            tsxDoc.Save(outputTsxPath);
        }
    }
}