using FSALib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace EFSAdvent.FourSwords
{
    public class Level
    {
        public Map Map { get; private set; }
        public Room Room { get; }

        public bool IsDirty => MapIsDirty || LayersAreDirty || ActorsAreDirty;
        public bool ActorsAreDirty { get; private set; }
        public bool LayersAreDirty { get; private set; }
        public bool MapIsDirty { get; private set; }

        private readonly string _number;
        private readonly string _basePath;
        private readonly string _mapPath;
        private readonly Logger _logger;

        public Level(string mapPath, Logger logger)
        {
            _number = new String(mapPath.Split(Path.DirectorySeparatorChar).Last().Skip(3).Take(3).ToArray());
            _basePath = mapPath.Remove(mapPath.LastIndexOf("map" + Path.DirectorySeparatorChar));
            _mapPath = mapPath;
            _logger = logger;

            if (_mapPath.EndsWith("map500_ctrl.csv"))
            {
                _mapPath = _mapPath.Replace("map500_ctrl.csv", "map500.csv");
                _logger.AppendText("Unknown what map500_ctrl.csv is, loading map500.csv instead.");
            }
            Room = new Room();
            Room.Actors.CollectionChanged += (sender, e) => { ActorsAreDirty = true; };
            foreach (var layer in Room.Layers)
            {
                layer.PropertyChanged += (sender, e) => { LayersAreDirty = true; };
            }
            LoadMap();
        }

        public void LoadMap()
        {
            // Load map file
            if (!File.Exists(_mapPath))
            {
                Map = new Map();
                _logger.AppendLine($"File not found: {_mapPath}");
            }
            else
            {
                using FileStream mapStream = File.Open(_mapPath, FileMode.Open);
                Map = new Map(mapStream);
            }

            // Inform us when something is changed
            Map.PropertyChanged += (sender, e) => { MapIsDirty = true; };
            if (Map.IsShadowBattle)
            {
                ReadOnlySpan<ShadowMapProperties> shadowRooms = Map.ShadowRooms;
                for (int i = 0; i < shadowRooms.Length; i++)
                {
                    shadowRooms[i].PropertyChanged += (sender, e) => { MapIsDirty = true; };
                }
            }
            MapIsDirty = false;
        }

        public void SaveMap()
        {
            // Save map file
            using (FileStream mapStream = File.Open(_mapPath, FileMode.Create))
            {
                Map.BinarySerialize(mapStream);
            }
            MapIsDirty = false;
        }

        public bool LoadRoom(int roomNumber, bool newRoom = false)
        {
            if (roomNumber == Map.EMPTY_ROOM_VALUE)
            {
                return false;
            }

            Room.Index = roomNumber;
            ReadOnlySpan<Layer> layers = Room.Layers;

            for (int i = 0; i < layers.Length; i++)
            {
                if (newRoom)
                {
                    layers[i].Clear();
                }
                else
                {
                    string layerPath = Layer.GetFilePath(_basePath, Map.Index, roomNumber, i > 7 ? 2 : 1, i % 8);
                    if (!File.Exists(layerPath))
                    {
                        layers[i].Clear();
                        _logger.AppendLine($"File not found: {layerPath}");
                    }
                    else
                    {
                        using FileStream layerStream = File.Open(layerPath, FileMode.Open);
                        layers[i].BinaryDeserialize(layerStream);
                    }
                }
            }
            LayersAreDirty = false;
            ReloadActors();
            return true;
        }

        public bool RoomExists(int roomNumber)
        {
            string filePath = Layer.GetFilePath(_basePath, Map.Index, roomNumber);
            return File.Exists(filePath);
        }

        public void DeleteRoom(int roomNumber)
        {
            File.Delete(ActorList.GetFilePath(_basePath, Map.Index, roomNumber));

            for (int layer = 0; layer < 8; layer++)
            {
                File.Delete(Layer.GetFilePath(_basePath, Map.Index, roomNumber, 1, layer));
                File.Delete(Layer.GetFilePath(_basePath, Map.Index, roomNumber, 2, layer));
            }
        }

        public int GetNextFreeRoom()
        {
            byte i = 0;
            while (true)
            {
                if (!RoomExists(i))
                {
                    return i;
                }
                if (i == byte.MaxValue)
                {
                    return -1;
                }
                i++;
            }
        }

        public void SaveLayers()
        {
            ReadOnlySpan<Layer> layers = Room.Layers;

            for (int i = 0; i < layers.Length; i++)
            {
                string layerPath = Layer.GetFilePath(_basePath, Map.Index, Room.Index, i > 7 ? 2 : 1, i % 8);
                using FileStream layerStream = File.Open(layerPath, FileMode.Create);
                layers[i].BinarySerialize(layerStream);
            }
            LayersAreDirty = false;
        }

        private void ReloadActors()
        {
            string actorListPath = ActorList.GetFilePath(_basePath, Map.Index, Room.Index);
            if (!File.Exists(actorListPath))
            {
                Room.Actors.Clear();
                _logger.AppendLine($"File not found: {actorListPath}");
            }
            else
            {
                using FileStream actorListStream = File.Open(actorListPath, FileMode.Open);
                Room.Actors.BinaryDeserialize(actorListStream);
            }
            ActorsAreDirty = false;
        }

        public void SaveActors()
        {
            string actorListPath = ActorList.GetFilePath(_basePath, Map.Index, Room.Index);
            using FileStream actorListStream = File.Open(actorListPath, FileMode.Create);
            Room.Actors.BinarySerialize(actorListStream);
            ActorsAreDirty = false;
        }

        public void ExportAsTMX(string filePath, string tilesetSource, string tvFilterSource = null)
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

            ReadOnlySpan<Layer> layers = Room.Layers;
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

        public void ImportRoomFromTMX(string filePath)
        {
            try
            {
                List<Tiled.Layer> layers = Tiled.ReadLayersFromTMX(filePath);

                LayersAreDirty = true;
                foreach (var layer in layers)
                {
                    if (layer.Size.Width > Layer.DIMENSION || layer.Size.Height > Layer.DIMENSION)
                    {
                        _logger.AppendLine($"Error: Layer ID {layer.ID} exceeds allowed dimensions. Layer Size: {layer.Size.Width}x{layer.Size.Height}, Max: {Layer.DIMENSION}x{Layer.DIMENSION}");
                        continue;
                    }
                    int layerID = layer.ID - 1;

                    int yIndex = layerID / 2;
                    int xIndex = layerID % 2;

                    if (yIndex < 8 && xIndex < 2)
                        Room.Layers[yIndex * xIndex].SetTiles(layer.Data, layer.Data.Length);
                    else
                        _logger.AppendLine($"Layer ID {layer.ID} is out of bounds.");
                }
            }
            catch (Exception ex)
            {
                _logger.AppendLine($"Critical error during TMX import: {ex.Message}");
            }
        }
    }
}
