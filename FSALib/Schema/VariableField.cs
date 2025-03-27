using AuroraLib.Core;
using System.Collections.Generic;

namespace FSALib.Schema
{
    public class VariableField : ISchema
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BitOffset { get; set; }
        public int BitSize { get; set; }
        public ValueType ValueType { get; set; }
        public Dictionary<int, EnumValue>? EnumValues { get; set; }

        public int Mask => (1 << BitSize) - 1;

        public VariableField()
            => Name = Description = string.Empty;

        public uint ReadActorField(uint actorVariable)
            => (uint)((actorVariable >> BitOffset) & Mask);

        public uint UpdateActorField(uint actorVariable, uint value)
        {
            uint mask = (uint)Mask;
            ThrowIf.GreaterThan(value, mask, nameof(value));

            actorVariable &= ~mask << BitOffset; 
            actorVariable |= value << BitOffset;
            return actorVariable;
        }
    }
}
