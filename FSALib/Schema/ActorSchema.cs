using System;
using System.Collections.Generic;
using System.IO;

namespace FSALib.Schema
{
    public class ActorSchema : ISchema
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ActorCategory Category { get; set; }
        public List<VariableField> Fields { get; set; }

        public ActorSchema()
        {
            Name = Description = string.Empty;
            Fields = new List<VariableField>();
        }

        public static ActorSchema LoadSchema(Stream stream)
            => Assets.Deserialize<ActorSchema>(stream) ?? throw new ArgumentException();

        public static void SaveSchema(ActorSchema schema, Stream stream)
            => Assets.Serialize(stream, schema);
    }
}
