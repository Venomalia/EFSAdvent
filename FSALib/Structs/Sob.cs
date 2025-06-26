using AuroraLib.Pixel.Processing;
using System;
using System.Runtime.InteropServices;

namespace FSALib.Structs
{
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct SpriteAttribute
    {
        /// <summary>
        /// Signed 8-bit height or Y-offset. Used for positioning.
        /// </summary>
        public readonly sbyte YOffset; // Byte 0: Signed 8-bit Y-offset

        private readonly byte _BlockShape; // Byte 1: 2 MSBs for shape, 6 bits unused

        /// <summary>
        /// Signed 8-bit X-offset from the sprite anchor.
        /// </summary>
        public readonly sbyte XOffset;        // Byte 2: Signed 8-bit x-offset

        private readonly byte _ObjectAttributes; // Byte 3: High nibble for block size, flip-y, flip-x

        /// <summary>
        /// Page offset in 8-pixel units. Used for character tile origin calculation within a 256x64 page.
        /// </summary>
        public readonly byte PageOffset;      // Byte 4: Origin position in multiples of 8 pixels

        private readonly byte _PalettePage;     // Byte 5: MSB nibble = palette selector, LSB nibble = page selector

        public readonly byte UnusedA => (byte)(_BlockShape & 0b0011_1111);
        public readonly byte UnusedB => (byte)(_ObjectAttributes & 0b0000_1111);

        /// <summary>
        /// Gets the block shape (Square, Wide, or Tall).
        /// </summary>
        public readonly BlockShapeType BlockShape => (BlockShapeType)(_BlockShape >> 6);

        /// <summary>
        /// Gets the block size variant (0 to 3) relative to the shape type.
        /// </summary>
        public readonly BlockSizeType BlockSize => (BlockSizeType)(_ObjectAttributes >> 6);

        /// <summary>
        /// Gets the mirror flags (horizontal and/or vertical) for this part.
        /// </summary>
        public readonly MirrorAxis Flip => (MirrorAxis)(_ObjectAttributes >> 4 & 0b11);

        /// <summary>
        /// Gets the palette index used by this part (0–15).
        /// </summary>
        public readonly byte PaletteIndex => (byte)(_PalettePage >> 4);

        /// <summary>
        /// Page selector (0-15), selects one of 16 sprite data pages.
        /// </summary>
        public readonly byte Page => (byte)(_PalettePage & 0b0000_1111);

        /// <summary>
        /// Returns the width and height in 8x8 blocks for this sprite based on block shape and size.
        /// </summary>
        public (int Width, int Height) GetBlockDimensions()
        {
            return BlockShape switch
            {
                BlockShapeType.Square => BlockSize switch
                {
                    BlockSizeType.Size0 => (1, 1),
                    BlockSizeType.Size1 => (2, 2),
                    BlockSizeType.Size2 => (4, 4),
                    BlockSizeType.Size3 => (8, 8),
                    _ => throw new NotSupportedException()
                },
                BlockShapeType.Wide => BlockSize switch
                {
                    BlockSizeType.Size0 => (2, 1),
                    BlockSizeType.Size1 => (4, 1),
                    BlockSizeType.Size2 => (4, 2),
                    BlockSizeType.Size3 => (8, 4),
                    _ => throw new NotSupportedException()
                },
                BlockShapeType.Tall => BlockSize switch
                {
                    BlockSizeType.Size0 => (1, 2),
                    BlockSizeType.Size1 => (1, 4),
                    BlockSizeType.Size2 => (2, 4),
                    BlockSizeType.Size3 => (4, 8),
                    _ => throw new NotSupportedException()
                },
                _ => throw new NotSupportedException()
            };
        }
        // Enum for Block Shape
        public enum BlockShapeType
        {
            Square = 0,
            Wide = 1,
            Tall = 2
        }

        public enum BlockSizeType
        {
            Size0 = 0,
            Size1 = 1,
            Size2 = 2,
            Size3 = 3
        }
    }
}
