using System;

namespace FSALib
{
    /// <summary>
    /// Represents a room in the game, which contains multiple layers and actors.
    /// </summary>
    public sealed class Room
    {
        public const int LAYER = 8, LAYERLEVEL = 2;

        private readonly Layer[] _layers = new Layer[LAYER * LAYERLEVEL];

        /// <summary>
        /// A collection of actors in the room (e.g., players, NPCs, enemies, etc.)
        /// </summary>
        public readonly ActorList Actors = new ActorList();

        /// <summary>
        /// The index of the room. This could be used to identify or reference the room in the game world.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the full set of layers in the room.
        /// </summary>
        public ReadOnlySpan<Layer> Layers => _layers.AsSpan();

        /// <summary>
        /// Gets the base layers of the room (Tile Layer 1, typically the background elements).
        /// These layers contain the background tiles like floors and walls.
        /// </summary>
        public ReadOnlySpan<Layer> BaseLayers => _layers.AsSpan(0, LAYER);

        /// <summary>
        /// Gets the top layers of the room (Tile Layer 2, the layers above the player).
        /// These layers are used for elements that should be drawn above the player.
        /// </summary>
        public ReadOnlySpan<Layer> TopLayers => _layers.AsSpan(LAYER);

        public Room()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i] = new Layer();
            }
        }

        /// <summary>
        /// Mirrors the room horizontally.
        /// </summary>
        public void Mirror()
        {
            foreach (var layer in _layers)
            {
                layer.Mirror();
            }
            Actors.Mirror();
        }
    }
}
