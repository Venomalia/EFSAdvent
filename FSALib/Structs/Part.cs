using AuroraLib.Pixel.Processing;
using System;

namespace FSALib.Structs
{
    /// <summary>
    /// Represents an 8×8 tile part.
    /// </summary>
    public readonly struct Part
    {
        // PPPPHVII IIIIIIII
        private readonly ushort PackedValue;

        public Part(byte palette, MirrorAxis flip, ushort Index)
        {
            if (palette > 0xF)
                throw new ArgumentOutOfRangeException(nameof(palette), "Palette must be between 0 and 15.");

            if (Index > 0x3FF)
                throw new ArgumentOutOfRangeException(nameof(Index), "Index must be between 0 and 1023.");

            PackedValue = (ushort)((palette & 0xF) << 12);
            PackedValue |= (ushort)((int)flip << 10);
            PackedValue |= Index;
        }

        /// <summary>
        /// Gets the palette index used by this part (0–15).
        /// </summary>
        public byte PaletteIndex => (byte)((PackedValue >> 12) & 0xF);

        /// <summary>
        /// Gets the mirror flags (horizontal and/or vertical) for this part.
        /// </summary>
        public MirrorAxis Flip => (MirrorAxis)((PackedValue >> 10) & 0x3);

        /// <summary>
        /// Gets the tile index of this part (0–1023).
        /// </summary>
        public ushort Index => (ushort)(PackedValue & 0x3FF);
    }
}
