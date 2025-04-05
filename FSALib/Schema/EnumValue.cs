
using System.Collections.Generic;

namespace FSALib.Schema
{
    /// <summary>
    /// Represents an individual value in an enumeration, are typically used to define specific options for a variable field, allowing for easy identification and mapping to specific logic or behaviors.
    /// </summary>
    public sealed class EnumValue : ISchemaActorResources
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public bool NoGBASprite { get; set; }

        /// <inheritdoc/>
        public List<string>? Resources { get; set; }

        public EnumValue()
            => Name = Description = string.Empty;
    }
}
