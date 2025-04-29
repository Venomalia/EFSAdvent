using AuroraLib.Core.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FSALib
{
    /// <summary>
    /// Represents a tile stamp (SMP) containing a grid of tiles.
    /// </summary>
    public sealed class Stamp : IBinaryObject
    {
        private readonly ushort[] _tiles = new ushort[Layer.TILES];

        /// <summary>
        /// Gets the width of the stamp (X dimension).
        /// </summary>
        public int Width { get; private set; } = 1;

        /// <summary>
        /// Gets the height of the stamp (Y dimension).
        /// </summary>
        public int Height { get; private set; } = 1;

        /// <summary>
        /// Gets the tile data for this stamp as a span.
        /// </summary>
        public Span<ushort> Tiles => _tiles;

        /// <summary>
        /// Sets the width and height of the stamp.
        /// </summary>
        /// <param name="width">The width (X dimension) to set.</param>
        /// <param name="height">The height (Y dimension) to set.</param>
        public void SetWidthAndHeight(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Copies a rectangular region of tiles from the specified <see cref="Layer"/> into this stamp.
        /// </summary>
        /// <param name="layer">The layer from which to copy tiles.</param>
        /// <param name="x">The x-coordinate of the top-left corner of the region.</param>
        /// <param name="y">The y-coordinate of the top-left corner of the region.</param>
        /// <param name="width">The width of the region in tiles.</param>
        /// <param name="height">The height of the region in tiles.</param>
        public void FromLayer(Layer layer, int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || width < 1 || height < 1 || x + width > Layer.DIMENSION || y + height > Layer.DIMENSION)
            {
                throw new ArgumentOutOfRangeException("The specified region is out of bounds.");
            }

            Span<ushort> tiles = _tiles.AsSpan();
            ReadOnlySpan<ushort> layerTiles = layer.Tiles;
            tiles.Clear();

            if (width == Layer.DIMENSION)
            {
                int length = height * Layer.DIMENSION;
                layerTiles.Slice(y * Layer.DIMENSION, length).CopyTo(tiles);
            }
            else
            {
                for (int iy = 0; iy < height; iy++)
                {
                    int stampY = iy * Layer.DIMENSION;
                    int layerY = (y + iy) * Layer.DIMENSION;

                    for (int ix = 0; ix < width; ix++)
                    {
                        tiles[stampY + ix] = layerTiles[layerY + (x + ix)];
                    }
                }
            }

            Width = width;
            Height = height;
        }

        /// <inheritdoc/>
        public void BinaryDeserialize(Stream source)
        {
            Span<byte> bytes = MemoryMarshal.Cast<ushort, byte>(_tiles);
            bytes.Clear();

            int needed = (int)source.Length == 0x200 ? 0x200 : Layer.TILES * 2;
            if (source.Length > 0x200) // Is 32*32
            {
                source.ReadExactly(bytes);
            }
            else  // Is 16*16 
            {
                for (int y = 0; y < 16; y++)
                {
                    source.ReadExactly(bytes.Slice(y*0x40, 0x20));
                }
            }

            Width = Layer.DIMENSION;
            for (int x = 1; x < Layer.DIMENSION; x++)
            {
                if (_tiles[x] == 0)
                {
                    Width = x;
                    break;
                }
            }

            Height = Layer.DIMENSION;
            for (int y = 1; y < Layer.DIMENSION; y++)
            {
                if (_tiles[y * Layer.DIMENSION] == 0)
                {
                    Height = y;
                    break;
                }
            }
        }

        /// <inheritdoc/>
        public void BinarySerialize(Stream dest)
        {
            Span<byte> bytes = MemoryMarshal.Cast<ushort, byte>(_tiles);
            if (Height > 16 || Width > 16) // Is 32*32
            {
                dest.Write(bytes);
            }
            else // Is 16*16 
            {
                for (int y = 0; y < 16; y++)
                {
                    dest.Write(bytes.Slice(y * 0x40, 0x20));
                }
            }
        }

        /// <summary>
        /// Mirrors the tiles horizontally.
        /// </summary>
        public void Mirror()
        {
            ReadOnlySpan<ushort> mirrorLOT = Assets.MirrorTileLOT;
            Span<ushort> tiles = _tiles;

            for (int y = 0; y < Height; y++)
            {
                int line = y * Layer.DIMENSION;
                int halfWidth = Width / 2;

                // Mirror left and right sides
                for (int x = 0; x < halfWidth; x++)
                {
                    int leftIndex = line + x;
                    int rightIndex = line + (Width - 1 - x);

                    ushort left = mirrorLOT[tiles[rightIndex]];
                    ushort right = mirrorLOT[tiles[leftIndex]];

                    tiles[leftIndex] = left;
                    tiles[rightIndex] = right;
                }

                // If width is odd, mirror the center tile
                if ((Width & 1) != 0)
                {
                    int middleIndex = line + halfWidth;
                    tiles[middleIndex] = mirrorLOT[tiles[middleIndex]];
                }
            }
        }
    }
}
