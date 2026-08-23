using System;

namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Defines the collision mask of a tile.
    /// </summary>
    [Flags]
    public enum TileCollision
    {
        /// <summary>The tile has no collision.</summary>
        Walkable = 0,
        /// <summary>The top-left quadrant is solid.</summary>
        TopLeft = 1 << 0,

        /// <summary>The top-right quadrant is solid.</summary>
        TopRight = 1 << 1,

        /// <summary>The bottom-left quadrant is solid.</summary>
        BottomLeft = 1 << 2,

        /// <summary>The bottom-right quadrant is solid.</summary>
        BottomRight = 1 << 3,


        /// <summary>The left half is solid.</summary>
        Left = TopLeft | BottomLeft,

        /// <summary>The right half is solid.</summary>
        Right = TopRight | BottomRight,

        /// <summary>The top half is solid.</summary>
        Top = TopLeft | TopRight,

        /// <summary>The bottom half is solid.</summary>
        Bottom = BottomLeft | BottomRight,

        /// <summary>All quadrants are solid.</summary>
        Solid = TopLeft | TopRight | BottomLeft | BottomRight
    }
}
