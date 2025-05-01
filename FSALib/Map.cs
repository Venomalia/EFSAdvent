using AuroraLib.Core;
using AuroraLib.Core.IO;
using System;
using System.IO;
using System.Text;

namespace FSALib
{
    /// <summary>
    /// Represents a map in fsa.
    /// </summary>
    public sealed class Map : MapProperties, IBinaryObject
    {
        public const int DIMENSION = 10, EMPTY_ROOM_VALUE = -1;

        private readonly int[] _rooms = new int[DIMENSION * DIMENSION];
        private readonly ShadowMapProperties[] _shadowRooms = new ShadowMapProperties[10];

        private int index;
        private int startX;
        private int startY;

        /// <summary>
        /// Gets the horizontal dimension of the map.
        /// If <see cref="IsShadowBattle"/> is <c>true</c>, the width is 5; otherwise, it is <see cref="DIMENSION"/>.
        /// </summary>
        public int XDimension => IsShadowBattle ? 5 : DIMENSION;

        /// <summary>
        /// Gets the vertical dimension of the map.
        /// If <see cref="IsShadowBattle"/> is <c>true</c>, the height is 2; otherwise, it is <see cref="DIMENSION"/>.
        /// </summary>
        public int YDimension => IsShadowBattle ? 2 : DIMENSION;

