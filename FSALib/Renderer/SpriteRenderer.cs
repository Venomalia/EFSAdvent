using AuroraLib.Pixel;
using AuroraLib.Pixel.Image;
using AuroraLib.Pixel.Processing;
using FSALib.Structs;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace FSALib.Renderer
{
    public sealed class SpriteRenderer<TColor> : GBA4bppTileRenderer<TColor> where TColor : unmanaged, IColor<TColor>
    {
        public readonly List<SpriteAttribute[]> spriteAttributes;

        public SpriteRenderer(Stream gb_ch_obj_base, Stream enemyOrEnding, Stream gals2, Stream player, Stream gals) : base(new TColor[0x100], new byte[0x2000 * 16])
        {
            spriteAttributes = new List<SpriteAttribute[]>();

            gb_ch_obj_base.Read(IndexSpritSheet, 0, 0x7000);
            LoadEnemyOrEnding(enemyOrEnding);
            gals2.Read(IndexSpritSheet, 0x8000 + 0x8000, 0x4000);
            player.Read(IndexSpritSheet, 0x8000 + 0x8000 + 0x4000, 0x4000);
            gals.Read(IndexSpritSheet, 0x8000 + 0x8000 + 0x8000, 0x8000);
        }

        public void LoadEnemyOrEnding(Stream enemyOrEnding) => enemyOrEnding.Read(IndexSpritSheet, 0x8000, 0x8000);

        public void LoadSpriteLevelIndices(Stream gb_ch_obj_tr) => gb_ch_obj_tr.Read(IndexSpritSheet, 0x7000, 0x1000);

        public void LoadSpriteAttributeList(Stream sob)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)sob.Length);
            try
            {
                sob.Read(buffer, 0, (int)sob.Length);
                LoadSpriteAttributeList(buffer.AsSpan(0, (int)sob.Length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public void LoadSpriteAttributeList(ReadOnlySpan<byte> sob)
        {
            ushort spriteCount = BinaryPrimitives.ReadUInt16LittleEndian(sob);
            ReadOnlySpan<ushort> offsets = MemoryMarshal.Cast<byte, ushort>(sob.Slice(2, spriteCount * 2 + 2));

            spriteAttributes.Clear();
            spriteAttributes.Capacity = spriteCount;

            for (int i = 0; i < spriteCount; i++)
            {
                int start = offsets[i];
                int end = offsets[i + 1];

                int length = end - start;

                ReadOnlySpan<byte> attrBytes = sob.Slice(start, length);
                ReadOnlySpan<SpriteAttribute> attributes = MemoryMarshal.Cast<byte, SpriteAttribute>(attrBytes);
                spriteAttributes.Add(attributes.ToArray());
            }
        }

        public void DrawSprite(IImage<TColor> target, int x, int y, ushort spriteIndex, int paletteIndex = -1, int replacePaletteIndex = -1)
        {
            Span<SpriteAttribute> attributes = spriteAttributes[spriteIndex];
            for (int i = attributes.Length - 1; i >= 0; i--)
            {
                int p = attributes[i].PaletteIndex;
                if (paletteIndex != - 1)
                {
                    p = (replacePaletteIndex == -1 || p == replacePaletteIndex) ? paletteIndex : p;
                }
                DrawSpriteAttribute(target, x, y, attributes[i], GetPalette(p));
            }
        }

        public void DrawSpriteAttribute(IImage<TColor> target, int x, int y, SpriteAttribute attribute)
            => DrawSpriteAttribute(target, x, y, attribute, GetPalette(attribute.PaletteIndex));

        public void DrawSpriteAttribute(IImage<TColor> target, int x, int y, SpriteAttribute attribute, ReadOnlySpan<TColor> palette)
        {
            var (blocksX, blocksY) = attribute.GetBlockDimensions();
            bool flipH = attribute.Flip.HasFlag(MirrorAxis.Horizontal);
            bool flipV = attribute.Flip.HasFlag(MirrorAxis.Vertical);
            int baseTileIndex = attribute.Page * 256 + attribute.PageOffset;

            for (int by = 0; by < blocksY; by++)
            {
                for (int bx = 0; bx < blocksX; bx++)
                {
                    int tileIndex = baseTileIndex + by * 32 + bx;
                    Span<byte> tile = GetPartIndices(tileIndex);

                    int tileOffsetX = flipH ? (blocksX - 1 - bx) * PartSize : bx * PartSize;
                    int tileOffsetY = flipV ? (blocksY - 1 - by) * PartSize : by * PartSize;

                    int drawX = x + attribute.XOffset + tileOffsetX;
                    int drawY = y + attribute.YOffset + tileOffsetY;

                    DrawPart(target, drawX, drawY, tile, palette, attribute.Flip);
                }
            }
        }
    }
}
