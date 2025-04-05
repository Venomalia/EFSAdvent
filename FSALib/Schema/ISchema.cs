
namespace FSALib.Schema
{
    /// <summary>
    /// Defines a common schema structure for various game entities.
    /// </summary>
    public interface ISchema
    {
        /// <summary>
        /// The name of the schema.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A brief description of the schema, explaining its purpose or role.
        /// </summary>
        string Description { get; }
    }
}
