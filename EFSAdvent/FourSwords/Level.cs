using System;
using System.IO;
using System.Linq;

namespace EFSAdvent.FourSwords
{
    public class Level
    {
        public Map Map { get; private set; }
        public Room Room { get; private set; }

        public int TileSheetId { get { return Map.TileSheetId; } }

        public bool IsDirty { get { return MapIsDirty || LayersAreDirty || ActorsAreDirty; } }
        public bool ActorsAreDirty { get { return Room?.ActorsAreDirty ?? false; } }
        public bool LayersAreDirty { get { return Room?.LayersAreDirty ?? false; } }
        public bool MapIsDirty { get { return Map?.IsDirty ?? false; } }

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
        }

        public void LoadMap()
        {
            Map = new Map(_mapPath, _logger);
        }

        public void SaveMap()
        {
            Map.Save();
        }

        public bool LoadRoom(byte roomNumber, bool newRoom = false)
        {
            if (roomNumber == Map.EMPTY_ROOM_VALUE)
            {
                return false;
            }
            Room = new Room(_basePath, _number, roomNumber, _logger, newRoom);
            return true;
        }

        public bool RoomExists(int roomNumber)
        {
            //d_map031_14_mmm_1_0.szs
            string filePath = Room.GetLayerFilePath(_basePath, _number, roomNumber);
            return File.Exists(filePath);
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
            Room?.SaveLayers(_basePath, _number);
        }

        public void ReloadActors()
        {
            Room.ReloadActors(_basePath, _number);
        }

        public void SaveActors()
        {
            Room?.SaveActors(_basePath, _number);
        }
    }
}
