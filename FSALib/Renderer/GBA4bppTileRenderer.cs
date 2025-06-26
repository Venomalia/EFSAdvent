using AuroraLib.Core.Buffers;
using AuroraLib.Core.IO;
using AuroraLib.Pixel;
using AuroraLib.Pixel.Image;
using AuroraLib.Pixel.PixelFormats;
using AuroraLib.Pixel.PixelProcessor;
using AuroraLib.Pixel.Processing;
using System;
using System.IO;

namespace FSALib.Renderer
{
    public abstract class GBA4bppTileRenderer<TColor> where TColor : unmanaged, IColor<TColor>
    {
        protected const int PartSize = 8;
        private const int BPP = 4, ColorsPerPalette = 1 << BPP, BMask = (1 << BPP) - 1;

        /// <summary>
        /// The full color palette storage. Each palette contains 16 colors.
        /// </summary>
        public readonly TColor[] Palettes; // SCL

        /// <summary>
        /// Part index data (4bpp) used to look up palette entries for each 8×8 part.
        /// </summary>
        public readonly byte[] IndexSpritSheet;

        public GBA4bppTileRenderer(TColor[] palettes, byte[] spritSheet)
        {
            Palettes = palettes;
            IndexSpritSheet = spritSheet;
        }

        /// <summary>
        /// Loads 16-color palettes (SCL) from a stream.
        /// </summary>
        /// <param name="scl">Stream containing palette data in RGB555 format.</param>
        public void LoadPalettes(Stream scl)
        {
            using SpanBuffer<BGR555> buffer = new SpanBuffer<BGR555>((int)scl.Length / 2);
            scl.Read<BGR555>(buffer);
            buffer.Span.To<BGR555, TColor>(Palettes);
        }

        /// <summary>
        /// Returns a span over the 16-color palette at the specified index.
        /// </summary>
        /// <param name="index">Index of the palette (0–15).</param>
        /// <returns>A span of 16 <typeparamref name="TColor"/> colors representing the selected palette.</returns>
        public Span<TColor> GetPalette(int index)
            => Palettes.AsSpan(index * ColorsPerPalette, ColorsPerPalette);

        /// <summary>
        /// Returns a span of packed pixel indices for the specified part.
        /// Each part is 8×8 pixels with 4 bits per pixel, stored in 32 bytes.
        /// </summary>
        /// <param name="index">Index of the tile part.</param>
        /// <returns>A span of 32 bytes containing packed pixel indices.</returns>
        public Span<byte> GetPartIndices(int index)
        {
            const int IndexPartSize = PartSize * PartSize / 2;
            int sourceIndex = index * IndexPartSize;
            return IndexSpritSheet.AsSpan(sourceIndex, IndexPartSize);
        }

        protected static void DrawPart(IImage<TColor> target, int x, int y, ReadOnlySpan<byte> tIndex, ReadOnlySpan<TColor> tpalette, MirrorAxis mirrorAxis)
        {
            if (x < 0 || y < 0 || x + PartSize > target.Width || y + PartSize > target.Height)
                return;

            bool flipV = mirrorAxis.HasFlag(MirrorAxis.Vertical);
            bool flipH = mirrorAxis.HasFlag(MirrorAxis.Horizontal);

            RowAccessor<TColor> targetAccessor = new RowAccessor<TColor>(target, x, PartSize);
            for (int line = 0, p = 0; line < PartSize; line++)
            {
                int targetY = y + (flipV ? (PartSize - 1 - line) : line);
                Span<TColor> targetLine = targetAccessor[targetY];
                if (!flipH)
                {
                    for (int i = 0; i < PartSize; i += 2)
                    {
                        byte b = tIndex[p++];
                        int lo = b & BMask;
                        if (lo != 0) targetLine[i] = tpalette[lo];

                        int hi = b >> BPP;
                        if (hi != 0) targetLine[i + 1] = tpalette[hi];
                    }
                }
                else
                {
                    for (int i = PartSize - 1; i > 0; i -= 2)
                    {
                        byte b = tIndex[p++];
                        int lo = b & BMask;
                        if (lo != 0) targetLine[i] = tpalette[lo];

                        int hi = b >> BPP;
                        if (hi != 0) targetLine[i - 1] = tpalette[hi];
                    }
                }

                if (targetAccessor.IsBuffered)
                {
                    targetAccessor[targetY] = targetLine;
                }
            }
        }
    }
}