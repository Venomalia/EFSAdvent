using AuroraLib.Compression;
using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using System;
using System.IO;
using static FSALib.Rarc;

namespace FSALib
{
    /// <summary>
    /// Represents a complete FSA stage including its map layouts, rooms, and resource archive.
    /// </summary>
    public sealed class Stage : IDisposable, IStreamSerializable
    {

        private readonly Room?[] _rooms = new Room?[MapLayout.MAX_Rooms];
        private Rarc resources;

        /// <summary>
        /// Gets or sets the map / stage index.
        /// </summary>
        public int Index
        {
            get => Map.Index;
            set
            {
                Map.Index  = value;
                if (MapSingleplayer != null)
                MapSingleplayer.Index = value;
                if (!Resources.Directorys.ContainsKey(Rarc.CommonFolderTypes.Map)) // This stage only has this map, so we can adjust the root map / stage index.
                    Resources.Name = $"boss{value:D3}";
            }
        }

        /// <summary>
        /// Gets the primary map layout used by the stage.
        /// </summary>
        public MapLayout Map { get; private set; }

        /// <summary>
        /// Gets the single-player map layout, if available.
        /// </summary>
        public MapLayout? MapSingleplayer { get; private set; }
        // Does the game support other types of maps?


        /// <summary>
        /// Gets span of all rooms in this stage.
        /// </summary>
        public Span<Room?> Rooms => _rooms.AsSpan();

        /// <summary>
        /// Gets the root directory of the stage resource archive.
        /// </summary>
        public DirectoryNode Resources => resources.Root;

        public Stage(bool isShadowBattle = false)
        {
            Map = new MapLayout(isShadowBattle);
            MapSingleplayer = isShadowBattle ? null : new MapLayout();
            resources = new Rarc();
        }

        public Stage(Rarc source)
        {
            resources = source;
            Load();
        }

        public Stage(Stream source) : this()
            => ReadFromStream(source);

        /// <inheritdoc/>
        public void ReadFromStream(Stream source)
        {
            resources.Dispose();
            resources = new Rarc(source);
            Load();
        }

        /// <inheritdoc/>
        public void WriteToStream(Stream dest)
        {
            SaveToRarc(resources);
            resources.WriteToStream(dest, CompressionSettings.Maximum);
            dest.WriteAlign(0x10);
            Load(Index);
        }

        /// <summary>
        /// Writes the stage to disk.
        /// </summary>
        /// <param name="path">The destination directory path.</param>
        public void WriteToDirectory(string path)
        {
            SaveToRarc(resources);
            Resources.WriteToDirectory(path);
            Load(Index);
        }

        private void Load(int mapIndex = -1)
        {
            if (!Resources.Directorys.ContainsKey(Rarc.CommonFolderTypes.Map))
                throw new ArgumentException("Does not contain a stage map folder.");

            if (mapIndex == -1)
                mapIndex = int.Parse(Resources.Name.Substring(4, 3));

            // load maps
            DirectoryNode mapDirectory = Resources.Directorys[Rarc.CommonFolderTypes.Map];

            string mapFileName = MapLayout.GetFileName(mapIndex, false);
            string mapSingleplayerFileName = MapLayout.GetFileName(mapIndex, true);


            mapDirectory.TryGetFile(mapFileName, out FileNode? mapFile);
            if (!mapDirectory.TryGetFile(mapSingleplayerFileName, out FileNode MapSingleplayerFile))
            {
                if (mapFile == null)
                    throw new ArgumentException($"Does not contain a stage map {mapFileName} that can be loaded.");
                MapSingleplayerFile = mapFile;
            }
            else
            {
                mapFile ??= MapSingleplayerFile;
            }
            Map = new MapLayout(mapFile.Data);
            MapSingleplayer = Map.IsShadowBattle ? null : new MapLayout(MapSingleplayerFile.Data);

            // load rooms
            _rooms.AsSpan().Clear();
            var actorFolderkey = (Identifier32)$"B{mapIndex:D3}";
            var layerFolderkey = (Identifier32)$"M{mapIndex:D3}";

            DirectoryNode actorDirectory = Resources.Directorys[Rarc.CommonFolderTypes.BIN].Directorys[actorFolderkey];
            DirectoryNode layerDirectory = Resources.Directorys[Rarc.CommonFolderTypes.SZS].Directorys[layerFolderkey];

            foreach (var item in actorDirectory.Files)
            {
                int roomIndex = int.Parse(item.Name.Substring(15, 2));
                var room = new Room();
                _rooms[roomIndex] = room;
                room.Actors.ReadFromStream(item.Data);
                room.LoadLayers(layerDirectory, mapIndex, roomIndex);
            }

            // free maps
            mapDirectory.DeleteAndDisposeFile(mapFileName);
            mapDirectory.DeleteAndDisposeFile(mapSingleplayerFileName);
            // free rooms
            actorDirectory = Resources.Directorys[Rarc.CommonFolderTypes.BIN];
            layerDirectory = Resources.Directorys[Rarc.CommonFolderTypes.SZS];
            actorDirectory.DeleteAndDisposeDirectory(actorFolderkey);
            layerDirectory.DeleteAndDisposeDirectory(layerFolderkey);
            // cleanup
            Resources.RemoveEmptyEntries();
        }

