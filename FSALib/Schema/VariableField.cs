using AuroraLib.Core;
using System.Collections.Generic;

namespace FSALib.Schema
{
    /// <summary>
    /// Represents a variable field within a schema, defining how data is stored and interpreted.
    /// </summary>
    public sealed class VariableField : ISchema
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <summary>
        /// The bit position where this field starts within the data structure.
        /// </summary>
        public int BitOffset { get; set; }

        /// <summary>
        /// The number of bits this field occupies.
        /// </summary>
        public int BitSize { get; set; }

        /// <summary>
        /// The type of value stored in this field (e.g., integer, enum, boolean).
        /// </summary>
        public ValueType ValueType { get; set; }

        /// <summary>
        /// Optional enumeration values, if the field represents an enumerated type.
        /// The key represents the raw integer value, while the value provides metadata.
        /// </summary>
        public Dictionary<int, EnumValue>? EnumValues { get; set; }

        public int Mask => (1 << BitSize) - 1;

        public VariableField()
            => Name = Description = string.Empty;

        /// <summary>
        /// Reads the value of the actor field from the given actor variable.
        /// </summary>
        /// <param name="actorVariable">The actor variable from which the field value is to be extracted.</param>
        /// <returns>The value of the actor field, extracted from the actor variable.</returns>
        public uint ReadActorField(uint actorVariable)
            => (uint)((actorVariable >> BitOffset) & Mask);

        /// <summary>
        /// Updates the value of the actor field within the given actor variable.
        /// </summary>
        /// <param name="actorVariable">The actor variable to be updated.</param>
        /// <param name="value">The new value to set for the actor field. It is validated to fit within the field's mask.</param>
        /// <returns>The updated actor variable with the new field value set.</returns>
        public uint UpdateActorField(uint actorVariable, uint value)
        {
            uint mask = (uint)Mask;
            ThrowIf.GreaterThan(value, mask, nameof(value));

            // Shift the mask to the correct bit offset
            uint shiftedMask = mask << BitOffset;
            // Clear the current field value in the actor variable
            actorVariable &= ~shiftedMask;
            // Insert the new field value at the correct bit offset
            actorVariable |= value << BitOffset;
            return actorVariable;
        }
    }
}
