
namespace FSALib.AssetEntries
{
    /// <summary>
    /// Defines a common schema structure for various game entities.
    /// </summary>
    public interface IAssetDefinition
    {
        /// <summary>
        /// The name of the entry.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A brief description of the entry, explaining its purpose or role.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Additional metadata flags describing the entry state and usage.
        /// </summary>
        AssetFlags AssetState { get; }
    }
}
