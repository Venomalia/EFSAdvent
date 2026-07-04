using System.Collections.Generic;

namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Defines how actor states are mapped to render variants using a bitmask-based lookup.
    /// </summary>
    public sealed class ActorRenderDefinition
    {
        /// <summary>
        /// Bitmask used to extract the actor state for rendering selection.
        /// </summary>
        public uint BitMask { get; set; }

        /// <summary>
        /// Mapping of actor state values to one or more render variants.
        /// </summary>
        public Dictionary<int, ActorRenderVariant[]> Variants { get; set; } = new Dictionary<int, ActorRenderVariant[]>();
    }
}
