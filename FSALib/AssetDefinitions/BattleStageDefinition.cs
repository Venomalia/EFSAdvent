using System.Diagnostics;

namespace FSALib.AssetEntries
{
    /// <summary>
    /// Represents a battle stage entry with metadata for loading and UI presentation.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class BattleStageDefinition : IAssetDefinition
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public AssetFlags AssetState { get; set; }

        /// <summary>
        /// Text index used to resolve the localized stage name from the game's BMD text archive.
        /// </summary>
        public int TextID { get; set; }

        /// <summary>
        /// Texture used as the stage UI icon.
        /// </summary>
        public string IconGraphic { get; set; }

        public BattleStageDefinition()
        {
            Name = IconGraphic = string.Empty;
            TextID = -1;
        }
    }
}
