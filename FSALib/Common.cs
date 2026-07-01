using AuroraLib.Compression.Formats.Nintendo;
using AuroraLib.Core.IO;

namespace FSALib
{
    internal static class Common
    {
        public static readonly Yaz0 Yaz0 = new Yaz0() { FormatByteOrder = Endian.Big };
    }
}
