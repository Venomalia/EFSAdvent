using AuroraLib.Compression.Algorithms;

namespace FSALib
{
    internal static class Common
    {
        public static readonly Yaz0 Yaz0 = new Yaz0() { LookAhead = true, FormatByteOrder = AuroraLib.Core.Endian.Big };
    }
}
