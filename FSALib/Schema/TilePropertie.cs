using AuroraLib.Core.Format.Identifier;

namespace FSALib.Schema
{
    /// <summary>
    /// Represents the properties associated with a tile in the game.
    /// </summary>
    public sealed class TilePropertie : ISchema
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <summary>
        /// Required actor that must be placed for the tile to work.
        /// </summary>
        public Identifier32? RequiredActorID { get; set; }

        /// <summary>
        /// The default value of the actor variable that the required actor should have. 
        /// </summary>
        public uint ActorValue { get; set; }

        /// <summary>
        /// The tile should always be placed on the top layer.
        /// </summary>
        public bool PlaceAlwaysOnTopLayer { get; set; }

        public TilePropertie()
            => Name = Description = string.Empty;
    }
}
