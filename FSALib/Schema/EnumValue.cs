
namespace FSALib.Schema
{
    public class EnumValue : ISchema
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public EnumValue()
            => Name = Description = string.Empty;
    }
}
