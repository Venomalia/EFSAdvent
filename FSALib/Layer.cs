using AuroraLib.Compression.Algorithms;
using AuroraLib.Core;
using AuroraLib.Core.IO;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace FSALib
{
    /// <summary>
    /// Represents a tile-based layer in FSA game environment.
    /// </summary>
    public sealed class Layer : IBinaryObject, INotifyPropertyChanged
    {
        public const int DIMENSION = 32;
        public const int TILES = DIMENSION * DIMENSION;

        private readonly ushort[] _tiles = new ushort[TILES];
        private bool isEmpty;

        /// <summary>
        /// Gets the tile data for this layer as a read-only span.
        /// </summary>
        public ReadOnlySpan<ushort> Tiles => _tiles;

        /// <summary>
        /// Indicates whether the layer is empty (contains no tiles).
        /// </summary>
        public bool IsEmpty
        {
            get => isEmpty;
            private set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    PropertyChanged?.Invoke(this, propertyChangedEventArgs_IsEmpty);
                }
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        public Layer()
            => IsEmpty = true;

        /// <summary>
        /// Gets or sets the tile at the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the tile.</param>
        /// <param name="y">The Y-coordinate of the tile.</param>
        /// <returns>The tile value at the given position.</returns>
        public ushort this[int x, int y]
        {
            get => _tiles[(y * DIMENSION) + x];
            set
            {
                ushort oldTile = _tiles[(y * DIMENSION) + x];

                // Do we need to update the value?
                if (oldTile == value)
                    return;

                // Update
                _tiles[(y * DIMENSION) + x] = value;
                // Is this layer empty?
                if (value != 0)
                    IsEmpty = false;
                else if (oldTile != 0)
                    IsEmpty = AreTilesEmpty(_tiles);

                // PropertyChanged
                PropertyChanged?.Invoke(this, propertyChangedEventArgs_Tiles);
            }
        }

        /// <summary>
        /// Replaces tile values starting from the specified index.
        /// </summary>
        /// <param name="tiles">The new tile data to set.</param>
        /// <param name="startIndex">The starting index in the tile array.</param>
        public void SetTiles(ReadOnlySpan<ushort> tiles, int startIndex)
        {
            tiles.CopyTo(_tiles.AsSpan(startIndex));

            // Is this layer empty?
            IsEmpty = AreTilesEmpty(_tiles);

            // PropertyChanged
            PropertyChanged?.Invoke(this, propertyChangedEventArgs_Tiles);
        }

        /// <summary>
        /// Copies tiles from a <see cref="Stamp"/> into the layer at the specified position.
        /// </summary>
        /// <param name="stamp">The source stamp to copy tiles from.</param>
        /// <param name="x">The x-coordinate in the layer where the stamp will be placed.</param>
        /// <param name="y">The y-coordinate in the layer where the stamp will be placed.</param>
        public void SetTiles(Stamp stamp, int x, int y)
        {
            int width = Math.Min(stamp.Width, DIMENSION - x);
            int height = Math.Min(stamp.Height, DIMENSION - y);
            ReadOnlySpan<ushort> copyTiles = stamp.Tiles;

            if (width == DIMENSION)
            {
                int length = height * DIMENSION;
                copyTiles.Slice(0, length).CopyTo(_tiles.AsSpan(y * DIMENSION, length));
            }
            else
            {
                for (int cY = 0; cY < height; cY++)
                {
                    int startY = (y + cY) * DIMENSION;
                    int stampStartY = cY * DIMENSION;

                    for (int cX = 0; cX < width; cX++)
                    {
                        _tiles[startY + x + cX] = copyTiles[stampStartY + cX];
                    }
                }
            }

            // Is this layer empty?
            IsEmpty = AreTilesEmpty(_tiles);

            // PropertyChanged
            PropertyChanged?.Invoke(this, propertyChangedEventArgs_Tiles);
        }

        /// <summary>
        /// Clears all tiles in the layer, setting them to zero.
        /// </summary>
        public void Clear()
        {
            if (IsEmpty)
                return;

            _tiles.AsSpan().Clear();
            IsEmpty = true;
            PropertyChanged?.Invoke(this, propertyChangedEventArgs_Tiles);
        }

        /// <inheritdoc/>
        public void BinaryDeserialize(Stream source)
        {
            byte[] data = new byte[TILES * 2];

            if (Common.Yaz0.IsMatch(source))
            {
                Common.Yaz0.Decompress(source, new MemoryStream(data));
            }
            else
            {
                source.ReadExactly(data, 0, data.Length);
            }
            ReadFromSzsFormat(MemoryMarshal.Cast<byte, ushort>(data), _tiles);
            IsEmpty = AreTilesEmpty(_tiles);
        }

        /// <inheritdoc/>
        public void BinarySerialize(Stream dest) => BinarySerialize(dest, CompressionLevel.Optimal);

        public void BinarySerialize(Stream dest, CompressionLevel level)
        {
            byte[] data = new byte[TILES * 2];
            WriteSzsFormat(_tiles, MemoryMarshal.Cast<byte, ushort>(data));
            Common.Yaz0.Compress(new MemoryStream(data), dest, level);
        }

        /// <summary>
        /// Converts tile data from SZS format to the internal tile layout.
        /// </summary>
        /// <param name="source">The source tile data in SZS format.</param>
        /// <param name="destination">The destination buffer where the converted tile data will be stored.</param>
        public static void ReadFromSzsFormat(ReadOnlySpan<ushort> source, Span<ushort> destination)
        {
            ThrowIf.LessThan(source.Length, TILES, nameof(source));
            ThrowIf.LessThan(destination.Length, TILES, nameof(destination));

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    destination[(y * DIMENSION) + x] = source[(y * 16) + x];
                    destination[(y * DIMENSION) + x + 16] = source[0x100 + (y * 16) + x];
                    destination[((y + 16) * DIMENSION) + x] = source[0x200 + (y * 16) + x];
                    destination[((y + 16) * DIMENSION) + x + 16] = source[0x300 + (y * 16) + x];
                }
            }
        }

        /// <summary>
        /// Converts tile data from the internal layout to SZS format.
        /// </summary>
        /// <param name="source">The source tile data in the internal format.</param>
        /// <param name="destination">The destination buffer where the SZS-formatted tile data will be stored.</param>
        public static void WriteSzsFormat(ReadOnlySpan<ushort> source, Span<ushort> destination)
        {
            ThrowIf.LessThan(source.Length, TILES, nameof(source));
            ThrowIf.LessThan(destination.Length, TILES, nameof(destination));

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    destination[(y * 16) + x] = source[(y * DIMENSION) + x];
                    destination[0x100 + (y * 16) + x] = source[(y * DIMENSION) + x + 16];
                    destination[0x200 + (y * 16) + x] = source[((y + 16) * DIMENSION) + x];
                    destination[0x300 + (y * 16) + x] = source[((y + 16) * DIMENSION) + x + 16];
                }
            }
        }

        private static bool AreTilesEmpty(ReadOnlySpan<ushort> tiles)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the folder path where SZS map data is stored for a specific map index.
        /// </summary>
        /// <param name="path">The base directory path.</param>
        /// <param name="mapIndex">The index of the map.</param>
        /// <returns>The full folder path for the specified map.</returns>
        public static string GetFolder(string path, int mapIndex)
            => Path.Combine(path, "szs", $"m{mapIndex:D3}");

        /// <summary>
        /// Generates the filename for an SZS map file based on the map, room, and layer information.
        /// </summary>
        /// <param name="mapIndex">The index of the map.</param>
        /// <param name="roomIndex">The index of the room within the map.</param>
        /// <param name="layerLevel">The layer level 1 for base and 2 for top layer.</param>
        /// <param name="layer">The specific layer index (default is 0).</param>
        /// <returns>The formatted filename for the SZS map file.</returns>
        public static string GetFileName(int mapIndex, int roomIndex, int layerLevel = 1, int layer = 0)
            => $"d_map{mapIndex:D3}_{roomIndex:D2}_mmm_{layerLevel}_{layer}.szs";

        /// <summary>
        /// Constructs the full file path for an SZS map file, combining the folder path and filename.
        /// </summary>
        /// <param name="path">The base directory path.</param>
        /// <param name="mapIndex">The index of the map.</param>
        /// <param name="roomIndex">The index of the room within the map.</param>
        /// <param name="layerLevel">The layer level 1 for base and 2 for top layer.</param>
        /// <param name="layer">The specific layer index (default is 0).</param>
        /// <returns>The full file path to the SZS map file.</returns>
        public static string GetFilePath(string path, int mapIndex, int roomIndex, int layerLevel = 1, int layer = 0)
            => Path.Combine(GetFolder(path, mapIndex), GetFileName(mapIndex, roomIndex, layerLevel, layer));

        private static readonly PropertyChangedEventArgs propertyChangedEventArgs_Tiles = new PropertyChangedEventArgs(nameof(Tiles));
        private static readonly PropertyChangedEventArgs propertyChangedEventArgs_IsEmpty = new PropertyChangedEventArgs(nameof(IsEmpty));
    }
}
