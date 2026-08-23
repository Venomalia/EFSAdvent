using System;

namespace FSALib.AssetDefinitions
{
    /// <summary>
    /// Defines how a tile can be interacted with.
    /// </summary>
    [Flags]
    public enum InteractionFlags
    {
        /// <summary>
        /// The tile has no special interactions.
        /// </summary>
        None = 0,

        /// <summary>
        /// The tile triggers an effect when touched.
        /// </summary>
        Touch = 1 << 0,

        /// <summary>
        /// The tile can be picked up.
        /// </summary>
        Pickupable = 1 << 1,

        /// <summary>
        /// The tile can be affected by a sword slash.
        /// </summary>
        Slashable = 1 << 2,

        /// <summary>
        /// The tile can be interacted with by an entity using the Pegasus Boots.
        /// </summary>
        Dashable = 1 << 3,

        /// <summary>
        /// The tile can be affected by fire using a lantern or Fire Rod.
        /// </summary>
        Burnable = 1 << 4,

        /// <summary>
        /// The tile can be affected by a bomb.
        /// </summary>
        Bombable = 1 << 5,

        /// <summary>
        /// The tile can be affected by the hammer.
        /// </summary>
        Hammerable = 1 << 6,

        /// <summary>
        /// The tile can be dug.
        /// </summary>
        Diggable = 1 << 7,

        /// <summary>
        /// The tile can be interacted with using a slingshot or bow.
        /// </summary>
        Projectile = 1 << 8,

        /// <summary>
        /// The tile can be destroyed by slashing, burning, bombing, or dashing into it.
        /// </summary>
        Destructible = Slashable | Burnable | Bombable | Dashable,

        /// <summary>
        /// The tile can be destroyed by any attack.
        /// </summary>
        Fragile = Destructible | Hammerable | Projectile,

        /// <summary>
        /// Responds to a “PNPC” tile changes.
        /// </summary>
        GBARewriter = 1 << 10,
    }
}
