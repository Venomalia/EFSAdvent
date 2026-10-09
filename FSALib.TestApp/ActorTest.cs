using AuroraLib.Core.Format.Identifier;
using FSALib.Structs;
using System.Runtime.InteropServices;

namespace FSALib.TestApp
{
    [TestClass]
    public sealed class ActorTest
    {
        [TestMethod]
        public void Size_Is11Bytes()
        {
            Assert.AreEqual(11, Marshal.SizeOf<Actor>());
        }

        [TestMethod]
        public void Variable_PreservesValue()
        {
            var actor = new Actor((Identifier32)"TEST", 1, 2, 3, 0x12345678);

            Assert.AreEqual("TEST", actor.Name);
            Assert.AreEqual(1, actor.Layer);
            Assert.AreEqual(2, actor.XCoord);
            Assert.AreEqual(3, actor.YCoord);
            Assert.AreEqual(0x12345678u, actor.Variable);
        }

        [TestMethod]
        public void VariableBytes_PreserveBigEndianOrder()
        {
            var actor = new Actor((Identifier32)"TEST", 0, 0, 0, 0x12345678);

            Assert.AreEqual((byte)0x78, actor.VariableByte1);
            Assert.AreEqual((byte)0x56, actor.VariableByte2);
            Assert.AreEqual((byte)0x34, actor.VariableByte3);
            Assert.AreEqual((byte)0x12, actor.VariableByte4);
        }

        [TestMethod]
        public void VariableByte_Setter_UpdatesOnlySelectedByte()
        {
            var actor = new Actor(new Identifier32("TEST".AsSpan()), 0, 0, 0, 0x12345678);

            actor.VariableByte3 = 0xAB;

            Assert.AreEqual(0x12AB5678u, actor.Variable);
        }

        [TestMethod]
        public void ToStringCode_RoundTrips()
        {
            var original = new Actor(new Identifier32("TEST".AsSpan()), 2, 10, 20, 0x12345678);

            Assert.IsTrue(Actor.PasteFromString(original.ToStringCode(), out var parsed));
            Assert.AreEqual(original, parsed);
        }

        [TestMethod]
        public void PasteFromString_RejectsInvalidInput()
        {
            Assert.IsFalse(Actor.PasteFromString("invalid", out _));
        }
    }
}
