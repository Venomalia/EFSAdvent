using System;

namespace FSALib.Structs
{
    /// <summary>
    /// Specifies the axis along which mirroring can be applied.
    /// </summary>
    [Flags]
    public enum MirrorAxis : byte
    {
        /// <summary>
        /// No mirroring is applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// Mirrors the image horizontally.
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// Mirrors the image vertically.
        /// </summary>
        Vertical = 2,
    }
}
