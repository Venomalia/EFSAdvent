using System;

namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Flags describing the status and intended usage of an asset.
    /// </summary>
    [Flags]
    public enum AssetFlags
    {
        /// <summary>
        /// Asset originates from an early or experimental development version.
        /// </summary>
        Development = 1 << 0,

        /// <summary>
        /// Asset is incomplete, malfunctioning, or known to be unusable.
        /// </summary>
        Broken = 1 << 1,

        /// <summary>
        /// Asset is intended for development or debugging purposes.
        /// </summary>
        Debug = 1 << 2,
    }
}