        /// <summary>
        /// Loads the specified sub-stage from the resource archive.
        /// The current stage data is saved back to the archive before loading the new sub-stage.
        /// </summary>
        /// <param name="mapIndex"> The index of the sub-stage to load.</param>
        public void LoadSubStage(int mapIndex)
        {
            if (Index == mapIndex)
                return;

            if (!Resources.Directorys.ContainsKey(Rarc.CommonFolderTypes.Map))
                throw new ArgumentException("Does not contain a Sub stage.");

            SaveToRarc(resources);
            Load(mapIndex);
        }

        private void SaveToRarc(Rarc dest)
        {
            DirectoryNode root = dest.Root;
            int mapIndex = Index;

            // Save maps
            DirectoryNode mapDirectory = root.CreateDirectory(Rarc.CommonFolderTypes.Map);
            FileNode mapFile = mapDirectory.GetFile(MapLayout.GetFileName(Index, false), FileMode.Create);
            Map.WriteToStream(mapFile.Data);
            if (!Map.IsShadowBattle)
            {
                MapSingleplayer ??= new MapLayout();
                MapSingleplayer.Index = mapIndex;
                FileNode MapSingleplayerFile = mapDirectory.GetFile(MapLayout.GetFileName(Index, true), FileMode.Create);
                MapSingleplayer.WriteToStream(MapSingleplayerFile.Data);
            }

            // Save rooms
            for (int i = 0; i < _rooms.Length; i++)
            {
                var room = _rooms[i];
                room?.SaveToRarc(root, mapIndex, i);
            }
        }

        /// <summary>
        /// Gets the index of the first available room slot.
        /// </summary>
        /// <returns>
        /// The index of the first free room slot, or -1 if no room slot is available.
        /// </returns>
        public int GetNextFreeRoom()
        {
            for (int i = 0; i < _rooms.Length; i++)
            {
                if (_rooms[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Removes rooms that are not referenced by any stage map layout.
        /// </summary>
        public void RemoveUnusedRooms()
        {
            Span<bool> usedRooms = stackalloc bool[_rooms.Length];

            Map.MarkUsedRooms(usedRooms);
            MapSingleplayer.MarkUsedRooms(usedRooms);

            for (int i = 0; i < _rooms.Length; i++)
            {
                if (!usedRooms[i] && _rooms[i] != null)
                    _rooms[i] = null;
            }
        }

        public bool IsRoomInUse(int roomIndex) => Map.IsRoomInUse(roomIndex) || MapSingleplayer.IsRoomInUse(roomIndex);

        /// <inheritdoc/>
        public void Dispose() => resources.Dispose();

    }
}
