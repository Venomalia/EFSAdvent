using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EFSAdvent.FourSwords
{
    public class Map
    {
        public const byte EMPTY_ROOM_VALUE = 0xFF;
        public const int DIMENSION = 10;

        private byte[,] _data;
        public int XDimension { get; private set; }
        public int YDimension { get; private set; }
        private readonly string _path;
        private readonly Logger _logger;

        public string Name { get; private set; }
        public string Number
        {
            get { return new String(Name.Skip(3).ToArray()); }
        }

        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int BackgroundMusicId { get; private set; }
        public int ShowE3Banner { get; private set; }
        public int TileSheetId { get; private set; }
        public int NPCSheetID { get; private set; }
        public int OverlayTextureId { get; private set; }
        public int Unknown2 { get; private set; }
        public int DisallowTingle { get; private set; }

        public bool IsShadowBattle { get; private set; }
        public bool IsDirty { get; private set; }

        public Map(string path, Logger logger)
        {
            _logger = logger;
            _path = path;
            _logger.Clear();
            if (path.EndsWith("map500_ctrl.csv"))
            {
                _path = path.Replace("map500_ctrl.csv", "map500.csv");
                _logger.AppendText("Unknown what map500_ctrl.csv is, loading map500.csv instead.");
            }

            if (!File.Exists(path))
            {
                NewMap(path);
                return;
            }

            var file = File.OpenText(path);
            string firstLine = file.ReadLine();
            IsShadowBattle = !firstLine.StartsWith("map");
            if (IsShadowBattle)
            {
                file.Close();
                LoadShadowBattleMap();
                return;
            }
            string[] firstLineParts = firstLine.Split(',');

            Name = firstLineParts[0];

            StartX = int.Parse(firstLineParts[1]);
            StartY = int.Parse(firstLineParts[2]);
            BackgroundMusicId = int.Parse(firstLineParts[3]);
            ShowE3Banner = int.Parse(firstLineParts[4]);
            TileSheetId = int.Parse(firstLineParts[5]);
            NPCSheetID = int.Parse(firstLineParts[6]);
            OverlayTextureId = int.Parse(firstLineParts[7]);
            Unknown2 = int.Parse(firstLineParts[8]);
            if (firstLineParts.Length >= 10 && int.TryParse(firstLineParts[9], out int value))
            {
                DisallowTingle = value;
            }

            _data = new byte[DIMENSION, DIMENSION];
            XDimension = YDimension = DIMENSION;

            for (int y = 0; y < 10; y++)
            {
                string line = file.ReadLine();
                string[] lineParts = line.Split(',');
                for (int x = 0; x < 10; x++)
                {
                    _data[y, x] = lineParts[x] == "NULL"
                        ? EMPTY_ROOM_VALUE
                        : byte.Parse(lineParts[x]);
                }
            }

            file.Close();
        }

        private void NewMap(string path)
        {
            Name = Path.GetFileNameWithoutExtension(path).Split('_').First();
            IsShadowBattle = false;

            XDimension = YDimension = DIMENSION;
            _data = new byte[DIMENSION, DIMENSION];
            for (int x = 0; x < DIMENSION; x++)
            {
                for (int y = 0; y < DIMENSION; y++)
                {
                    _data[x, y] = EMPTY_ROOM_VALUE;
                }
            }
            Unknown2 = OverlayTextureId = NPCSheetID = TileSheetId = ShowE3Banner = BackgroundMusicId = StartY = StartX = 0;
        }

        // boss500 has a different format
        private void LoadShadowBattleMap()
        {
            Name = Path.GetFileNameWithoutExtension(_path).Split('_').First();
            using (var file = new StreamReader(_path))
            {
                var lines = new List<string>();
                string line;
                while (!(line = file.ReadLine()).StartsWith("end"))
                {
                    lines.Add(line);
                }
                XDimension = 1;
                YDimension = lines.Count;
                _data = new byte[YDimension, XDimension];

                for (int y = 0; y < lines.Count; y++)
                {
                    var parts = lines[y].Split(',');
                    _data[y, 0] = byte.Parse(parts[0]);
                }
            }
        }

        public void LoadShadowBattleRoom(int room)
        {
            IsDirty = false;
            using (var file = new StreamReader(_path))
                for (int y = 0; y < YDimension; y++)
                {
                    string firstLine = file.ReadLine();
                    if (_data[y, 0] == room)
                    {
                        string[] firstLineParts = firstLine.Split(',');

                        BackgroundMusicId = int.Parse(firstLineParts[1]);
                        ShowE3Banner = int.Parse(firstLineParts[2]);
                        TileSheetId = int.Parse(firstLineParts[3]);
                        //Value 4 is unknown but always zero.
                        NPCSheetID = int.Parse(firstLineParts[5]);
                        OverlayTextureId = int.Parse(firstLineParts[6]);
                        Unknown2 = int.Parse(firstLineParts[7]);
                        DisallowTingle = int.Parse(firstLineParts[8]);
                        return;
                    }
                }
            _logger.AppendLine($"Room {room} not found in the file.");
        }

        public void SaveShadowBattleRoom(int room)
        {
            var lines = File.ReadAllLines(_path);
            for (int y = 0; y < YDimension; y++)
            {
                if (_data[y, 0] == room)
                {
                    var firstLineParts = lines[y].Split(',');
                    firstLineParts[0] = _data[y, 0].ToString();
                    firstLineParts[1] = BackgroundMusicId.ToString();
                    firstLineParts[2] = ShowE3Banner.ToString();
                    firstLineParts[3] = TileSheetId.ToString();
                    //Value 4 is unknown but always zero.
                    firstLineParts[5] = NPCSheetID.ToString();
                    firstLineParts[6] = OverlayTextureId.ToString();
                    firstLineParts[7] = Unknown2.ToString();
                    firstLineParts[8] = DisallowTingle.ToString();
                    lines[y] = string.Join(",", firstLineParts);

                    File.WriteAllLines(_path, lines);
                    return;
                }
            }

            _logger.AppendLine($"Room {room} not found in the file.");
        }

        public void Save()
        {
            if (IsShadowBattle)
            {
                return;
            }

            const byte COMMA = 0x2C;
            byte[] NEWLINE = new byte[] { 0x0D, 0x0A };
            var csvVariables = new string[]
            {
                StartX.ToString(),
                StartY.ToString(),
                BackgroundMusicId.ToString(),
                ShowE3Banner.ToString(),
                TileSheetId.ToString(),
                NPCSheetID.ToString(),
                OverlayTextureId.ToString(),
                Unknown2.ToString(),
                DisallowTingle.ToString()
            };
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            var csvStream = new FileStream(_path, FileMode.Create);

            csvStream.Write(Encoding.ASCII.GetBytes(Name), 0, Name.Length);

            foreach (string variable in csvVariables)
            {
                csvStream.WriteByte(COMMA);
                csvStream.Write(Encoding.ASCII.GetBytes(variable), 0, variable.Length);
            }

            csvStream.Write(NEWLINE, 0, NEWLINE.Length);

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (x != 0)
                    {
                        csvStream.WriteByte(COMMA);
                    }

                    if (_data[y, x] == EMPTY_ROOM_VALUE)
                    {
                        csvStream.Write(Encoding.ASCII.GetBytes("NULL"), 0, 4);
                    }
                    else
                    {
                        string value = Convert.ToString(_data[y, x]);
                        csvStream.Write(Encoding.ASCII.GetBytes(value), 0, value.Length);
                    }
                }
                csvStream.Write(NEWLINE, 0, NEWLINE.Length);
            }

            string endOfFile = "end,,,,";
            byte[] endOfFileBytes = Encoding.ASCII.GetBytes(endOfFile).Concat(NEWLINE).ToArray();
            csvStream.Write(endOfFileBytes, 0, endOfFileBytes.Length);

            //Finally it's over ^-^
            csvStream.Flush();
            csvStream.Close();

            IsDirty = false;
        }

        public byte GetRoomValue(int x, int y)
        {
            if (x < 0 || x >= XDimension || y < 0 || y >= YDimension)
            {
                throw new ArgumentException($"{nameof(GetRoomValue)}: Invalid map coordinates ({x},{y})");
            }

            return _data[y, x];
        }

        public (int, int) GetRoomCoordinates(byte roomValue)
        {
            for (int y = 0; y < YDimension; y++)
            {
                for (int x = 0; x < XDimension; x++)
                {
                    if (_data[y, x] == roomValue)
                    {
                        return (x, y);
                    }
                }
            }
            throw new ArgumentException($"{nameof(GetRoomCoordinates)}: Map does not contain room value {roomValue}");
        }

        public bool IsRoomInUse(int RoomID)
        {
            for (int x = 0; x < XDimension; x++)
            {
                for (int y = 0; y < YDimension; y++)
                {
                    if (RoomID == _data[y, x])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetRoomValue(int x, int y, byte newValue)
        {
            if (x < 0 || x > XDimension || y < 0 || y > YDimension)
            {
                throw new ArgumentException($"{nameof(GetRoomValue)}: Invalid map coordinates ({x},{y})");
            }

            _data[y, x] = newValue;
            IsDirty = true;
        }

        public void SetVariables(int startX, int startY, int backgroundMusicId, int showE3Banner,
                int tileSetId, int nPCSheetID, int overlayTextureId, int unknown2, int disallowTingle)
        {
            StartX = startX;
            StartY = startY;
            BackgroundMusicId = backgroundMusicId;
            ShowE3Banner = showE3Banner;
            TileSheetId = tileSetId;
            NPCSheetID = nPCSheetID;
            OverlayTextureId = overlayTextureId;
            Unknown2 = unknown2;
            DisallowTingle = disallowTingle;

            IsDirty = true;
        }
    }
}
