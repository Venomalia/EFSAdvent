using System.Diagnostics;

namespace FSALib.AssetEntries
{
    /// <summary>
    /// Represents a world entry with metadata for loading and UI presentation.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class WorldDefinition : IAssetDefinition
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public AssetFlags AssetState { get; set; }

        /// <summary>
        /// Text index used to resolve the localized world name from the game's BMD text archive.
        /// </summary>
        public int TextID { get; set; }

        /// <summary>
        /// Texture used to render the map mask / region overlay for this world.
        /// </summary>
        public string MapMaskGraphic { get; set; }

        public WorldDefinition()
        {
            Name = MapMaskGraphic = string.Empty;
            TextID = -1;
        }
    }
}
