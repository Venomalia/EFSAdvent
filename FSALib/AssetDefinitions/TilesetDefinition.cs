using System.Diagnostics;

namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Defines metadata and file identifiers for a tileset.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class TilesetDefinition : IAssetDefinition
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public AssetFlags AssetState { get; set; }

        /// <summary>
        /// Internal tileset identifier used by the game's file naming convention.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Alternative identifier used for the GBA-specific tileset files, if available.
        /// </summary>
        public string? GbaID { get; set; }

        public TilesetDefinition()
        {
            Name = Description = ID = string.Empty;
        }
        public string GetFileName(bool GBA = false)
            => $"bg_{(GBA && GbaID != null ? GbaID : ID)}_sch.szs";

        public string GetPaletteFileName(bool GBA = false)
            => $"bg_{ID}{(GBA ? "_gb" : string.Empty)}_scl.szs";
    }
}
