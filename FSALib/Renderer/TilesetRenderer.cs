using AuroraLib.Core.IO;
using AuroraLib.Pixel;
using AuroraLib.Pixel.Image;
using FSALib.Structs;
using System;
using System.IO;

namespace FSALib.Renderer
{
    /// <summary>
    /// Represents a 2D tile-based graphics set composed of 16×16 pixel tiles, each built from four 8×8 parts, using indexed color palettes.
    /// </summary>
    /// <typeparam name="TColor">The color type implementing <see cref="IColor{T}"/> used for rendering.</typeparam>
    public sealed class TilesetRenderer<TColor> : GBA4bppTileRenderer<TColor> where TColor : unmanaged, IColor<TColor>
    {
        private const int TileSize = PartSize + PartSize;

        /// <summary>
        /// The full list of 16×16 pixel tiles, where each tile is composed of four parts.
        /// </summary>
        public readonly Tile[] Tiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="TilesetRenderer{TColor}"/> class.
        /// Loads base <see cref="Tile"/> layout (SPL) and static part indices (SCH).
        /// </summary>
        /// <param name="bgaSpl">Stream containing the <see cref="Tile"/> layout data (SPL).</param>
        /// <param name="bgBaseSch">Stream containing the initial part indices (SCH).</param>
        public TilesetRenderer(Stream bgaSpl, Stream bgBaseSch) : base(new TColor[0x100], new byte[0x8000])
        {
            Tiles = new Tile[0x400];
            bgaSpl.Read<Tile>(Tiles);
            bgBaseSch.Read(IndexSpritSheet, 0x6000, 0x1000);
        }

        /// <summary>
        /// Loads part index data (SCH) from a stream.
        /// </summary>
        /// <param name="setSCH">Stream containing the SCH data to load.</param>
        public void LoadTiles(Stream setSCH)
            => setSCH.Read(IndexSpritSheet, 0, 0x6000);

        /// <summary>
        /// Renders the entire tileset onto the target image surface.
        /// Tiles are arranged in a 16×64 grid (256×1024 pixels).
        /// </summary>
        /// <param name="target">The image surface to draw on. Must be at least 256×1024 pixels.</param>
        public void Draw(IImage<TColor> target)
        {
            if (target.Width < 256 || target.Height < 1024)
                throw new ArgumentException(null, nameof(target));

            for (ushort i = 0; i < Tiles.Length; i++)
            {
                int x = (i & 0xf) * TileSize;
                int y = (i >> 4) * TileSize;
                DrawTile(target, x, y, i);
            }
        }

        /// <summary>
        /// Renders a single tile composed of 4 parts at the given position.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="x">Top-left X coordinate in the image.</param>
        /// <param name="y">Top-left Y coordinate in the image.</param>
        /// <param name="tileIndex">Index of the tile to draw.</param>
        public void DrawTile(IImage<TColor> target, int x, int y, ushort tileIndex)
        {
            ReadOnlySpan<Part> parts = Tiles[tileIndex].Parts;
            DrawPart(target, x, y, parts[0]); // TopLeft
            DrawPart(target, x + PartSize, y, parts[1]); // TopRight
            DrawPart(target, x, y + PartSize, parts[2]); // BottomLeft
            DrawPart(target, x + PartSize, y + PartSize, parts[3]); // BottomRight
        }

        /// <summary>
        /// Renders a single 8×8 part with optional horizontal and vertical mirroring.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="x">Top-left X coordinate in the image.</param>
        /// <param name="y">Top-left Y coordinate in the image.</param>
        /// <param name="part">The tile part to render, containing palette and flipping info.</param>
        public void DrawPart(IImage<TColor> target, int x, int y, Part part)
        {
            ReadOnlySpan<byte> pIndex = GetPartIndices(part.Index);
            ReadOnlySpan<TColor> palette = GetPalette(part.PaletteIndex);
            DrawPart(target, x, y, pIndex, palette, part.Flip);
        }
    }
}
