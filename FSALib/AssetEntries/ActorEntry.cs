using System.Collections.Generic;
using System.Diagnostics;

namespace FSALib.AssetEntries
{
    /// <summary>
    /// Defines the schema for an actor type, describing its properties and behavior.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public sealed class ActorEntry : IActorResourcesEntry
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public AssetFlags AssetState { get; set; }

        /// <inheritdoc/>
        public bool NoGBASprite { get; set; }

        /// <inheritdoc/>
        public List<string>? Resources { get; set; }

        /// <summary>
        /// Internal name if known.
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// The category of the actor, defining its role or function in the game.
        /// </summary>
        public ActorCategoryType Category { get; set; }

        /// <summary>
        /// A list of fields defining additional properties or configurations for the actor.
        /// </summary>
        public List<VariableField> Fields { get; set; }

        public ActorEntry()
        {
            Name = Description = InternalName = string.Empty;
            Fields = new List<VariableField>();
        }

        /// <inheritdoc/>
        public override string ToString() => Name;
    }
}
