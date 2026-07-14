using System;

namespace FSALib
{
    public sealed partial class Rarc
    {
        /// <summary>
        /// File Attibutes
        /// </summary>
        [Flags]
        public enum FileAttribute : byte
        {
            /// <summary>
            /// Indicates this is a File
            /// </summary>
            FILE = 0x01,

            /// <summary>
            /// Directory.
            /// </summary>
            DIRECTORY = 0x02,

            /// <summary>
            /// Indicates that this file is YAY0 Compressed.
            /// </summary>
            YAY0_COMPRESSED = 0x04,

            /// <summary>
            /// Indicates that this file gets Pre-loaded into Main RAM
            /// </summary>
            PRELOAD_TO_MRAM = 0x10,

            /// <summary>
            /// Indicates that this file gets Pre-loaded into Auxiliary RAM (GameCube only) 
            /// </summary>
            PRELOAD_TO_ARAM = 0x20,

            /// <summary>
            /// Indicates that this file does not get pre-loaded, but rather read from the DVD
            /// </summary>
            LOAD_FROM_DVD = 0x40,

            /// <summary>
            /// Indicates that this file is YAZ0 Compressed not YAY0.
            /// </summary>
            YAZ0_COMPRESSED = 0x80 | 0x04
        }
    }
}
