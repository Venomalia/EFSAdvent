using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FSALib.TestApp
{
    [TestClass]
    public sealed class AssetsTest
    {
        [TestMethod]
        public void Assets_LoadsDefinitions()
        {
            Assert.IsNotNull(Assets.Songs);
            Assert.IsNotNull(Assets.TileProperties);
            Assert.IsNotNull(Assets.Worlds);
            Assert.IsNotNull(Assets.Stages);
            Assert.IsNotNull(Assets.BattleStages);
            Assert.IsNotNull(Assets.Tilesets);
            Assert.IsNotNull(Assets.Actors);
        }

        [TestMethod]
        public void MirrorTileLOT_HasExpectedLength()
        {
            Assert.AreEqual(0x400, Assets.MirrorTileLOT.Length);
        }

        [TestMethod]
        public void Reload_DoesNotThrow()
        {
            Assets.Reload();
        }
    }
}
