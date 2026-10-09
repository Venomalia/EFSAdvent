using FSALib.Structs;
using System.Runtime.InteropServices;

namespace FSALib.TestApp
{
    [TestClass]
    public sealed class PartAndTileTest
    {

        [TestMethod]
        public void Part_SizeIs2Bytes()
        {
            Assert.AreEqual(2, Marshal.SizeOf<Part>());
        }

        [TestMethod]
        [DataRow((byte)0, MirrorAxis.None, (ushort)0)] // min
        [DataRow((byte)10, MirrorAxis.Horizontal, (ushort)642)]
        [DataRow((byte)15, MirrorAxis.Horizontal | MirrorAxis.Vertical, (ushort)1023)] // max
        public void Constructor_PreservesValues(byte paletteIndex, MirrorAxis flip, ushort index)
        {
            var part = new Part(paletteIndex, flip, index);

            Assert.AreEqual(paletteIndex, part.PaletteIndex);
            Assert.AreEqual(flip, part.Flip);
            Assert.AreEqual(index, part.Index);
        }

        [TestMethod]
        public void Constructor_RejectsPaletteAbove15()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Part(16, MirrorAxis.None, 0));
        }

        [TestMethod]
        public void Constructor_RejectsIndexAbove1023()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Part(0, MirrorAxis.None, 1024));
        }

        [TestMethod]
        public void Tile_SizeIs8Bytes()
        {
            Assert.AreEqual(8, Marshal.SizeOf<Tile>());
        }

        [TestMethod]
        public void Tile_ReturnsFourPartsInOrder()
        {
            var topLeft = new Part(0, MirrorAxis.None, 1);
            var topRight = new Part(1, MirrorAxis.None, 2);
            var bottomLeft = new Part(2, MirrorAxis.None, 3);
            var bottomRight = new Part(3, MirrorAxis.None, 4);

            var tile = new Tile(topLeft, topRight, bottomLeft, bottomRight);
            var parts = tile.Parts;

            Assert.AreEqual(4, parts.Length);
            Assert.AreEqual((ushort)1, parts[0].Index);
            Assert.AreEqual((ushort)2, parts[1].Index);
            Assert.AreEqual((ushort)3, parts[2].Index);
            Assert.AreEqual((ushort)4, parts[3].Index);
        }
    }
}
