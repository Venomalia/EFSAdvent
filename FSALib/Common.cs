using AuroraLib.Compression.Formats.Nintendo;
using AuroraLib.Core.IO;

namespace FSALib
{
    internal static class Common
    {
        public static readonly Yaz0 Yaz0 = new Yaz0() { FormatByteOrder = Endian.Big };
        public static readonly Yay0 Yay0 = new Yay0() { FormatByteOrder = Endian.Big };
    }
}
