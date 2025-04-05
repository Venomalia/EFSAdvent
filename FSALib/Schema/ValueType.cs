
namespace FSALib.Schema
{
    /// <summary>
    /// Defines the types of values that can be associated with actor fields.
    /// These types represent different kinds of data that can be stored and manipulated for actors in the game.
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// Represents an integer value, typically used for numerical data like health, score, or position.
        /// </summary>
        Integer,

        /// <summary>
        /// Represents a boolean value, used for binary states like true/false, on/off, or enabled/disabled.
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents an enum value, which is used when the field can take on one of several predefined values.
        /// This allows for more descriptive and readable code by using named constants.
        /// </summary>
        Enum,

        /// <summary>
        /// Represents flags, which are bitwise values that can combine multiple true/false states into a single value.
        /// This is often used for defining multiple independent properties that can be toggled on/off.
        /// </summary>
        Flags,
    }

}
