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

        public bool LoadRoom(byte roomNumber)
        {
            if (roomNumber == Map.EMPTY_ROOM_VALUE)
            {
                return false;
            }
            Room = new Room(_basePath, _number, roomNumber, _logger);
            return true;
        }

        public bool RoomExists(byte roomNumber)
        {
            string filePath = Path.Combine(_basePath, "szs", $"m{_number}", $"d_map{_number}_{roomNumber:D2}_mmm_1_0.szs");
            return File.Exists(filePath);
        }

        public void SaveLayers()
        {
            Room?.SaveLayers();
        }

        public void LoadActors()
        {
            Room.LoadActors(_basePath, _number);
        }

        public void SaveActors()
        {
            Room?.SaveActors(_basePath, _number);
        }
    }
}
