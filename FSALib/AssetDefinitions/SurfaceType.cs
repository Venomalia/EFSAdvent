namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Defines the surface type of a tile.
    /// </summary>
    public enum SurfaceType
    {
        /// <summary>The tile has a normal surface.</summary>
        Normal = default,

        /// <summary>The tile represents an abyss or bottomless pit.</summary>
        Abyss,

        /// <summary>The tile represents shallow water.</summary>
        ShallowWater,

        /// <summary>The tile represents deep water.</summary>
        DeepWater,

        /// <summary>The tile has a slippery surface.</summary>
        Slippery,

        /// <summary>The tile represents quicksand.</summary>
        Quicksand,

        /// <summary>The tile represents ladder.</summary>
        Ladder
    }
}
