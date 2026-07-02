namespace FSALib.AssetEntries
{
    /// <summary>
    /// Defines how an actor variant is rendered for a specific actor state.
    /// </summary>
    public sealed class ActorRenderVariant
    {
        /// <summary>
        /// Horizontal offset applied when rendering the sprite.
        /// </summary>
        public short XOffset { get; set; } = 0;

        /// <summary>
        /// Vertical offset applied when rendering the sprite.
        /// </summary>
        public short YOffset { get; set; } = 0;

        /// <summary>
        /// Index of the sprite used to render this actor variant.
        /// </summary>
        public short SpriteIndex { get; set; } = -1;

        /// <summary>
        /// Index of the sprite list containing the sprite.
        /// </summary>
        public ushort SpriteListIndex { get; set; } = 0;

        /// <summary>
        /// Index of the palette used as the replacement source (0-15).
        /// </summary>
        public sbyte ReplacementPaletteIndex { get; set; } = -1;

        /// <summary>
        /// Index of the palette to replace with <see cref="ReplacementPaletteIndex"/>.
        /// </summary>
        public sbyte TargetPaletteIndex { get; set; } = -1;

        /// <summary>
        /// BTI texture file used to render this actor variant as a bitmap instead of a sprite-based representation.
        /// </summary>
        public string BtiFile { get; set; } = string.Empty;
    }
}
