using System.Diagnostics;

namespace FSALib.AssetEntries
{
    /// <summary>
    /// Represents a stage entry with metadata for loading and UI presentation.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class StageDefinition : IAssetDefinition
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public AssetFlags AssetState { get; set; }

        /// <summary>
        /// Text index from the BMD archive.
        /// Not used for stages; kept only for data completeness.
        /// </summary>
        public int TextID { get; set; }

        /// <summary>
        /// Identifier of the world this stage belongs to.
        /// </summary>
        public int WorldID { get; set; }

        /// <summary>
        /// Texture displayed as the stage title card.
        /// </summary>
        public string TitleCardGraphic { get; set; }

        public StageDefinition()
        {
            Name = TitleCardGraphic = string.Empty;
            TextID = WorldID = -1;
        }
    }
}
