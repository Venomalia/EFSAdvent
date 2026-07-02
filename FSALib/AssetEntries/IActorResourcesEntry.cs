using System.Collections.Generic;

namespace FSALib.AssetEntries
{
    public interface IActorResourcesEntry : IAssetEntry
    {
        /// <summary>
        /// Indicates whether the actor has no associated GBA sprite.
        /// </summary>
        bool NoGBASprite { get; set; }

        /// <summary>
        /// A list of level resources required for this actor, such as images.
        /// </summary>
        List<string>? Resources { get; set; }
    }
}