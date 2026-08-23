using AuroraLib.Core.Format.Identifier;
using System;
using System.Diagnostics;

namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Represents the properties associated with a tile in the game.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public sealed class TilePropertyDefinition : IAssetDefinition
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public AssetFlags AssetState { get; set; }

        /// <summary>
        /// The mirrored version of the tile.
        /// </summary>
        public ushort MirrorTile { get; set; }

        /// <summary>
        /// Gets or sets the surface type of the tile.
        /// </summary>
        public SurfaceType Surface { get; set; }

        /// <summary>
        /// Gets or sets the collision mask of the tile.
        /// </summary>
        public TileCollision Collision { get; set; }

        /// <summary>
        /// Gets or sets the additional properties of the tile.
        /// </summary>
        public TileProperties Properties { get; set; }

        /// <summary>
        /// Defines how the tile can be interacted with.
        /// </summary>
        public InteractionFlags Interaction { get; set; }

        /// <summary>
        /// Gets or sets the tile to use after an interaction.
        /// </summary>
        public ushort InteractionTile { get; set; }

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

        public TilePropertyDefinition()
            => Name = Description = string.Empty;
    }
}
