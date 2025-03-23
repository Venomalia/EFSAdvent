using System.ComponentModel;

namespace FSALib
{
    /// <summary>
    /// Represents the properties of a shadow battle room.
    /// </summary>
    public sealed class ShadowMapProperties : MapProperties
    {
        private int roomIndex = Map.EMPTY_ROOM_VALUE;
        private int unknown2;

        /// <summary>
        /// Gets or sets the index of the room in the shadow battle map.
        /// </summary
        public int RoomIndex
        {
            get => roomIndex;
            set => OnPropertyChanged(ref roomIndex, value, nameof(RoomIndex));
        }

        public int Unknown2
        {
            get => unknown2;
            set => OnPropertyChanged(ref unknown2, value, nameof(Unknown2));
        }
    }

    /// <summary>
    /// Represents the base properties of a map.
    /// </summary>
    public abstract class MapProperties : INotifyPropertyChanged
    {
        private int backgroundMusicId;
        private int showE3Banner;
        private int tileSheetId;
        private int nPCSheetID;
        private int overlayTextureId;
        private int unknown;
        private int disallowTingle;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the ID of the background music associated with the map.
        /// </summary>
        public int BackgroundMusicId
        {
            get => backgroundMusicId;
            set => OnPropertyChanged(ref backgroundMusicId, value, nameof(BackgroundMusicId));
        }

        public int ShowE3Banner
        {
            get => showE3Banner;
            set => OnPropertyChanged(ref showE3Banner, value, nameof(ShowE3Banner));
        }

        /// <summary>
        /// Gets or sets the ID of the tile sheet used for rendering the map.
        /// </summary>
        public int TileSheetId
        {
            get => tileSheetId;
            set => OnPropertyChanged(ref tileSheetId, value, nameof(TileSheetId));
        }

        /// <summary>
        /// Gets or sets the ID of the NPC sprite sheet associated with the map.
        /// </summary>
        public int NPCSheetID
        {
            get => nPCSheetID;
            set => OnPropertyChanged(ref nPCSheetID, value, nameof(NPCSheetID));
        }

        /// <summary>
        /// Gets or sets the ID of the overlay texture applied to the map.
        /// </summary>
        public int OverlayTextureId
        {
            get => overlayTextureId;
            set => OnPropertyChanged(ref overlayTextureId, value, nameof(OverlayTextureId));
        }

        public int Unknown
        {
            get => unknown;
            set => OnPropertyChanged(ref unknown, value, nameof(Unknown));
        }

        /// <summary>
        /// Gets or sets a value indicating whether Tingle is disallowed on this map.
        /// </summary>
        public int DisallowTingle
        {
            get => disallowTingle;
            set => OnPropertyChanged(ref disallowTingle, value, nameof(DisallowTingle));
        }

        protected void OnPropertyChanged(ref int field, int value, string propertyName)
        {
            if (field != value)
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}