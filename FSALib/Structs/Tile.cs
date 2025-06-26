using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FSALib.Structs
{
    /// <summary>
    /// Represents a 16×16 pixel tile composed of four 8×8 pixel parts.
    /// </summary>
    public readonly struct Tile
    {
        public readonly Part TopLeft, TopRight, BottomLeft, BottomRight;

        /// <summary>
        /// Returns the four parts of the tile as a read-only span in order: TopLeft, TopRight, BottomLeft, BottomRight.
        /// </summary>
        public ReadOnlySpan<Part> Parts
        {
            get
            {
#if NET6_0_OR_GREATER
                ref Part tRef = ref Unsafe.As<Tile, Part>(ref Unsafe.AsRef(in this));
                return MemoryMarshal.CreateSpan(ref tRef, 4);
#else
                unsafe
                {
                    fixed (Part* pTopLeft = &TopLeft)
                    {
                        return new ReadOnlySpan<Part>(pTopLeft, 4);
                    }
                }
#endif
            }
        }

        public Tile(Part topLeft, Part topRight, Part bottomLeft, Part bottomRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }
    }

}
