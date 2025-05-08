using EFSAdvent.FourSwords;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace EFSAdvent.Controls
{
    public sealed partial class MapEditor : UserControl
    {
        private FSALib.Map Map;
        private Level Level;

        public MapEditor()
        {
            InitializeComponent();
        }

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

        public event EventHandler<MapEditor> SelectedRoomCoordinatesChanged;

        public void SetMap(FSALib.Map map, Level level)
        {
            // Load Actor from Assets
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
            MapRoomRemoveButton.Enabled = roomValue != FSALib.Map.EMPTY_ROOM_VALUE;
            MapRoomNewButton.Enabled = true;

            int selectedRoom = mapPictureBox.SelectedRoomID;
            int currentRoom = Level.Room.Index;
            bool roomExists = MapRoomLoadButton.Enabled = Level.RoomExists(roomValue);

            MapRoomSetButton.Enabled = roomExists && MapRoomNumberInput.Value != selectedRoom;

            if (roomExists)
            {
                if (MapRoomNumberInput.Value != currentRoom)
                {
                    MapRoomRemoveButton.Enabled = true;
                }
                else
                {
                    MapRoomRemoveButton.Enabled = !Level.IsRoomInUse((int)MapRoomNumberInput.Value);
                }
            }
            else
            {
                MapRoomRemoveButton.Enabled = false;
            }

            if (Map.IsShadowBattle)
                LoadMapVariable();
        }


        private void MapRoomSetButton_Click(object sender, EventArgs e)
        {
            int roomID = (int)MapRoomNumberInput.Value;
            mapPictureBox.SelectedRoomID = roomID;
            MapRoomRemoveButton.Enabled = roomID != FSALib.Map.EMPTY_ROOM_VALUE;
        }

        private void MapRoomLoadButton_Click(object sender, EventArgs e) => LoadRoom(sender, this);

        private void MapRoomNewButton_Click(object sender, EventArgs e)
        {
            int newRoomNumber = (int)MapRoomNumberInput.Value;
            newRoomNumber = Level.IsRoomInUse(newRoomNumber) ? Level.GetNextFreeRoom() : newRoomNumber;
            MapRoomNumberInput.Value = mapPictureBox.SelectedRoomID = newRoomNumber;
            NewRoom(sender, this);
        }

        private void MapRoomRemoveButton_Click(object sender, EventArgs e)
        {
            int selectedRoom = mapPictureBox.SelectedRoomID;
            byte roomToRemove = (byte)MapRoomNumberInput.Value;

            // Prevent deleting the currently loaded room
            if (Level.Room.Index == selectedRoom)
            {
                MessageBox.Show(
                    "The currently loaded room cannot be deleted.",
                    "Action Not Allowed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            // Prompt user before removing room from the map
            if (roomToRemove == selectedRoom)
            {
                var result = MessageBox.Show(
                    $"Do you want to remove room {roomToRemove} from the map?",
                    $"Remove room {roomToRemove} from map?",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (result != DialogResult.OK && result != DialogResult.Yes)
                    return;

                mapPictureBox.SelectedRoomID = FSALib.Map.EMPTY_ROOM_VALUE;
            }
            MapRoomRemoveButton.Enabled = false;

            // If room exists and is not used in the map, offer to delete the actual room files
            if (Level.RoomExists(roomToRemove) && !Level.IsRoomInUse(roomToRemove))
            {
                var result = MessageBox.Show(
                    $"Room {roomToRemove} is not used anywhere in the level map.\n\n" +
                    "Do you want to delete it completely? This action cannot be undone!",
                    $"Delete room {roomToRemove} permanently?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    Level.DeleteRoom(roomToRemove);
                }
            }
        }

        private void MapPictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MapRoomLoadButton.Enabled)
                LoadRoom.Invoke(sender, this);
        }

        private void MapPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
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
            properties.TileSheetId = (int)MapVariableTileSheet.Value;
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
            SetMapVariableInput(MapVariableTileSheet, properties.TileSheetId);
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

        private void MapSaveButton_Click(object sender, EventArgs e)
        {
            Level.SaveMap();
        }
    }
}
