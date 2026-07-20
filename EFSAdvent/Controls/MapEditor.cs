using FSALib;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace EFSAdvent.Controls
{
    public sealed partial class MapEditor : UserControl
    {
        private readonly Stopwatch mouseHold = new Stopwatch();
        private MapLayout Map;
        private Stage Level;
        public MapEditor() => InitializeComponent();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedRoomID
        {
            get => (int)MapRoomNumberInput.Value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public (int X, int Y) SelectedRoomCoordinates
        {
            get => mapPictureBox.SelectedRoomCoordinates;
            set
            {
                mapPictureBox.SelectedRoomCoordinates = value;
                MapRoomNumberInput.Value = mapPictureBox.SelectedRoomID;
                SelectedRoomCoordinatesChanged?.Invoke(this, this);
            }
        }

        public event EventHandler<MapEditor> LoadRoom;

        public event EventHandler<MapEditor> NewRoom;

        public event EventHandler<MapEditor> RemoveRoom;

        public event EventHandler<MapEditor> SelectedRoomCoordinatesChanged;

        public void SetMap(MapLayout map, Stage level)
        {
            // Load songs from assets
            if (MapVariableMusicComboBox.Items.Count == 0)
            {
                int songindex = 0;
                foreach (var song in FSALib.Assets.Songs)
                {
                    if (songindex == song.Key)
                    {
                        MapVariableMusicComboBox.Items.Add(song.Value);
                    }
                    else
                    {
                        MapVariableMusicComboBox.Items.Add(songindex);
                    }
                    songindex++;
                }
            }

            // Load tilesets from assets
            if (MapVariableTileSheetComboBox.Items.Count == 0)
            {
                int tilesetsindex = 0;
                foreach (var tileset in FSALib.Assets.Tilesets)
                {
                    if (tilesetsindex == tileset.Key)
                    {
                        MapVariableTileSheetComboBox.Items.Add(tileset.Value.Name);
                    }
                    else
                    {
                        MapVariableTileSheetComboBox.Items.Add(tilesetsindex);
                    }
                    tilesetsindex++;
                }
            }

            if (Map != null)
                Map.PropertyChanged -= Map_PropertyChanged;

            Map = map;
            Map.PropertyChanged += Map_PropertyChanged;


            Level = level;
            mapPictureBox.SetMap(map);
            SelectedRoomCoordinates = (Map.StartX, Map.StartY);
            LoadMapVariable();
        }

        private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Rooms")
                LoadMapVariable();
        }

        private void MapRoomNumberInput_ValueChanged(object sender, EventArgs e)
        {
            int roomValue = (int)MapRoomNumberInput.Value;
            MapRoomRemoveButton.Enabled = roomValue != FSALib.MapLayout.EMPTY_ROOM_VALUE;
            MapRoomNewButton.Enabled = true;

            int selectedRoom = mapPictureBox.SelectedRoomID;
            bool roomExists = MapRoomRemoveButton.Enabled = MapRoomLoadButton.Enabled = roomValue != -1 && Level.Rooms[roomValue] != null;

            MapRoomSetButton.Enabled = roomExists && MapRoomNumberInput.Value != selectedRoom;

            if (Map.IsShadowBattle)
                LoadMapVariable();
        }


        private void MapRoomSetButton_Click(object sender, EventArgs e)
        {
            int roomID = (int)MapRoomNumberInput.Value;
            mapPictureBox.SelectedRoomID = roomID;
            MapRoomRemoveButton.Enabled = roomID != FSALib.MapLayout.EMPTY_ROOM_VALUE;
        }

        private void MapRoomLoadButton_Click(object sender, EventArgs e) => LoadRoom(sender, this);

        private void MapRoomNewButton_Click(object sender, EventArgs e)
        {
            int newRoomNumber = (int)MapRoomNumberInput.Value;
            newRoomNumber = newRoomNumber == MapLayout.EMPTY_ROOM_VALUE || Level.IsRoomInUse(newRoomNumber) ? Level.GetNextFreeRoom() : newRoomNumber;
            MapRoomNumberInput.Value = mapPictureBox.SelectedRoomID = newRoomNumber;
            NewRoom(sender, this);
            MapRoomRemoveButton.Enabled = true;
        }

        private void MapRoomRemoveButton_Click(object sender, EventArgs e)
        {
            int selectedRoom = mapPictureBox.SelectedRoomID;
            byte roomToRemove = (byte)MapRoomNumberInput.Value;

            // Prompt user before removing room from the map
            if (roomToRemove == selectedRoom)
            {
                var result = MessageBox.Show(
                    $"Do you want to remove room {roomToRemove} from the map layout?",
                    $"Remove room {roomToRemove} from layout?",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (result != DialogResult.OK && result != DialogResult.Yes)
                    return;

                mapPictureBox.SelectedRoomID = MapLayout.EMPTY_ROOM_VALUE;
            }
            MapRoomRemoveButton.Enabled = false;

            // If room exists and is not used in the map, offer to delete the actual room files
            if (Level.Rooms[roomToRemove] != null && !Level.IsRoomInUse(roomToRemove))
            {
                RemoveRoom(sender, this);
            }
        }

        private void MapPictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MapRoomLoadButton.Enabled)
                LoadRoom.Invoke(sender, this);
        }

        private void MapPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseHold.Restart(); // for MapPictureBox_MouseUp
            int roomWidthInPixels = mapPictureBox.Width / Map.XDimension;
            int roomHeightInPixels = mapPictureBox.Height / Map.YDimension;

            //When the user clicks in the map load the value of the clicked room into the edit box
            SelectedRoomCoordinates = (e.X / roomWidthInPixels, e.Y / roomHeightInPixels);

            switch (e.Button)
            {
                case MouseButtons.Right:
                    MapPictureBox_MouseDoubleClick(sender, e);
                    return;
                case MouseButtons.Middle:
                    if (!Map.IsShadowBattle)
                    {
                        Map.StartX = SelectedRoomCoordinates.X;
                        Map.StartY = SelectedRoomCoordinates.Y;
                    }
                    return;
            }
        }

        private void MapPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            // Prevents accidental moving
            if (mouseHold.ElapsedMilliseconds < 500)
                return;

            int roomWidthInPixels = mapPictureBox.Width / Map.XDimension;
            int roomHeightInPixels = mapPictureBox.Height / Map.YDimension;
            switch (e.Button)
            {
                case MouseButtons.Left:

                    (int X, int Y) upCoordinates = (e.X / roomWidthInPixels, e.Y / roomHeightInPixels);

                    if (upCoordinates.X < 0 || upCoordinates.Y < 0 || upCoordinates.X >= Map.XDimension || upCoordinates.Y >= Map.YDimension)
                        return;

                    if (upCoordinates != mapPictureBox.SelectedRoomCoordinates)
                    {
                        int downRoom = mapPictureBox.SelectedRoomID;
                        int upRoom = Map[upCoordinates.X, upCoordinates.Y];

                        if (downRoom != upRoom)
                        {
                            mapPictureBox.SelectedRoomID = upRoom;
                            Map[upCoordinates.X, upCoordinates.Y] = downRoom;
                        }
                        SelectedRoomCoordinates = upCoordinates;
                    }
                    break;
            }
        }

        private bool _IgnoreVariablechanges;

        private void MapVariable_Changed(object sender, EventArgs e)
        {
            if (_IgnoreVariablechanges)
                return;

            Map.PropertyChanged -= Map_PropertyChanged;

            var properties = Map.GetRoomProperties(SelectedRoomID);
            if (!Level.Map.IsShadowBattle)
            {
                Map.StartX = (int)MapVariableStartX.Value;
                Map.StartY = (int)MapVariableStartY.Value;
            }
            properties.BackgroundMusicId = MapVariableMusicComboBox.SelectedIndex;
            properties.ShowE3Banner = (int)MapVariableE3Banner.Value;
            properties.TileSheetId = MapVariableTileSheetComboBox.SelectedIndex;
            properties.NPCSheetID = (int)MapVariableNPCSheetID.Value;
            properties.OverlayTextureId = (int)MapVariableOverlay.Value;
            properties.Unknown = (int)MapVariableUnknown2.Value;
            properties.DisallowTingle = (int)MapVariableDisallowTingle.Value;

            Map.PropertyChanged += Map_PropertyChanged;
        }

        private void LoadMapVariable()
        {
            _IgnoreVariablechanges = true;
            var properties = Map.GetRoomProperties(SelectedRoomID);

            MapVariableStartY.Enabled = MapVariableStartX.Enabled = !Map.IsShadowBattle;
            SetMapVariableInput(MapVariableStartX, Map.StartX);
            SetMapVariableInput(MapVariableStartY, Map.StartY);
            MapVariableMusicComboBox.SelectedIndex = properties.BackgroundMusicId;
            SetMapVariableInput(MapVariableE3Banner, properties.ShowE3Banner);
            SetMapVariableInput(MapVariableOverlay, properties.OverlayTextureId);
            MapVariableTileSheetComboBox.SelectedIndex = properties.TileSheetId;
            SetMapVariableInput(MapVariableNPCSheetID, properties.NPCSheetID);
            SetMapVariableInput(MapVariableUnknown2, properties.Unknown);
            SetMapVariableInput(MapVariableDisallowTingle, properties.DisallowTingle);

            MapVariablesGroupBox.Text = $"Map{Map.Index}";
            _IgnoreVariablechanges = false;

            static void SetMapVariableInput(NumericUpDown input, int value)
            {
                try
                {
                    input.Value = value;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Trace.WriteLine($"{input.Name} has an invalid value of {value}, setting to 0.");
                    input.Value = 0;
                }
            }
        }
    }
}