        /// <summary>
        /// Gets or sets the map index.
        /// </summary>
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value, nameof(Index));
        }

        /// <summary>
        /// Determines whether the map is a shadow battle map.
        /// </summary>
        public bool IsShadowBattle { get; private set; }

        /// <summary>
        /// Gets a read-only span of shadow battle rooms.
        /// If <see cref="IsShadowBattle"/> is <c>true</c>, returns the shadow room data; otherwise, returns an empty span.
        /// </summary>
        public ReadOnlySpan<ShadowMapProperties> ShadowRooms
            => IsShadowBattle ? _shadowRooms.AsSpan() : Span<ShadowMapProperties>.Empty;

        /// <summary>
        /// Gets or sets the starting <see cref="Room"/> X coordinate of the map.
        /// If <see cref="IsShadowBattle"/> is <c>true</c>, this is always 0.
        /// </summary>
        public int StartX
        {
            get => IsShadowBattle ? 0 : startX;
            set => OnPropertyChanged(ref startX, value, nameof(StartX));
        }

        /// <summary>
        /// Gets or sets the starting <see cref="Room"/> Y coordinate of the map.
        /// If <see cref="IsShadowBattle"/> is <c>true</c>, this is always 0.
        /// </summary>
        public int StartY
        {
            get => IsShadowBattle ? 0 : startY;
            set => OnPropertyChanged(ref startY, value, nameof(StartY));
        }

        /// <summary>
        /// Gets or sets the <see cref="Room"/> index at the specified (x, y) coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the <see cref="Room"/>.</param>
        /// <param name="y">The Y-coordinate of the <see cref="Room"/>.</param>
        /// <returns>The <see cref="Room"/> index at the given coordinates.</returns>
        public int this[int x, int y]
        {
            get => IsShadowBattle ? _shadowRooms[x + y * 5].RoomIndex : _rooms[x + y * DIMENSION];
            set
            {
                ThrowIf.LessThan(value, EMPTY_ROOM_VALUE, nameof(value));
                if (IsShadowBattle)
                    _shadowRooms[x + y * 5].RoomIndex = value;
                else
                    OnPropertyChanged(ref _rooms[x + y * DIMENSION], value, "Rooms");
            }
        }

        public Map(bool isShadowBattle = false)
        {
            IsShadowBattle = isShadowBattle;
            if (isShadowBattle)
            {
                index = 500;
                for (int i = 0; i < _shadowRooms.Length; i++)
                    _shadowRooms[i] = new ShadowMapProperties();
            }
            else
            {
                _rooms.AsSpan().Fill(EMPTY_ROOM_VALUE);
                index = 10;
            }
        }

        public Map(Stream source)
            => BinaryDeserialize(source);

        /// <inheritdoc/>
        public void BinaryDeserialize(Stream source)
        {
            using var file = new StreamReader(source);
            string? line = file.ReadLine();
            ThrowIf.Null(line, nameof(line));
            IsShadowBattle = !line.StartsWith("map");

            if (!IsShadowBattle)
            {
                string[] lineParts = line.Split(',');
                Index = int.Parse(lineParts[0].Substring(3));
                StartX = int.Parse(lineParts[1]);
                StartY = int.Parse(lineParts[2]);
                BackgroundMusicId = int.Parse(lineParts[3]);
                ShowE3Banner = int.Parse(lineParts[4]);
                TileSheetId = int.Parse(lineParts[5]);
                NPCSheetID = int.Parse(lineParts[6]);
                OverlayTextureId = int.Parse(lineParts[7]);
                Unknown = int.Parse(lineParts[8]);
                if (lineParts.Length >= 10 && int.TryParse(lineParts[9], out int value))
                    DisallowTingle = value;

                for (int y = 0; y < 10; y++)
                {
                    line = file.ReadLine();
                    ThrowIf.Null(line, nameof(line));
                    lineParts = line.Split(',');
                    for (int x = 0; x < 10; x++)
                    {
                        _rooms[x + y * DIMENSION] = lineParts[x] == "NULL" ? EMPTY_ROOM_VALUE : byte.Parse(lineParts[x]);
                    }
                }
            }
            else
            {
                if (_shadowRooms[0] == null)
                {
                    for (int i = 0; i < _shadowRooms.Length; i++)
                        _shadowRooms[i] = new ShadowMapProperties();
                }
                index = 500;
                ReadShadowRoomLine(_shadowRooms[0], line);
                for (int i = 1; i < _shadowRooms.Length; i++)
                {
                    line = file.ReadLine();
                    ThrowIf.Null(line, nameof(line));
                    ReadShadowRoomLine(_shadowRooms[i], line);
                }
            }

            static void ReadShadowRoomLine(ShadowMapProperties shadowMap, string line)
            {
                string[] lineParts = line.Split(',');
                shadowMap.RoomIndex = int.Parse(lineParts[0]);
                shadowMap.BackgroundMusicId = int.Parse(lineParts[1]);
                shadowMap.ShowE3Banner = int.Parse(lineParts[2]);
                shadowMap.TileSheetId = int.Parse(lineParts[3]);
                shadowMap.Unknown = int.Parse(lineParts[4]);
                shadowMap.NPCSheetID = int.Parse(lineParts[5]);
                shadowMap.OverlayTextureId = int.Parse(lineParts[6]);
                shadowMap.Unknown2 = int.Parse(lineParts[7]);
                shadowMap.DisallowTingle = int.Parse(lineParts[8]);
            }
        }

        /// <inheritdoc/>
        public void BinarySerialize(Stream dest)
        {
            using StreamWriter writer = new StreamWriter(dest);
            if (!IsShadowBattle)
            {
                writer.WriteLine($"map{Index},{StartX},{StartY},{BackgroundMusicId},{ShowE3Banner},{TileSheetId},{NPCSheetID},{OverlayTextureId},{Unknown},{DisallowTingle}");
                StringBuilder sb = new StringBuilder();
                for (int y = 0; y < DIMENSION; y++)
                {
                    sb.Clear();
                    sb.Append(GetRoomString(_rooms[y * DIMENSION]));
                    for (int x = 1; x < DIMENSION; x++)
                    {
                        sb.Append(',');
                        sb.Append(GetRoomString(_rooms[x + y * DIMENSION]));
                    }
                    writer.WriteLine(sb);
                }
                writer.WriteLine("end,,,,");
            }
            else
            {
                for (int i = 0; i < DIMENSION; i++)
                {
                    ShadowMapProperties room = _shadowRooms[i];
                    writer.WriteLine($"{room.RoomIndex},{room.BackgroundMusicId},{room.ShowE3Banner},{room.TileSheetId},{room.Unknown},{room.NPCSheetID},{room.OverlayTextureId},{room.Unknown},{room.DisallowTingle}");
                }
                writer.WriteLine("end,,,,,,,,");
            }

            static string GetRoomString(int current) => current == EMPTY_ROOM_VALUE ? "NULL" : current.ToString();
        }

        /// <summary>
        /// Finds the position of a room within the map grid based on its index.
        /// </summary>
        /// <param name="roomIndex">The index of the <see cref="Room"/> to locate.</param>
        /// <returns>
        /// A <see cref="ValueTuple"/> (int x, int y) representing the room's position within the grid.  
        /// Returns (<see cref="EMPTY_ROOM_VALUE"/>, <see cref="EMPTY_ROOM_VALUE"/>) if the room is not found.
        /// </returns>
        public (int x, int y) FindRoomPositon(int roomIndex)
        {
            if (roomIndex != EMPTY_ROOM_VALUE)
            {
                for (int y = 0; y < YDimension; y++)
                {
                    for (int x = 0; x < XDimension; x++)
                    {
                        if (this[x, y] == roomIndex)
                        {
                            return (x, y);
                        }
                    }
                }
            }

            return (EMPTY_ROOM_VALUE, EMPTY_ROOM_VALUE);
        }

        /// <summary>
        /// Determines whether a specified <see cref="Room.Index"/> is currently in use within the map.
        /// </summary>
        /// <param name="roomIndex">The index of the <see cref="Room"/> to check.</param>
        /// <returns><c>true</c> if the <see cref="Room"/> is in use; otherwise, <c>false</c>.</returns>
        public bool IsRoomInUse(int roomIndex)
            => FindRoomPositon(roomIndex).x != EMPTY_ROOM_VALUE;

        /// <summary>
        /// Retrieves the properties of a specified <see cref="Room"/>.
        /// If <see cref="IsShadowBattle"/> is true, it searches for the room in the <see cref="ShadowRooms"/> span.
        /// Otherwise, it returns the properties of the map.
        /// </summary>
        /// <param name="roomIndex">The <see cref="Room"/> index whose properties are requested.</param>
        /// <returns>
        /// The <see cref="MapProperties"/> of the specified room.
        /// If the room is not found in a shadow battle, the first shadow room is returned as a fallback.
        /// </returns>
        public MapProperties GetRoomProperties(int roomIndex)
        {
            if (IsShadowBattle)
            {
                for (int i = 0; i < _shadowRooms.Length; i++)
                {
                    if (_shadowRooms[i].RoomIndex == roomIndex)
                    {
                        return _shadowRooms[i];
                    }
                }
                return _shadowRooms[0];
            }
            else
            {
                return this;
            }
        }
    }
}
