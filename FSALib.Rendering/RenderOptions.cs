using System;

namespace FSALib.Rendering
{
    [Flags]
    public enum RenderOptions
    {
        Nothing = 0,
        BaseLayer = 1 << 0,
        Actors = 1 << 1,
        TopLayer = 1 << 2,
        Overlay = 1 << 3,
        TileChanges = 1 << 4,
        Collision = 1 << 5,
        Environment = 1 << 6,

        Layers = BaseLayer | TopLayer,
    }
}
