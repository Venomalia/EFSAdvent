using System;

namespace FSALib
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Room
    {
        public const int LAYER = 8, LAYERLEVEL = 2;

        private readonly Layer[] _layers = new Layer[LAYER * LAYERLEVEL];
        public readonly ActorList Actors = new ActorList();

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlySpan<Layer> Layers => _layers.AsSpan();

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlySpan<Layer> BaseLayers => _layers.AsSpan(0, LAYER);

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlySpan<Layer> TopLayers => _layers.AsSpan(LAYER);

        public Room()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i] = new Layer();
            }
        }
    }
}
