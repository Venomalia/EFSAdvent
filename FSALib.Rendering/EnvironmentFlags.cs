using System;

namespace FSALib.Rendering
{
    [Flags]
    public enum EnvironmentFlags
    {
        Nothing = 0,
        LostForest = 1 << 0,
        CaveBG = 1 << 1,
        PiramidBG = 1 << 4,
        TowerBG = 1 << 5,
        Snowstorm = 1 << 6,
        Snow = 1 << 7,
        RainA = 1 << 9,
        RainB = 1 << 10,
        CloudMist = 1 << 11,
        MapBG = 1 << 12,
        MoriBG = 1 << 13,
        LavaEffect = 1 << 14,
        WaterEffect = 1 << 15,
        Fog = 1 << 16,
        Clouds = 1 << 17,
        DarkFog = 1 << 21,
        DarkBG = 1 << 22,
    }
}
