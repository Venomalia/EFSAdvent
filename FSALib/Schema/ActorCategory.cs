namespace FSALib.Schema
{
    /// <summary>
    /// Defines different categories of actors in the game.
    /// </summary>
    public enum ActorCategory
    {
        /// <summary>
        /// Objects that can be placed in the level, such as decorations or interactive elements.
        /// </summary>
        Object,

        /// <summary>
        /// Objects that are fixed to the tile grid or interact with tiles,  
        /// such as environmental elements, or tile-based mechanics like a Tile Rewriter.
        /// </summary>
        TileObject,

        /// <summary>
        /// Collectible or usable items, such as keys and force gems.
        /// </summary>
        Item,

        /// <summary>
        /// Non-playable characters, including friendly and neutral NPCs.
        /// </summary>
        NPC,

        /// <summary>
        /// Hostile entities that the player can fight.
        /// </summary>
        Enemy,

        /// <summary>
        /// Powerful enemies that act as main bosses.
        /// </summary>
        Boss,

        /// <summary>
        /// Game properties such as room propertys, player spawn points or environmental triggers.
        /// </summary>
        Property,

        /// <summary>
        /// An actor used for scripted events or cutscenes.
        /// </summary>
        Cutscene,

        /// <summary>
        /// System-managed objects that are not directly placed in the level, such as the player or projectiles.
        /// </summary>
        Intern
    }
}
