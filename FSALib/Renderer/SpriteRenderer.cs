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
using static FSALib.Rarc;

namespace FSALib.Renderer
{
    /// <summary>
    /// Renders sprite graphics and manages sprite tiles, palettes, and sprite attribute data.
    /// </summary>
    /// <typeparam name="TColor">The color type used by the rendering surface.</typeparam>
    public sealed class SpriteRenderer<TColor> : GBA4bppTileRenderer<TColor> where TColor : unmanaged, IColor<TColor>
    {
        /// <summary>
        /// Gets the loaded sprite attribute lists grouped by source file.
        /// </summary>
        public readonly List<List<SpriteAttribute[]>> SpriteAttributeLists;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer{TColor}"/> class
        /// using the specified sprite graphics streams.
        /// </summary>
        /// <param name="gb_ch_obj_base">Stream containing the base sprite graphics.</param>
        /// <param name="enemyOrEnding">Stream containing the enemy or ending sprite graphics.</param>
        /// <param name="gals2">Stream containing the secondary character sprite graphics.</param>
        /// <param name="player">Stream containing the player sprite graphics.</param>
        /// <param name="gals">Stream containing the character sprite graphics.</param>
        public SpriteRenderer(Stream gb_ch_obj_base, Stream enemyOrEnding, Stream gals2, Stream player, Stream gals) : base(new TColor[0x100], new byte[0x2000 * 16])
        {
            SpriteAttributeLists = new List<List<SpriteAttribute[]>>();

            gb_ch_obj_base.Read(IndexSpritSheet, 0, 0x7000);
            LoadEnemyOrEnding(enemyOrEnding);
            gals2.Read(IndexSpritSheet, 0x8000 + 0x8000, 0x4000);
            player.Read(IndexSpritSheet, 0x8000 + 0x8000 + 0x4000, 0x4000);
            gals.Read(IndexSpritSheet, 0x8000 + 0x8000 + 0x8000, 0x8000);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer{TColor}"/> class
        /// using resources loaded from a RARC archive.
        /// </summary>
        /// <param name="data">The RARC archive containing the sprite resources.</param>
        public SpriteRenderer(Rarc data) : this(data.Root.Directorys[Rarc.CommonFolderTypes.SCH])
        {
            // I don't know if that's the most logical order.
            DirectoryNode sobDirectory = data.Root.Directorys[Rarc.CommonFolderTypes.SOB];
            var enemyFile = sobDirectory.GetFile("enemy_both_sob.bin").Data;
            var enemy2File = sobDirectory.GetFile("enemy_both2_sob.bin").Data;
            var enemyGCFile = sobDirectory.GetFile("enemy_gc_sob.bin").Data;
            var playerFile = sobDirectory.GetFile("player_sob.bin").Data;
            var kageFile = sobDirectory.GetFile("kage_sob.bin").Data;
            var playerkageFile = sobDirectory.GetFile("player_kage_sob.bin").Data;
            LoadSpriteAttributeList(enemyFile, enemy2File, enemyGCFile, playerFile, kageFile, playerkageFile);
        }

        private SpriteRenderer(DirectoryNode sch) : this(sch.GetFile("gb_ch_obj_base_sch.szs").Data, sch.GetFile("enemy_sch.szs").Data, sch.GetFile("gals2_sch.szs").Data, sch.GetFile("player_sch.szs").Data, sch.GetFile("gals_sch.szs").Data)
        { }

        /// <summary>
        /// Loads the sprite tiles, enemy or ending graphics, and palettes for a level.
        /// </summary>
        /// <param name="data">The RARC archive containing the sprite resources.</param>
        /// <param name="objTilesheetIndex">The object tilesheet index to load.</param>
        /// <param name="GBA">Specifies whether to load the Game Boy Advance palette variant.</param>
        /// <param name="ending">Specifies whether to load the ending graphics instead of the enemy graphics.</param>
        public void LoadTilesheet(Rarc data, int objTilesheetIndex, bool GBA = false, bool ending = false)
        {
            DirectoryNode schDirectory = data.Root.Directorys[Rarc.CommonFolderTypes.SCH];

            string tilesheetFileName = $"gb_ch_obj_tr_sch{objTilesheetIndex:x}.szs";
            LoadSpriteLevelIndices(schDirectory.GetFile(tilesheetFileName).Data);

            string enemyOrEndingFileName = ending ? "ending_sch.szs" : "enemy_sch.szs";
            LoadEnemyOrEnding(schDirectory.GetFile(enemyOrEndingFileName).Data);

            string paletteFileName = (GBA ? "gb" : "cu") + "_cl_obj_base_scl.szs";
            var palette = data.Root.Directorys[Rarc.CommonFolderTypes.SCL].GetFile(paletteFileName);
            LoadPalettes(palette.Data);
        }

        /// <summary>
        /// Loads the enemy or ending sprite graphics.
        /// </summary>
        /// <param name="enemyOrEnding">The stream containing the sprite graphics.</param>
        public void LoadEnemyOrEnding(Stream enemyOrEnding) => enemyOrEnding.Read(IndexSpritSheet, 0x8000, 0x8000);

        /// <summary>
        /// Loads the level-specific sprite tile indices.
        /// </summary>
        /// <param name="gb_ch_obj_tr">The stream containing the sprite tile index data.</param>
        public void LoadSpriteLevelIndices(Stream gb_ch_obj_tr) => gb_ch_obj_tr.Read(IndexSpritSheet, 0x7000, 0x1000);

        /// <summary>
        /// Loads sprite attribute lists from one or more SOB files.
        /// </summary>
        /// <param name="sobs">The SOB streams containing sprite attribute data.</param>
        public void LoadSpriteAttributeList(params Stream[] sobs)
        {
            SpriteAttributeLists.Clear();
            foreach (var sob in sobs)
            {
                var attributeList = new List<SpriteAttribute[]>();
                LoadSpriteAttributeList(sob, attributeList);
                SpriteAttributeLists.Add(attributeList);
            }
        }

        /// <summary>
        /// Loads sprite attributes from a SOB stream.
        /// </summary>
        /// <param name="sob">The stream containing the SOB data.</param>
        /// <param name="attributeList">The list that receives the loaded sprite definitions.</param>
        public static void LoadSpriteAttributeList(Stream sob, List<SpriteAttribute[]> attributeList)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)sob.Length);
            try
            {
                sob.Read(buffer, 0, (int)sob.Length);
                LoadSpriteAttributeList(buffer.AsSpan(0, (int)sob.Length), attributeList);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        /// <summary>
        /// Loads sprite attributes from a block of SOB data.
        /// </summary>
        /// <param name="sob">The SOB data.</param>
        /// <param name="attributeList">The list that receives the loaded sprite definitions.</param>
        public static void LoadSpriteAttributeList(ReadOnlySpan<byte> sob, List<SpriteAttribute[]> attributeList)
        {
            ushort spriteCount = BinaryPrimitives.ReadUInt16LittleEndian(sob);
            ReadOnlySpan<ushort> offsets = MemoryMarshal.Cast<byte, ushort>(sob.Slice(2, spriteCount * 2 + 2));

            attributeList.Capacity = attributeList.Count + spriteCount;

            for (int i = 0; i < spriteCount; i++)
            {
                int start = offsets[i];
                int end = offsets[i + 1];

                int length = end - start;

                ReadOnlySpan<byte> attrBytes = sob.Slice(start, length);
                ReadOnlySpan<SpriteAttribute> attributes = MemoryMarshal.Cast<byte, SpriteAttribute>(attrBytes);
                attributeList.Add(attributes.ToArray());
            }
        }

        /// <summary>
        /// Draws a sprite using the specified sprite index from a sprite attribute list.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="x">The horizontal position in pixels.</param>
        /// <param name="y">The vertical position in pixels.</param>
        /// <param name="spriteIndex">The index of the sprite to draw.</param>
        /// <param name="attributeList">The index of the sprite attribute list containing the sprite.</param>
        /// <param name="replacementPaletteIndex">The palette index to use as a replacement.</param>
        /// <param name="targetPaletteIndex">The palette index to replace. If <c>-1</c>, all palettes are replaced.</param>
        public void DrawSprite(IImage<TColor> target, int x, int y, ushort spriteIndex, ushort attributeList = 0, sbyte replacementPaletteIndex = -1, sbyte targetPaletteIndex = -1)
            => DrawSprite(target, x, y, SpriteAttributeLists[attributeList][spriteIndex], replacementPaletteIndex, targetPaletteIndex);

        /// <summary>
        /// Draws a sprite from a collection of sprite attributes.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="x">The horizontal position in pixels.</param>
        /// <param name="y">The vertical position in pixels.</param>
        /// <param name="attributes">The sprite attributes defining the sprite.</param>
        /// <param name="replacementPaletteIndex">The palette index to use as a replacement.</param>
        /// <param name="targetPaletteIndex">The palette index to replace. If <c>-1</c>, all palettes are replaced.</param>
        public void DrawSprite(IImage<TColor> target, int x, int y, Span<SpriteAttribute> attributes, sbyte replacementPaletteIndex = -1, sbyte targetPaletteIndex = -1)
        {
            for (int i = attributes.Length - 1; i >= 0; i--)
            {
                int p = attributes[i].PaletteIndex;
                if (replacementPaletteIndex != -1)
                {
                    p = (targetPaletteIndex == -1 || p == targetPaletteIndex) ? replacementPaletteIndex : p;
                }
                DrawSpriteAttribute(target, x, y, attributes[i], GetPalette(p));
            }
        }

        /// <summary>
        /// Draws a single sprite attribute using the specified palette.
        /// </summary>
        /// <param name="target">The image surface to draw on.</param>
        /// <param name="x">The horizontal position in pixels.</param>
        /// <param name="y">The vertical position in pixels.</param>
        /// <param name="attribute">The sprite attribute to draw.</param>
        /// <param name="palette">The palette used to render the sprite attribute.</param>
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
