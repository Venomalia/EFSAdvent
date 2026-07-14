using AuroraLib.Core.Format.Identifier;
using System;
using System.IO;
using static FSALib.Rarc;

namespace FSALib
{
    /// <summary>
    /// Represents a room in the game, which contains multiple layers and actors.
    /// </summary>
    public sealed class Room
    {
        public const int LAYER = 8, LAYERLEVEL = 2;

        private readonly Layer[] _layers = new Layer[LAYER * LAYERLEVEL];

        /// <summary>
        /// A collection of actors in the room (e.g., players, NPCs, enemies, etc.)
        /// </summary>
        public readonly ActorList Actors;

        /// <summary>
        /// The index of the room. This could be used to identify or reference the room in the game world.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the full set of layers in the room.
        /// </summary>
        public ReadOnlySpan<Layer> Layers => _layers.AsSpan();

        /// <summary>
        /// Gets the base layers of the room (Tile Layer 1, typically the background elements).
        /// These layers contain the background tiles like floors and walls.
        /// </summary>
        public ReadOnlySpan<Layer> BaseLayers => _layers.AsSpan(0, LAYER);

        /// <summary>
        /// Gets the top layers of the room (Tile Layer 2, the layers above the player).
        /// These layers are used for elements that should be drawn above the player.
        /// </summary>
        public ReadOnlySpan<Layer> TopLayers => _layers.AsSpan(LAYER);

        public Room()
        {
            Actors = new ActorList();
            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i] = new Layer();
            }
        }

        /// <summary>
        /// Loads the room actor data and all layer data from a RARC archive.
        /// </summary>
        /// <param name="root">The root directory of the RARC archive.</param>
        /// <param name="mapIndex">The stage map index.</param>
        /// <param name="roomIndex">The room index within the stage.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the room actor file could not be found in the archive.
        /// </exception>
        public void LoadFromRarc(DirectoryNode root, int mapIndex, int roomIndex)
        {
            DirectoryNode actorDirectory = root.Directorys[Rarc.CommonFolderTypes.BIN].Directorys[(Identifier32)$"B{mapIndex:D3}"];
            DirectoryNode layerDirectory = root.Directorys[Rarc.CommonFolderTypes.SZS].Directorys[(Identifier32)$"M{mapIndex:D3}"];

            string actorFileName = ActorList.GetFileName(mapIndex, roomIndex);

            if (!actorDirectory.TryGetFile(actorFileName, out FileNode actorFile))
                throw new ArgumentException($"The room actor file '{actorFileName}' was not found.");

            Actors.ReadFromStream(actorFile.Data);
            LoadLayers(layerDirectory, mapIndex, roomIndex);
        }

        internal void LoadLayers(DirectoryNode layerDirectory, int mapIndex, int roomIndex)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                int layerIndex = i % 8;
                int layerLevel = i / 8 + 1;
                string layerFileName = Layer.GetFileName(mapIndex, roomIndex, layerLevel, layerIndex);

                if (layerDirectory.TryGetFile(layerFileName, out FileNode layerFile))
                    Layers[i].ReadFromStream(layerFile.Data);
            }
        }

        /// <summary>
        /// Saves the room actor data and all layer data to a RARC archive.
        /// </summary>
        /// <param name="root">The root directory of the RARC archive.</param>
        /// <param name="mapIndex">The stage map index.</param>
        /// <param name="roomIndex">The room index within the stage.</param>
        public void SaveToRarc(DirectoryNode root, int mapIndex, int roomIndex)
        {
            var actorFolderkey = root.CreateDirectory(Rarc.CommonFolderTypes.BIN).CreateDirectory((Identifier32)$"B{mapIndex:D3}");
            var layerFolderkey = root.CreateDirectory(Rarc.CommonFolderTypes.SZS).CreateDirectory((Identifier32)$"M{mapIndex:D3}");

            string actorFileName = ActorList.GetFileName(mapIndex, roomIndex);
            var actorData = actorFolderkey.GetFile(actorFileName, FileMode.Create).Data;
            Actors.WriteToStream(actorData);

            SaveLayers(layerFolderkey, mapIndex, roomIndex);
        }

        internal void SaveLayers(DirectoryNode layerDirectory, int mapIndex, int roomIndex)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                int layerIndex = i % 8;
                int layerLevel = i / 8 + 1;
                string layerFileName = Layer.GetFileName(mapIndex, roomIndex, layerLevel, layerIndex);

                var layerFile = layerDirectory.GetFile(layerFileName, FileMode.Create);
                layerFile.Attribute |= FileAttribute.YAZ0_COMPRESSED;
                var layerData = layerFile.Data;
                layerData.SetLength(0);
                Layers[i].WriteToStream(layerData);
            }
        }

        /// <summary>
        /// Mirrors the room horizontally.
        /// </summary>
        public void Mirror()
        {
            foreach (var layer in _layers)
            {
                layer.Mirror();
            }
            Actors.Mirror();
        }
    }
}
