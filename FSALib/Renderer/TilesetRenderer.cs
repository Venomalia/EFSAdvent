using AuroraLib.Core.IO;
using AuroraLib.Pixel;
using AuroraLib.Pixel.Image;
using AuroraLib.Pixel.Processing;
using FSALib.Structs;
using System;
using System.Drawing;
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
        /// Initializes a new instance of the <see cref="TilesetRenderer{TColor}"/> class
        /// using tile resources from a RARC archive.
        /// </summary>
        /// <param name="data">The RARC archive containing the tile layout and sprite sheet resources.</param>
        public TilesetRenderer(Rarc data) : this(data.Root.Directorys[Rarc.CommonFolderTypes.SPL].GetFile("bg_a_spl.szs").Data, data.Root.Directorys[Rarc.CommonFolderTypes.SCH].GetFile("bg_base_sch.szs").Data)
        {
        }

        /// <summary>
        /// Loads a tileset palette and tile data from a RARC archive.
        /// </summary>
        /// <param name="data">The RARC archive containing the tileset resources.</param>
        /// <param name="index">The index of the tileset to load.</param>
        /// <param name="GBA">Specifies whether to load the GBA variant of the tileset.</param>
        public void LoadTileset(Rarc data, int index, bool GBA = false)
        {
            string paletteFileName = Assets.Tilesets[index].GetPaletteFileName(GBA);
            var palette = data.Root.Directorys[Rarc.CommonFolderTypes.SCL].GetFile(paletteFileName);
            LoadPalettes(palette.Data);

            string fileName = Assets.Tilesets[index].GetFileName(GBA);
            var file = data.Root.Directorys[Rarc.CommonFolderTypes.SCH].GetFile(fileName);
            LoadTiles(file.Data);
        }

        /// <summary>
        /// Loads part index data (SCH) from a stream.
        /// </summary>
        /// <param name="setSCH">Stream containing the SCH data to load.</param>
        public void LoadTiles(Stream setSCH)
            => setSCH.Read(IndexSpritSheet, 0, 0x6000);

        /// <summary>
        /// Renders all tiles onto the target image surface.
        /// Tiles are arranged in a 16×64 grid (256×1024 pixels).
        /// </summary>
        /// <param name="target">The image surface to draw on. Must be at least 256×1024 pixels.</>
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
        /// Draws a complete tile layer onto the target image.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="layer">The layer containing the tile indices to render.</param>
        public void Draw(IImage<TColor> target, Layer layer)
            => Draw(target, layer.Tiles, new Rectangle(0, 0, Layer.DIMENSION, Layer.DIMENSION), Layer.DIMENSION);

        /// <summary>
        /// Draws a stamp onto the target image at the specified offset.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="stamp">The stamp containing the tile data to render.</param>
        /// <param name="targetOffset">The pixel offset on the target image where the stamp is drawn.</param>
        public void Draw(IImage<TColor> target, Stamp stamp, Point targetOffset = default)
            => Draw(target, stamp.Tiles, new Rectangle(0, 0, stamp.Width, stamp.Height), Layer.DIMENSION, targetOffset);

        /// <summary>
        /// Draws a region of tiles onto the target image.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="tiles">The tile indices to render.</param>
        /// <param name="region">The tile region to draw from the tile array.</param>
        /// <param name="stride">The number of tiles per row in the source tile data.</param>
        /// <param name="targetOffset">The pixel offset on the target image where the tile region is drawn.</param>
        public void Draw(IImage<TColor> target, ReadOnlySpan<ushort> tiles, Rectangle region, int stride, Point targetOffset = default)
        {
            Rectangle targetRegion = new Rectangle(targetOffset.X + region.X * TileSize, targetOffset.Y + region.Y * TileSize, region.Width * TileSize, region.Height * TileSize);

            if (!target.GetBounds().Contains(targetRegion))
                throw new ArgumentException("Tile region exceeds target bounds.", nameof(target));

            for (int y = region.Y; y < region.Bottom; y++)
            {
                for (int x = region.X; x < region.Right; x++)
                {
                    ushort tile = tiles[(y - region.Y) * stride + (x - region.X)];
                    DrawTile(target, targetOffset.X + x * TileSize, targetOffset.Y + y * TileSize, tile);
                }
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
