using System;

namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Defines additional properties and behaviors of a tile.
    /// </summary>
    [Flags]
    public enum TileProperties
    {
        /// <summary>The tile has no additional properties.</summary>
        None = 0,

        /// <summary>The tile damages entities that come into contact with it.</summary>
        Hazard = 1 << 0,

        /// <summary>The tile blocks enemies from passing through it.</summary>
        EnemyCollision = 1 << 1,

        /// <summary>The tile allows entities to be thrown over it.</summary>
        ThrowOver = 1 << 2,

        /// <summary>The tile allows entities to drop off or jump down from it.</summary>
        DropOff = 1 << 3
    }
}
