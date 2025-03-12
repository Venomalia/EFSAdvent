using EFSAdvent.FourSwords;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace EFSAdvent
{
    public partial class Form1 : Form
    {
        private const string VERSION = "1.8";
        private const string BaseTitel = "EFSAdvent " + VERSION + " [Venomalia]";
        private const string WikiUrl = "https://github.com/Venomalia/EFSAdvent/wiki";
        private const string SourceCodeUrl = "https://github.com/Venomalia/EFSAdvent";

        const int ACTOR_PIXELS_PER_COORDINATE = 8;
        const string DEFAULT_SPRITE = "ActorDefault";
        const int LAYER_DIMENSION_IN_PIXELS = Layer.DIMENSION * TILE_DIMENSION_IN_PIXELS;
        const int MAP_ROOM_DIMENSION_IN_PIXELS = 20;
        const int TILE_DIMENSION_IN_PIXELS = 16;

        Bitmap mapBitmap, tileSheetBitmap, tileSheetBitmapGBA, roomLayerBitmap, brushTileBitmap, actorLayerBitmap, currentActorBitmap, overlayBitmap;
        Graphics mapGraphics, roomLayerGraphics, actorLayerGraphics;

        private Level _level;
        private History _history;
        private TileBrush _tileBrush;
        private Rectangle _tileSelection;
        private (int x, int y) _tileSelectionOrigin;
        private bool _ignoreMapVariableUpdates = false;
        private Logger _logger;

        byte currentRoomNumber;
        (int x, int y) selectedRoomCoordinates;
        (int x, int y) lastActorCoordinates;

        int? actorMouseDownOnIndex;

        private readonly string[] ACTOR_NAMES;
        private readonly Dictionary<string, Bitmap> ACTOR_SPRITES = new Dictionary<string, Bitmap>();
        private readonly Dictionary<int, string> TILE_INFO = new Dictionary<int, string>();

        private readonly HashSet<string> V4ACTORS = new HashSet<string>();
        private readonly HashSet<string> V6ACTORS = new HashSet<string>();
        private readonly HashSet<string> V8ACTORS = new HashSet<string>();

        private string dataDirectory;

        public Form1()
        {
            InitializeComponent();
            this.Text = BaseTitel;
            _tileSelection.Width = _tileSelection.Height = 1;
            dataDirectory = "data";
            if (!Directory.Exists(dataDirectory))
            {
                dataDirectory = "..\\..\\data";
                if (!Directory.Exists(dataDirectory))
                {
                    dataDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data");
                    MessageBox.Show($"External resources from the \"{dataDirectory}\" folder are required for this tool to function.",
                                                 "Data folder not found!",
                                                 MessageBoxButtons.OK,
                                                 MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

            string SheetPath = Path.Combine(dataDirectory, "Tile Sheet 00.PNG");
            tileSheetBitmap = new Bitmap(SheetPath);
            tileSheetBitmapGBA = new Bitmap(SheetPath);
            tileSheetPictureBox.Image = tileSheetBitmap;

            ChangeOverlay(0);

            brushTileBitmap = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BrushTilePictureBox.Image = brushTileBitmap;

            roomLayerBitmap = new Bitmap(LAYER_DIMENSION_IN_PIXELS, LAYER_DIMENSION_IN_PIXELS, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            roomLayerGraphics = Graphics.FromImage(roomLayerBitmap);
            layerPictureBox.Image = roomLayerBitmap;

            mapBitmap = new Bitmap(mapPictureBox.Width, mapPictureBox.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            mapGraphics = Graphics.FromImage(mapBitmap);
            mapPictureBox.Image = mapBitmap;

            currentTileSheetComboBox.SelectedIndex = 0;
            BrushSizeComboBox.SelectedIndex = 0;

            actorLayerBitmap = new Bitmap(LAYER_DIMENSION_IN_PIXELS, LAYER_DIMENSION_IN_PIXELS, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            actorLayerGraphics = Graphics.FromImage(actorLayerBitmap);

            currentActorBitmap = new Bitmap(64, 64);
            ActorInfoPictureBox.Image = currentActorBitmap;

            _history = new History(500);
            _tileBrush = new TileBrush(_history);
            _logger = new Logger(loggerTextBox);

            string spriteFolder = Path.Combine(dataDirectory, "actorsprites");
            var spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var spritePath in spritePaths)
            {
                var sprite = new Bitmap(spritePath);
                ACTOR_SPRITES.Add(spritePath.Split(Path.DirectorySeparatorChar).Last().Split('.')[0], sprite);
            }

            // Load Actor Templates
            string templatesFolder = Path.Combine(dataDirectory, "actortemplates");
            if (Directory.Exists(templatesFolder))
            {
                foreach (var filePath in Directory.GetFiles(templatesFolder, "*.txt"))
                {
                    string categoryName = Path.GetFileNameWithoutExtension(filePath);
                    ToolStripMenuItem categoryItem = new ToolStripMenuItem(categoryName);

                    foreach (var line in File.ReadLines(filePath))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            categoryItem.DropDownItems.Add(new ToolStripSeparator());
                            continue;
                        }

                        var parts = line.Split(';');
                        if (parts.Length < 2)
                            continue;

                        string actorName = parts[0].Trim();
                        string actorCode = parts[1].Trim();

                        ToolStripMenuItem actorItem = new ToolStripMenuItem(actorName);
                        actorItem.Tag = actorCode;
                        actorItem.Click += AddActorStripMenuItem_Click;
                        categoryItem.DropDownItems.Add(actorItem);
                    }

                    actorContextMenuStrip.Items.Add(categoryItem);
                }
            }

            // Load Actor Namelist
            string ActorNameListPath = Path.Combine(dataDirectory, "FSA Actor Namelist.txt");
            if (File.Exists(ActorNameListPath))
            {
                var names = File.ReadLines(ActorNameListPath);
                ACTOR_NAMES = names.Select(n => n.Trim()).ToArray();
                ActorNameComboBox.Items.AddRange(ACTOR_NAMES);
            }

            // Load V4 Typ Actors
            string V4ActorListPath = Path.Combine(dataDirectory, "V4 Typ Actors.txt");
            if (File.Exists(V4ActorListPath))
            {
                var names = File.ReadLines(V4ActorListPath);
                V4ACTORS = new HashSet<string>(names.Select(n => n.Trim()));
            }

            // Load V6 Typ Actors
            string V6ActorListPath = Path.Combine(dataDirectory, "V6 Typ Actors.txt");
            if (File.Exists(V6ActorListPath))
            {
                var names = File.ReadLines(V6ActorListPath);
                V6ACTORS = new HashSet<string>(names.Select(n => n.Trim()));
            }

            // Load V8 Typ Actors
            string V8ActorListPath = Path.Combine(dataDirectory, "V8 Typ Actors.txt");
            if (File.Exists(V8ActorListPath))
            {
                var names = File.ReadLines(V8ActorListPath);
                V8ACTORS = new HashSet<string>(names.Select(n => n.Trim()));
            }

            string TileInfoListPath = Path.Combine(dataDirectory, "Tile Data.txt");
            if (File.Exists(TileInfoListPath))
            {
                foreach (var line in File.ReadLines(TileInfoListPath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(':');
                    if (int.TryParse(parts[0], out int key))
                    {
                        TILE_INFO[key] = parts[1].Trim().Replace("\\n", Environment.NewLine);
                    }
                }
            }


            string musicListPath = Path.Combine(dataDirectory, "Music Name List.txt");
            if (File.Exists(musicListPath))
            {
                var names = File.ReadLines(musicListPath);
                foreach (var item in names.Select(n => n.Trim()))
                {
                    MapVariableMusicComboBox.Items.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < 40; i++)
                {
                    MapVariableMusicComboBox.Items.Add(i);
                }
            }
            ResetVarsForNewLevel();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog())
                return;

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a save location for the new level.";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return;

                string selectedPath = folderDialog.SelectedPath;
                int number = 9;
                string newLevelPath;
                do
                {
                    newLevelPath = Path.Combine(selectedPath, $"boss{++number:000}", "map");
                } while (Directory.Exists(newLevelPath));
                string newMapFilePath = Path.Combine(newLevelPath, $"map{number:000}.csv");
                OpenLevelFile(newMapFilePath);
            }
        }

        private void OpenLevel(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog())
                return;

            var openDialog = new OpenFileDialog
            {
                Filter = "CSV map files|*.csv"
            };

            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (!openDialog.FileName.EndsWith("csv"))
            {
                MessageBox.Show("File must be a *.csv");
                return;
            }
            OpenLevelFile(openDialog.FileName);
        }

        private void OpenLevelFile(string mapPath)
        {
            ResetVarsForNewLevel();
            _level = new Level(mapPath, _logger);
            _level.LoadMap();
            LoadMapVariable();
            DrawMap();

            //Get a string which is just the root bossxxx filepath for loading other files
            RootFolderPathTextBox.Text = mapPath.Remove(mapPath.LastIndexOf("\\map\\") + 1);
            this.Text = $"{BaseTitel} - {_level.Map.Name} - {(mapPath.EndsWith("_1.csv") ? "single " : "multi")}player map";
            layerPictureBox.Refresh();

            MapRoomLoadButton.Enabled = false;
            MapRoomNewButton.Enabled = false;
            MapRoomRemoveButton.Enabled = false;
            MapRoomSetButton.Enabled = true;
            MapSaveButton.Enabled = true;
            ExportMenuItem.Enabled = true;
            SaveMenuItem.Enabled = true;
            SaveAsMenuItem.Enabled = true;
            tabControl.Enabled = true;
            layerPictureBox.Enabled = true;
            rightSideGroupBox.Enabled = true;
            importToolStripMenuItem.Enabled = true;
            MapVariableMusicComboBox.Enabled = true;
            MapVariableE3Banner.Enabled = true;
            MapVariableOverlay.Enabled = true;
            MapVariableTileSheet.Enabled = true;
            MapVariableNPCSheetID.Enabled = true;
            MapVariableUnknown2.Enabled = true;
            MapVariableDisallowTingle.Enabled = true;
            MapRoomNumberInput.Enabled = true;

            MapVariableStartX.Enabled = !_level.Map.IsShadowBattle;
            MapVariableStartY.Enabled = !_level.Map.IsShadowBattle;

            MapRoomNumberInput.Value = _level.Map.GetRoomValue(_level.Map.StartX, _level.Map.StartY);
            LoadRoom(false);

        }

        private void LoadMapVariable()
        {
            _ignoreMapVariableUpdates = true;
            SetMapVariableInput(MapVariableStartX, _level.Map.StartX);
            SetMapVariableInput(MapVariableStartY, _level.Map.StartY);
            MapVariableMusicComboBox.SelectedIndex = _level.Map.BackgroundMusicId;
            SetMapVariableInput(MapVariableE3Banner, _level.Map.ShowE3Banner);
            SetMapVariableInput(MapVariableOverlay, _level.Map.OverlayTextureId);
            SetMapVariableInput(MapVariableTileSheet, _level.Map.TileSheetId);
            SetMapVariableInput(MapVariableNPCSheetID, _level.Map.NPCSheetID);
            SetMapVariableInput(MapVariableUnknown2, _level.Map.Unknown2);
            SetMapVariableInput(MapVariableDisallowTingle, _level.Map.DisallowTingle);
            MapVariablesGroupBox.Text = _level.Map.Name;
            ChangeTileSheet(_level.TileSheetId);
            _ignoreMapVariableUpdates = false;

            void SetMapVariableInput(NumericUpDown input, int value)
            {
                try
                {
                    input.Value = value;
                }
                catch (ArgumentOutOfRangeException)
                {
                    _logger.AppendLine($"{input.Name} has an invalid value of {value}, setting to 0.");
                    input.Value = 0;
                }
            }
        }

        private void ResetVarsForNewLevel()
        {
            roomLayerGraphics.Clear(Color.Transparent);
            _history.Reset();
            actorMouseDownOnIndex = null;
            currentRoomNumber = Map.EMPTY_ROOM_VALUE;
            selectedRoomCoordinates = (0, 0);
        }

        private void LoadRoom(object sender, EventArgs e) => LoadRoom(false);

        private void NewRoom(object sender, EventArgs e) => LoadRoom(true);

        private void LoadRoom(bool newRoom)
        {
            if (!newRoom && MapRoomNumberInput.Value == Map.EMPTY_ROOM_VALUE)
                return;
            if (ShowSaveChangesDialog(_level.Map.IsShadowBattle))
                return;

            actorAttributesgroupBox.Enabled = false;
            byte newRoomNumber = newRoom ? (_level.Map.IsRoomInUse((byte)MapRoomNumberInput.Value) ? (byte)_level.GetNextFreeRoom() : (byte)MapRoomNumberInput.Value) : (byte)MapRoomNumberInput.Value;
            if (_level.LoadRoom(newRoomNumber, newRoom))
            {
                currentRoomNumber = newRoomNumber;
                _history.Reset();
                BuildLayerActorList(false);

                if (_level.Map.IsShadowBattle)
                    LoadMapVariable();

                //Enable all the actor buttons now that data to work with exists
                actorDeleteButton.Enabled = true;
                actorsSaveButton.Enabled = true;
                actorsReloadButton.Enabled = true;
                actorLayerComboBox.Enabled = true;

                for (int i = 1; i < 16; i++)
                {
                    layersCheckList.SetItemChecked(i, false);
                }
                layersCheckList.SetItemChecked(0, true);
                layersCheckList.SetItemChecked(8, true);
                if (newRoom)
                {
                    MapRoomNumberInput.Value = newRoomNumber;
                    UpdateMapRoomNumber(newRoomNumber);
                    for (int y = 0; y < 24; y++)
                    {
                        for (int x = 0; x < Layer.DIMENSION; x++)
                        {
                            _level.Room.SetLayerTile(0, x, y, 432);
                        }
                    }
                }
                for (int i = 0; i < 16; i++)
                {
                    Color color = _level.Room.IsLayerEmpty(i) ? Color.Gray : Color.Black;
                    layersCheckList.SetItemColor(i, color);
                }
                layersCheckList.Refresh();

                if (autoSelectToolStripMenuItem.Checked)
                {
                    SelectLayerActors(true, 0);
                    actorLayerComboBox.SelectedIndex = 0;
                }
                else
                {
                    actorLayerComboBox.SelectedIndex = -1;
                }

                UpdateView();
            }
        }

        private void RemoveRoom(object sender, EventArgs e)
        {
            byte selectedRoom = _level.Map.GetRoomValue(selectedRoomCoordinates.x, selectedRoomCoordinates.y);

            if (MapRoomNumberInput.Value == selectedRoom)
            {
                UpdateMapRoomNumber(Map.EMPTY_ROOM_VALUE);
            }
            if (!_level.Map.IsRoomInUse((byte)MapRoomNumberInput.Value) && _level.RoomExists((byte)MapRoomNumberInput.Value) && _level.Room.RoomNumber != selectedRoom)
            {
                var result = MessageBox.Show($"The room {MapRoomNumberInput.Value} is not used in this level map should it be deleted completely? This cannot be undone!",
                                             $"Deleted room {MapRoomNumberInput.Value}?",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    File.Delete(Room.GetActorFilePath(RootFolderPathTextBox.Text, _level.Map.Number, (byte)MapRoomNumberInput.Value));
                    for (int layer = 0; layer < 8; layer++)
                    {
                        File.Delete(Room.GetLayerFilePath(RootFolderPathTextBox.Text, _level.Map.Number, (byte)MapRoomNumberInput.Value, 1, layer));
                        File.Delete(Room.GetLayerFilePath(RootFolderPathTextBox.Text, _level.Map.Number, (byte)MapRoomNumberInput.Value, 2, layer));
                    }
                }
            }
            // update
            MapRoomNumberInput.Value = MapRoomNumberInput.Value;

        }

        private void UpdateView(int? layer = null)
        {
            if (_level?.Room == null)
                return;

            roomLayerGraphics.Clear(Color.Transparent);
            for (int i = 0; i < 8; i++)
            {
                // Draw Layer
                for (int n = 0; n <= 8; n += 8)
                {
                    if ((layer == null && layersCheckList.GetItemChecked(i + n)) || (i + n) == layer)
                    {
                        DrawLayer(i + n);
                    }
                }

                // Draw Overlay
                if (displayOverlayToolStripMenuItem.Checked && i == 0)
                {
                    using (var graphics = Graphics.FromImage(roomLayerBitmap))
                    {
                        for (int x = 0; x < roomLayerBitmap.Width; x += overlayBitmap.Width)
                        {
                            for (int y = 0; y < roomLayerBitmap.Height - 128; y += overlayBitmap.Height)
                            {
                                graphics.DrawImage(overlayBitmap, x, y);
                            }
                        }
                    }
                }
            }
            DrawActors();
        }

        #region Map

        private unsafe void DrawMap()
        {
            byte roomColour;
            var bitmapLock = mapBitmap.LockBits(
                new Rectangle(0, 0, mapPictureBox.Width, mapPictureBox.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                mapBitmap.PixelFormat);

            byte roomValue;
            int roomWidthInPixels = mapPictureBox.Width / _level.Map.XDimension;
            int roomHeightInPixels = mapPictureBox.Height / _level.Map.YDimension;

            for (int y = 0; y < _level.Map.YDimension * roomHeightInPixels; y += roomHeightInPixels)
            {
                for (int x = 0; x < _level.Map.XDimension * roomWidthInPixels; x += roomWidthInPixels)
                {
                    roomValue = _level.Map.GetRoomValue(x / roomWidthInPixels, y / roomHeightInPixels);
                    roomColour = (byte)(roomValue == Map.EMPTY_ROOM_VALUE ? 0xFF : 0x00);
                    for (int px = 0; px < roomWidthInPixels; px++)
                    {
                        for (int py = 0; py < roomHeightInPixels; py++)
                        {
                            byte* pixel = (byte*)(bitmapLock.Scan0 + ((y + py) * bitmapLock.Stride) + ((x + px) * 4));
                            pixel[0] = pixel[1] = pixel[2] = roomColour;
                        }
                    }
                }
            }
            mapBitmap.UnlockBits(bitmapLock);

            var serif = new Font("Microsoft Sans Serif", 7);
            Brush brush = Brushes.White;

            for (int y = 0; y < _level.Map.YDimension * roomHeightInPixels; y += roomHeightInPixels)
            {
                for (int x = 0; x < _level.Map.XDimension * roomWidthInPixels; x += roomWidthInPixels)
                {
                    roomValue = _level.Map.GetRoomValue(x / roomWidthInPixels, y / roomHeightInPixels);
                    if (roomValue != Map.EMPTY_ROOM_VALUE)
                    {
                        //Draw the room number over the top of the room for clarity
                        mapGraphics.DrawString(Convert.ToString(roomValue), serif, brush, x, y);
                    }
                }
            }

            roomValue = _level.Map.GetRoomValue(_level.Map.StartX, _level.Map.StartY);
            mapGraphics.DrawString(Convert.ToString(roomValue), serif, Brushes.Crimson, _level.Map.StartX * roomWidthInPixels, _level.Map.StartY * roomHeightInPixels);

            MapPanel.Refresh();
        }

        private void SelectMapRoom(object sender, MouseEventArgs e)
        {

            int roomWidthInPixels = mapPictureBox.Width / _level.Map.XDimension;
            int roomHeightInPixels = mapPictureBox.Height / _level.Map.YDimension;
            //When the user clicks in the map load the value of the clicked room into the edit box
            selectedRoomCoordinates = (e.X / roomWidthInPixels, e.Y / roomHeightInPixels);
            byte roomValue = _level.Map.GetRoomValue(selectedRoomCoordinates.x, selectedRoomCoordinates.y);
            MapRoomNumberInput.Value = roomValue;
            MapRoomRemoveButton.Enabled = roomValue != Map.EMPTY_ROOM_VALUE;
            MapRoomNewButton.Enabled = true;

            CoridinatesTextBox.Clear();
            CoridinatesTextBox.AppendText($"Map coordinates: x{selectedRoomCoordinates.x} y{selectedRoomCoordinates.y}");
            DrawMapWithSelectedRoomHighlighted();
        }

        private void SelectRoomNumber(object sender, EventArgs e)
        {
            bool roomExists = MapRoomLoadButton.Enabled = _level.RoomExists((byte)MapRoomNumberInput.Value);
            byte selectedRoom = _level.Map.GetRoomValue(selectedRoomCoordinates.x, selectedRoomCoordinates.y);

            if (roomExists && MapRoomNumberInput.Value != selectedRoom)
            {
                MapRoomSetButton.Enabled = true;
                MapRoomRemoveButton.Enabled = !_level.Map.IsRoomInUse((byte)MapRoomNumberInput.Value);
            }
            else
            {
                MapRoomRemoveButton.Enabled = MapRoomSetButton.Enabled = false;
            }
        }

        private void DrawMapWithSelectedRoomHighlighted()
        {
            DrawMap();
            int roomWidthInPixels = mapPictureBox.Width / _level.Map.XDimension;
            int roomHeightInPixels = mapPictureBox.Height / _level.Map.YDimension;
            var brush = new SolidBrush(Color.FromArgb(100, 0, 255, 0));
            mapGraphics.FillRectangle(brush,
                                              selectedRoomCoordinates.x * roomWidthInPixels,
                                              selectedRoomCoordinates.y * roomHeightInPixels,
                                              roomWidthInPixels,
                                              roomHeightInPixels);
            MapPanel.Refresh();
        }

        private void mapPictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LoadRoom(sender, e);
        }

        private void UpdateMapRoomNumber(object sender, EventArgs e) => UpdateMapRoomNumber((byte)MapRoomNumberInput.Value);

        private void UpdateMapRoomNumber(byte roomID)
        {
            _level.Map.SetRoomValue(selectedRoomCoordinates.x, selectedRoomCoordinates.y, roomID);
            MapRoomRemoveButton.Enabled = roomID != Map.EMPTY_ROOM_VALUE;
            DrawMap();
            DrawMapWithSelectedRoomHighlighted();
        }

        private void SaveMap(object sender, EventArgs e)
        {
            _level.SaveMap();
        }

        private void UpdateMapVariables(object sender, EventArgs e)
        {
            if (sender is Control csender)
            {
                if (!_ignoreMapVariableUpdates)
                {
                    _level.Map.SetVariables(
                        (int)MapVariableStartX.Value,
                        (int)MapVariableStartY.Value,
                        (int)MapVariableMusicComboBox.SelectedIndex,
                        (int)MapVariableE3Banner.Value,
                        (int)MapVariableTileSheet.Value,
                        (int)MapVariableNPCSheetID.Value,
                        (int)MapVariableOverlay.Value,
                        (int)MapVariableUnknown2.Value,
                        (int)MapVariableDisallowTingle.Value
                    );
                }
                switch (csender.Name)
                {
                    case "MapVariableOverlay":
                        ChangeOverlay((int)MapVariableOverlay.Value);
                        UpdateView();
                        break;
                    case "MapVariableStartY":
                    case "MapVariableStartX":
                        DrawMap();
                        break;
                    case "MapVariableTileSheet":
                        ChangeTileSheet((int)MapVariableTileSheet.Value);
                        currentTileSheetComboBox.SelectedIndex = (int)MapVariableTileSheet.Value;
                        UpdateView();
                        break;
                    case "currentTileSheetComboBox":
                        MapVariableTileSheet.Value = currentTileSheetComboBox.SelectedIndex;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion Map

        #region Layers

        private MouseEventArgs ScaleEventToLayerRealSize(MouseEventArgs e)
        {
            int shortest = Math.Min(layerPictureBox.Width, layerPictureBox.Height);
            float scale = (float)LAYER_DIMENSION_IN_PIXELS / shortest;
            int widthOfset = (layerPictureBox.Size.Width - shortest) / 2;
            int heightOfset = (layerPictureBox.Size.Height - shortest) / 2;

            return new MouseEventArgs(e.Button, e.Clicks, (int)((e.X - widthOfset) * scale), (int)((e.Y - heightOfset) * scale), e.Delta);
        }

        private void layersPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs scaledEvent = ScaleEventToLayerRealSize(e);
            switch (tabControl.SelectedTab.TabIndex)
            {
                case 1: // Tile tab
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            DoTileAction(scaledEvent);
                            break;
                        case MouseButtons.Right:
                            UpdateClipboardSelection();
                            return;
                        case MouseButtons.None:
                            Rectangle position = new Rectangle(
                                scaledEvent.X / TILE_DIMENSION_IN_PIXELS * TILE_DIMENSION_IN_PIXELS,
                                scaledEvent.Y / TILE_DIMENSION_IN_PIXELS * TILE_DIMENSION_IN_PIXELS,
                                _tileBrush.Width * TILE_DIMENSION_IN_PIXELS,
                                _tileBrush.Height * TILE_DIMENSION_IN_PIXELS
                                );

                            if (_tileSelection.X != position.X || _tileSelection.Y != position.Y)
                            {
                                UpdateView();
                                if (position.X >= 0 && position.Y >= 0)
                                {
                                    DrawTileSelection(Color.White, position);
                                    _tileSelection.X = position.X;
                                    _tileSelection.Y = position.Y;
                                }
                                else
                                {
                                    _tileSelection.X = _tileSelection.Y = -1;
                                }
                            }
                            return;
                        default:
                            return;
                    }
                    break;
                case 2: // Actor tab
                    if (e.Button == MouseButtons.None)
                    {
                        return;
                    }
                    MoveActor();
                    return;
                default:

                    return;
            }

            void MoveActor()
            {
                if (!actorMouseDownOnIndex.HasValue || e.Button != MouseButtons.Left)
                {
                    return;
                }

                int newActorXCoord = scaledEvent.X / ACTOR_PIXELS_PER_COORDINATE;
                int newActorYCoord = scaledEvent.Y / ACTOR_PIXELS_PER_COORDINATE;

                if (newActorXCoord < 0) newActorXCoord = 0;
                if (newActorYCoord < 0) newActorYCoord = 0;
                if (newActorXCoord > ActorXCoordInput.Maximum) newActorXCoord = (int)ActorXCoordInput.Maximum;
                if (newActorYCoord > ActorYCoordInput.Maximum) newActorYCoord = (int)ActorYCoordInput.Maximum;

                _level.Room.SetActorLocation(actorMouseDownOnIndex.Value, newActorXCoord, newActorYCoord);

                _ignoreActorChanges = true;
                ActorXCoordInput.Value = newActorXCoord;
                ActorYCoordInput.Value = newActorYCoord;
                _ignoreActorChanges = false;

                UpdateView();
            }

            void UpdateClipboardSelection()
            {
                if (e.Button != MouseButtons.Right)
                {
                    return;
                }

                int eventX = (int)Math.Ceiling(scaledEvent.X / (float)TILE_DIMENSION_IN_PIXELS);
                int eventY = (int)Math.Ceiling(scaledEvent.Y / (float)TILE_DIMENSION_IN_PIXELS);
                int width = eventX - _tileSelectionOrigin.x;
                if (width <= 0)
                {
                    _tileSelection.X = _tileSelectionOrigin.x + width - 1;
                    _tileSelection.Width = Math.Abs(width) + 2;
                }
                else
                {
                    _tileSelection.X = _tileSelectionOrigin.x;
                    _tileSelection.Width = width;
                }

                int height = eventY - _tileSelectionOrigin.y;
                if (height <= 0)
                {
                    _tileSelection.Y = _tileSelectionOrigin.y + height - 1;
                    _tileSelection.Height = Math.Abs(height) + 2;
                }
                else
                {
                    _tileSelection.Y = _tileSelectionOrigin.y;
                    _tileSelection.Height = height;
                }

                UpdateView();
                Rectangle position = new Rectangle(
                    _tileSelection.X * TILE_DIMENSION_IN_PIXELS,
                    _tileSelection.Y * TILE_DIMENSION_IN_PIXELS,
                    _tileSelection.Width * TILE_DIMENSION_IN_PIXELS,
                    _tileSelection.Height * TILE_DIMENSION_IN_PIXELS
                    );
                DrawTileSelection(Color.Blue, position);
            }
        }

        private void DrawLayer(int layer)
        {
            // Is TV layer or GBA?
            Bitmap tileSheet = (layer == 0 || layer == 8) ? tileSheetBitmap : tileSheetBitmapGBA;

            ushort tile;
            for (int y = 0; y < Layer.DIMENSION; y++)
            {
                for (int x = 0; x < Layer.DIMENSION; x++)
                {
                    tile = _level.Room.GetLayerTile(layer, x, y).Value;

                    // Only used tiles must be drawn.
                    if (tile != 0)
                    {
                        DrawTile(roomLayerBitmap, tileSheet, x, y, tile);
                    }
                }
            }
        }

        private static unsafe void DrawTile(Bitmap roomLayer, Bitmap tileSheet, int x, int y, int tileNo)
        {
            int srcX = (tileNo % TILE_DIMENSION_IN_PIXELS) * TILE_DIMENSION_IN_PIXELS;
            int srcY = (tileNo / TILE_DIMENSION_IN_PIXELS) * TILE_DIMENSION_IN_PIXELS;
            int dstY = y * TILE_DIMENSION_IN_PIXELS;
            int dstX = x * TILE_DIMENSION_IN_PIXELS;

            BitmapData lockedTileSheet = tileSheet.LockBits(new Rectangle(srcX, srcY, 16, 16), System.Drawing.Imaging.ImageLockMode.ReadOnly, tileSheet.PixelFormat);
            BitmapData lockedRoomLayer = roomLayer.LockBits(new Rectangle(dstX, dstY, 16, 16), System.Drawing.Imaging.ImageLockMode.WriteOnly, roomLayer.PixelFormat);
            for (int py = 0; py < lockedTileSheet.Height; py++)
            {
                byte* srcRow = (byte*)lockedTileSheet.Scan0 + (py * lockedTileSheet.Stride);
                byte* dstRow = (byte*)lockedRoomLayer.Scan0 + (py * lockedRoomLayer.Stride);
                for (int px = 0; px < lockedRoomLayer.Width; px++)
                {
                    byte srcAlpha = srcRow[(px * 4) + 3];
                    if (srcAlpha == 255)
                    {
                        dstRow[px * 4] = srcRow[px * 4];
                        dstRow[(px * 4) + 1] = srcRow[(px * 4) + 1];
                        dstRow[(px * 4) + 2] = srcRow[(px * 4) + 2];
                        dstRow[(px * 4) + 3] = srcAlpha;
                    }
                    else if (srcAlpha > 0)
                    {
                        float srcAlphaNorm = srcAlpha / 255f;
                        float invAlpha = 1 - srcAlphaNorm;

                        dstRow[px * 4] = (byte)(srcRow[px * 4] * srcAlphaNorm + dstRow[px * 4] * invAlpha);
                        dstRow[(px * 4) + 1] = (byte)(srcRow[(px * 4) + 1] * srcAlphaNorm + dstRow[(px * 4) + 1] * invAlpha);
                        dstRow[(px * 4) + 2] = (byte)(srcRow[(px * 4) + 2] * srcAlphaNorm + dstRow[(px * 4) + 2] * invAlpha);
                        dstRow[(px * 4) + 3] = (byte)(srcAlpha + dstRow[(px * 4) + 3] * invAlpha);
                    }

                }
            }
            tileSheet.UnlockBits(lockedTileSheet);
            roomLayer.UnlockBits(lockedRoomLayer);
        }

        private void layersPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventArgs scaledEvent = ScaleEventToLayerRealSize(e);

            switch (tabControl.SelectedTab.TabIndex)
            {
                case 1: // Tile tab
                    DoTileAction(scaledEvent);
                    break;
                case 2: // Actor tab
                    SetMouseDownActor();
                    break;
                default:

                    break;
            }

            void SetMouseDownActor()
            {
                lastActorCoordinates = (scaledEvent.X / ACTOR_PIXELS_PER_COORDINATE, scaledEvent.Y / ACTOR_PIXELS_PER_COORDINATE);
                CoridinatesTextBox.Clear();
                CoridinatesTextBox.AppendText($"Actor coordinates: x{lastActorCoordinates.x} y{lastActorCoordinates.y}");

                if (e.Button != MouseButtons.Left)
                {
                    return;
                }

                actorMouseDownOnIndex = null;
                for (int i = 0; i < actorsCheckListBox.Items.Count; i++)
                {
                    if (actorsCheckListBox.GetItemChecked(i) == true)
                    {
                        var actor = _level.Room.GetActor(i);
                        bool isVisible = alwaysShowActorsToolStripMenuItem.Checked || GetHighestActiveLayerIndex() % 8 == actor.Layer || IsSelectedActor(actor);
                        if (isVisible && actor.XCoord == lastActorCoordinates.x && actor.YCoord == lastActorCoordinates.y)
                        {
                            actorMouseDownOnIndex = i;
                            SelectedActor(i);
                            return;
                        }
                    }
                }
            }
        }

        private void layersPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            //When the mouse button is released select no actor so that a new one can be selected
            if (tabControl.SelectedTab.TabIndex == 2)
            {
                actorMouseDownOnIndex = null;
            }
            else if (tabControl.SelectedTab.TabIndex == 1)
            {
                int? layer = GetHighestActiveLayerIndex();
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (layer.HasValue) UpdateLayerCheckListColor(layer.Value);
                        break;
                    case MouseButtons.Right:
                        if (layer.HasValue)
                        {
                            _tileBrush.Copy(_tileSelection, _level, layer.Value);
                            UpdateBrushTileBitmap();
                        }
                        break;
                    default:
                        break;
                }
            }
            //Thread.Sleep(400);
            UpdateView();
        }

        private void layerPictureBox_MouseLeave(object sender, EventArgs e)
        {
            if (_tileSelection.X != -1 || _tileSelection.Y != -1)
            {
                UpdateView();
                _tileSelection.X = _tileSelection.Y = -1;
            }
        }

        private void layerPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int? layerPosition = GetHighestActiveLayerIndex();
            if (!layerPosition.HasValue)
                layerPosition = -1;

            if (tabControl.SelectedTab.TabIndex != 1)
            {
                if (e.Delta < 0) // up
                {
                    if (layerPosition == -1)
                    {
                        layersCheckList.SetItemChecked(0, true);
                        layersCheckList.SetItemChecked(8, true);
                    }
                    else
                    {
                        layerPosition %= 8;
                        do
                        {
                            layerPosition++;
                            if (layerPosition == 8)
                                return;
                        } while (layersCheckList.GetItemColor(layerPosition.Value) == Color.Gray);

                        layersCheckList.SetItemChecked(layerPosition.Value, true);

                        layerPosition += 8;
                        if (!layersCheckList.GetItemChecked(layerPosition.Value))
                            layersCheckList.SetItemChecked(layerPosition.Value, true);
                    }
                }
                else if (e.Delta > 0 && layerPosition > 0 && layerPosition != 8)
                {
                    layersCheckList.SetItemChecked(layerPosition.Value, false);
                    layerPosition %= 8;
                    if (layersCheckList.GetItemChecked(layerPosition.Value))
                        layersCheckList.SetItemChecked(layerPosition.Value, false);

                    do
                    {
                        layerPosition--;
                        if (layerPosition == 0)
                            return;
                    } while (layersCheckList.GetItemColor(layerPosition.Value) == Color.Gray);

                    if (!layersCheckList.GetItemChecked(layerPosition.Value))
                        layersCheckList.SetItemChecked(layerPosition.Value, true);
                    layerPosition += 8;
                    if (!layersCheckList.GetItemChecked(layerPosition.Value))
                        layersCheckList.SetItemChecked(layerPosition.Value, true);
                }
            }
            else
            {
                if (e.Delta < 0 && layerPosition != 15) // up
                {
                    if (layerPosition == -1)
                    {
                        layersCheckList.SetItemChecked(0, true);
                    }
                    else
                    {
                        layerPosition = layerPosition < 8 ? layerPosition + 8 : layerPosition % 8 + 1;
                        layersCheckList.SetItemChecked(layerPosition.Value, true);
                    }
                }
                else if (e.Delta > 0 && layerPosition > 0)
                {
                    layersCheckList.SetItemChecked(layerPosition.Value, false);
                    if (layerPosition == 0)
                        return;

                    layerPosition = layerPosition < 8 ? layerPosition + 7 : layerPosition % 8;
                    if (!layersCheckList.GetItemChecked(layerPosition.Value))
                        layersCheckList.SetItemChecked(layerPosition.Value, true);
                }
            }
        }

        private void DoTileAction(MouseEventArgs scaledEvent)
        {
            int eventX = scaledEvent.X / TILE_DIMENSION_IN_PIXELS;
            int eventY = scaledEvent.Y / TILE_DIMENSION_IN_PIXELS;

            if (eventX < 0 || eventX >= Layer.DIMENSION || eventY < 0 || eventY >= Layer.DIMENSION)
            {
                return;
            }
            int? layer = GetHighestActiveLayerIndex();
            if (!layer.HasValue)
            {
                return;
            }

            //If right click set the brush tile to the clicked tile
            switch (scaledEvent.Button)
            {
                case MouseButtons.Left: //Change tiles
                    if (_tileBrush.Draw(_level, layer.Value, eventX, eventY))
                    {
                        UpdateView(layer);
                        buttonSaveLayers.Enabled = true;
                    }
                    break;
                case MouseButtons.Right: //Copy tiles
                    _tileSelection.X = _tileSelectionOrigin.x = eventX;
                    _tileSelection.Y = _tileSelectionOrigin.y = eventY;
                    _tileSelection.Height = _tileSelection.Width = 1;

                    //Gives the user some time to release the button
                    Thread.Sleep(100);
                    break;
                default:
                    return;
            }
            CoridinatesTextBox.Clear();
            CoridinatesTextBox.AppendText($"Tile coordinates: x{eventX} y{eventY}");
        }

        private int? GetHighestActiveLayerIndex()
        {
            for (int i = 15; i > 7; i += 7)
            {
                if (layersCheckList.GetItemChecked(i) == true)
                {
                    return i;
                }
                i -= 8;
                if (layersCheckList.GetItemChecked(i) == true)
                {
                    return i;
                }
            }
            return null;
        }

        private void DrawTileSelection(Color brushColor, Rectangle position)
        {
            var brush = new SolidBrush(Color.FromArgb(30, brushColor));
            var pen = new Pen(brushColor, 1f) { DashStyle = DashStyle.Dash };
            roomLayerGraphics.FillRectangle(brush, position);
            roomLayerGraphics.DrawRectangle(pen, position);
            layersPanel.Refresh();
        }

        private void UpdateLayerCheckListColor(int layer)
        {
            Color color = _level.Room.IsLayerEmpty(layer) ? Color.Gray : Color.Black;
            layersCheckList.SetItemColor(layer, color);
            layersCheckList.Refresh();
        }

        private void tilePictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            //When a TileSheet tile is clicked on load its value into the brush
            UpdateBrushTileBitmap((ushort)(((e.Y / TILE_DIMENSION_IN_PIXELS) * TILE_DIMENSION_IN_PIXELS) + (e.X / TILE_DIMENSION_IN_PIXELS)));
        }

        private void UpdateBrushTileBitmap(ushort tileID)
        {
            if (tileID > 1023)
            {
                return;
            }

            int brushWidth = BrushSizeComboBox.SelectedIndex + 1;
            _tileBrush.Set(tileID, brushWidth, brushWidth);

            UpdateBrushTileBitmap();
        }

        private void UpdateBrushTileBitmap()
        {
            BrushTileLabel.Text = Convert.ToString(_tileBrush.TileValue);
            Bitmap currentTileSheet = (Bitmap)tileSheetPictureBox.Image;
            brushTileBitmap.Clear(Color.White);
            DrawTile(brushTileBitmap, currentTileSheet, 0, 0, _tileBrush.TileValue);

            _logger.Clear();
            if (TILE_INFO.TryGetValue(_tileBrush.TileValue, out string info))
            {
                _logger.AppendText(info);
            }

            BrushTilePictureBox.Refresh();
        }

        private void BrushSizeComboBox_SelectionChangeCommitted(object sender, EventArgs e) => UpdateBrushTileBitmap(_tileBrush.TileValue);

        private void updateLayersButton_Click(object sender, EventArgs e) => UpdateView();

        private void LayersCheckList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Need to delay redraw because right now the newly checked layer won't have checked=true
            this.BeginInvoke((MethodInvoker)(() =>
            {
                UpdateView();
                UpdateTileSheetPictureBox();
            }
            ));
            layersCheckList.SelectedIndex = -1;
        }

        private void ChangeTileSheet(int tileSheetIndex)
        {
            string tileSheetPath = Path.Combine(dataDirectory, $"Tile Sheet {tileSheetIndex:D2}.PNG");
            string tileSheetPathGBA = Path.Combine(dataDirectory, $"Tile Sheet {tileSheetIndex:D2}_GBA.PNG");

            if (RedrawImage(tileSheetPath, tileSheetBitmap))
            {
                if (File.Exists(tileSheetPathGBA))
                    RedrawImage(tileSheetPathGBA, tileSheetBitmapGBA);
                else
                    RedrawImage(tileSheetPath, tileSheetBitmapGBA);

                UpdateTileSheetPictureBox();
            }
        }

        private void UpdateTileSheetPictureBox()
            => tileSheetPictureBox.Image = GetHighestActiveLayerIndex() % 8 == 0 ? tileSheetBitmap : tileSheetBitmapGBA;

        private static bool RedrawImage(string path, Bitmap bitmap)
        {
            if (!File.Exists(path))
                return false;

            using (var newBitmap = new Bitmap(path))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.FromArgb(0));
                graphics.DrawImage(newBitmap, 0, 0);
            }
            return true;
        }

        private void ChangeOverlay(int overlayIndex)
        {
            var tileSheetPath = Path.Combine(dataDirectory, $"Overlays\\filter{overlayIndex}.png");

            if (File.Exists(tileSheetPath))
            {
                overlayBitmap?.Dispose();
                overlayBitmap = new Bitmap(tileSheetPath);
            }
        }

        private void updateView_Click(object sender, EventArgs e) => UpdateView();

        private void buttonSaveLayers_Click(object sender, EventArgs e)
        {
            _level.SaveLayers();
            MessageBox.Show("Changes Saved");
        }

        private void tileSheetPictureBox_MouseEnter(object sender, EventArgs e)
        {
            tileSheetPictureBox.Focus(); // So mousewheel can be used to scroll
        }

        #endregion Layers

        #region Menu/Form

        private void oneXSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Size = new Size(974, 614);
        }

        private void twoXSizeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Size = new Size(1486, 1126);
        }

        private void ExportLevel(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog(true, "Save all data before exporting?"))
                return;

            var saveRarc = new SaveFileDialog
            {
                DefaultExt = ".arc",
                AddExtension = true,
                Filter = "RARC files|*.arc",
                FileName = $"boss{_level.Map.Number}"
            };

            if (saveRarc.ShowDialog() == DialogResult.OK)
            {
                string path = RootFolderPathTextBox.Text.Remove(RootFolderPathTextBox.Text.Length - 1);

                EnsureAllLayersAreEncoded(path);

                var packer = new RarcPacker();
                packer.CreateRarc(path, saveRarc.FileName);
            }
        }

        private void ExportRoomAsTmx_Click(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog(true, "Save all data before exporting?"))
                return;

            var saveTmx = new SaveFileDialog
            {
                DefaultExt = ".tmx",
                AddExtension = true,
                Filter = "Tiled map files|*.tmx;*.xml",
                FileName = $"boss{_level.Map.Number}_Room{_level.Room.RoomNumber}"
            };

            if (saveTmx.ShowDialog() == DialogResult.OK)
            {
                string tilesetSource = $"Tile Sheet {currentTileSheetComboBox.SelectedIndex:D2}.tsx";
                string tsxFilePath = Path.Combine(Path.GetDirectoryName(saveTmx.FileName), tilesetSource);
                ExportMapTilesetAsTsx(tsxFilePath);
                _level.Room.ExportAsTMX(saveTmx.FileName, tilesetSource);
            }
        }

        private void ImportRoomFromTmx_Click(object sender, EventArgs e)
        {
            var openTmx = new OpenFileDialog
            {
                DefaultExt = ".tmx",
                Filter = "Tiled map files|*.tmx;*.xml",
                FileName = $"boss{_level.Map.Number}_Room{_level.Room.RoomNumber}.tmx"
            };

            if (openTmx.ShowDialog() == DialogResult.OK)
            {
                _level.Room.ImportRoomFromTMX(openTmx.FileName);
                UpdateView();
            }
        }

        private void ExportMapTilesetAsTsx(string savePath)
        {
            string tiledBaseSheet = Path.Combine(dataDirectory, "TiledBaseSheet.xml");
            string tileSheetPath = Path.Combine(dataDirectory, $"Tile Sheet {currentTileSheetComboBox.SelectedIndex:D2}.PNG");
            Tiled.UpdateTilesetImage(tiledBaseSheet, tileSheetPath, savePath);
        }

        private void ExportViewAsPng(object sender, EventArgs e)
        {
            var savePng = new SaveFileDialog
            {
                DefaultExt = ".png",
                AddExtension = true,
                Filter = "Portable Network Graphics|*.png",
                FileName = $"boss{_level.Map.Number}_{currentRoomNumber}"
            };

            if (savePng.ShowDialog() == DialogResult.OK)
            {
                roomLayerBitmap.Save(savePng.FileName);
            }
        }

        private void ExportLevelAsPng(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog())
                return;

            var savePng = new SaveFileDialog
            {
                DefaultExt = ".png",
                AddExtension = true,
                Filter = "Portable Network Graphics|*.png",
                FileName = $"boss{_level.Map.Number}"
            };

            if (savePng.ShowDialog() == DialogResult.OK)
            {

                bool autoLoadActors = autoSelectToolStripMenuItem.Checked;
                byte lastRoom = currentRoomNumber;
                autoSelectToolStripMenuItem.Checked = sender is ToolStripItem csender && csender.Name == "mapAndAAspngToolStripMenuItem";

                int roomWidth = 512, roomHeight = 384; // Maße eines Raums
                int mapWidth = roomWidth * _level.Map.XDimension; // Gesamtbreite
                int mapHeight = roomHeight * _level.Map.YDimension; // Gesamthöhe

                using (Bitmap levelBitmap = new Bitmap(mapWidth, mapHeight))
                using (Graphics g = Graphics.FromImage(levelBitmap))
                {
                    for (int y = 0; y < _level.Map.YDimension; y++)
                    {
                        for (int x = 0; x < _level.Map.XDimension; x++)
                        {
                            byte roomValue = _level.Map.GetRoomValue(x, y);

                            if (roomValue != Map.EMPTY_ROOM_VALUE)
                            {
                                MapRoomNumberInput.Value = roomValue;
                                LoadRoom(false);

                                int drawX = x * roomWidth;
                                int drawY = y * roomHeight;

                                Rectangle sourceRect = new Rectangle(0, 0, roomWidth, roomHeight);
                                Rectangle destRect = new Rectangle(drawX, drawY, roomWidth, roomHeight);
                                g.DrawImage(roomLayerBitmap, destRect, sourceRect, GraphicsUnit.Pixel);
                            }
                        }
                    }

                    levelBitmap.Save(savePng.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }

                MapRoomNumberInput.Value = currentRoomNumber = lastRoom;
                autoSelectToolStripMenuItem.Checked = autoLoadActors;
                LoadRoom(false);
            }
        }

        private void ExportRoomsAsPng(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog())
                return;

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a save location for the images.";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return;

                bool autoLoadActors = autoSelectToolStripMenuItem.Checked;
                bool overlay = displayOverlayToolStripMenuItem.Checked;
                byte lastRoom = currentRoomNumber;
                autoSelectToolStripMenuItem.Checked = sender is ToolStripItem csender && csender.Name == "allRoomsAndActorsAspngToolStripMenuItem";
                displayOverlayToolStripMenuItem.Checked = false;

                for (int room = 0; room < byte.MaxValue; room++)
                {
                    if (_level.RoomExists((byte)room))
                    {
                        MapRoomNumberInput.Value = room;
                        LoadRoom(false);
                        string baseFileName = $"boss{_level.Map.Number}_room{currentRoomNumber}";

                        for (int layer = 0; layer < 8; layer++)
                        {
                            if (layersCheckList.GetItemColor(layer) == Color.Black)
                            {
                                layersCheckList.SetItemChecked(layer, true);
                                layersCheckList.SetItemChecked(layer + 8, true);

                                UpdateView();
                                string path = Path.Combine(folderDialog.SelectedPath, $"{baseFileName}_layer{layer}.png");
                                if (layer == 0)
                                {
                                    using (Bitmap baselayer = roomLayerBitmap.Clone(new Rectangle(0, 0, 512, 384), roomLayerBitmap.PixelFormat))
                                    {
                                        baselayer.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                }
                                else
                                {
                                    roomLayerBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                                }

                                layersCheckList.SetItemChecked(layer, false);
                                layersCheckList.SetItemChecked(layer + 8, false);
                            }
                        }
                    }
                }

                MapRoomNumberInput.Value = currentRoomNumber = lastRoom;
                autoSelectToolStripMenuItem.Checked = autoLoadActors;
                displayOverlayToolStripMenuItem.Checked = overlay;
                LoadRoom(false);
            }
        }

        private void EnsureAllLayersAreEncoded(string path)
        {
            string layersPath = Path.Combine(path, "szs", $"m{_level.Map.Number}");
            foreach (var filePath in Directory.EnumerateFiles(layersPath))
            {
                if (!Yaz0.IsYaz0(filePath))
                {
                    var bytes = File.ReadAllBytes(filePath);
                    var encodedBytes = Yaz0.Encode(bytes);
                    File.WriteAllBytes(filePath, encodedBytes);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Editor for Four Swords Adventures by JaytheHam & Venomalia. v{VERSION}"
                + "\nwww.jaytheham.com"
                + "\nSend comments, bug reports etc to: jaytheham@gmail.com", $"EFSAdvent version {VERSION}");
        }

        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open the Wiki page in the default web browser
            System.Diagnostics.Process.Start(WikiUrl);
        }

        private void sourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open the Source code page in the default web browser
            System.Diagnostics.Process.Start(SourceCodeUrl);
        }

        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveChangesDialog();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = ShowSaveChangesDialog(true, "Save all changes before exiting?");
        }

        private bool ShowSaveChangesDialog(bool saveMap = true, string message = "Save all changes?")
        {
            if (saveMap ? _level?.IsDirty ?? false : (_level?.LayersAreDirty ?? false) || (_level?.ActorsAreDirty ?? false))
            {
                var dirtyDataBuilder = new StringBuilder();

                if (_level.ActorsAreDirty) dirtyDataBuilder.Append("Actor data");
                if (_level.LayersAreDirty)
                {
                    if (dirtyDataBuilder.Length > 0)
                        dirtyDataBuilder.Append(" and ");
                    dirtyDataBuilder.Append("Layer data");
                }
                if (_level.MapIsDirty && saveMap)
                {
                    if (dirtyDataBuilder.Length > 0)
                        dirtyDataBuilder.Append(" and ");
                    dirtyDataBuilder.Append("Map data");
                }

                string dirtyData = dirtyDataBuilder.ToString();

                var result = MessageBox.Show($"{dirtyData} has been changed, save changes first?", message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        _level.SaveActors();
                        _level.SaveLayers();
                        if (saveMap)
                        {
                            _level.Map.Save();
                        }
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return true;
                }
            }
            return false;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_history.TryGetUndoAction(out HistoryAction action))
            {
                foreach (var tile in action.Tiles)
                {
                    _level.Room.SetLayerTile(action.Layer, tile.x, tile.y, tile.oldValue);
                    UpdateLayerCheckListColor(action.Layer);
                    buttonSaveLayers.Enabled = true;
                }
                UpdateView();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_history.TryGetRedoAction(out HistoryAction action))
            {
                foreach (var tile in action.Tiles)
                {
                    _level.Room.SetLayerTile(action.Layer, tile.x, tile.y, tile.oldValue);
                    UpdateLayerCheckListColor(action.Layer);
                    buttonSaveLayers.Enabled = true;
                }
                UpdateView();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_level.IsDirty)
            {
                switch (MessageBox.Show("Save all data before saving as new level?",
                                                    "Save changes?",
                                                    MessageBoxButtons.YesNoCancel,
                                                    MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        _level.SaveActors();
                        BuildLayerActorList(true);
                        _level.SaveLayers();
                        _level.Map.Save();
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return;
                }
            }

            var saveDialog = new SaveFileDialog
            {
                AddExtension = false,
                FileName = $"boss{_level.Map.Number}",
                Title = "Save as different level number e.g. bossXXX"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string savedFile = Path.GetFileNameWithoutExtension(saveDialog.FileName);

                var regex = new Regex("boss[0-9]{3}$");
                if (!regex.IsMatch(savedFile))
                {
                    MessageBox.Show("\"Save as\" only supports level names like boss000");
                    return;
                }

                string savedFileNumber = new string(savedFile.Skip(4).Take(3).ToArray());

                CopyDirectory(RootFolderPathTextBox.Text, RootFolderPathTextBox.Text, _level.Map.Number, savedFileNumber);
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir, string oldNumber, string newNumber)
        {
            if (oldNumber == newNumber)
            {
                return;
            }

            var pathParts = destinationDir.Split(Path.DirectorySeparatorChar);
            pathParts[pathParts.Length - 1] = ReplaceOldLevelNumberWithNew(pathParts[pathParts.Length - 1], oldNumber, newNumber);
            pathParts[pathParts.Length - 2] = ReplaceOldLevelNumberWithNew(pathParts[pathParts.Length - 2], oldNumber, newNumber);
            pathParts[0] = pathParts[0] + Path.DirectorySeparatorChar;
            destinationDir = Path.Combine(pathParts);

            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string fileName = ReplaceOldLevelNumberWithNew(file.Name, oldNumber, newNumber);
                string targetFilePath = Path.Combine(destinationDir, fileName);
                file.CopyTo(targetFilePath, true);

                if (targetFilePath.EndsWith(".csv"))
                {
                    string contents = File.ReadAllText(targetFilePath);
                    contents = contents.Replace($"map{oldNumber},", $"map{newNumber},");
                    File.WriteAllText(targetFilePath, contents);
                }
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, oldNumber, newNumber);
            }
        }

        private static string ReplaceOldLevelNumberWithNew(string target, string oldNumber, string newNumber)
        {
            return target
                .Replace("boss" + oldNumber, "boss" + newNumber)
                .Replace("b" + oldNumber, "b" + newNumber)
                .Replace("B" + oldNumber, "B" + newNumber)
                .Replace("map" + oldNumber, "map" + newNumber)
                .Replace("m" + oldNumber, "m" + newNumber)
                .Replace("M" + oldNumber, "M" + newNumber);
        }

        #endregion Menu/Form

        #region Actors

        private void CopyActorToClipboard(object sender, EventArgs e)
        {
            string actor = _level.Room.GetActor(actorsCheckListBox.SelectedIndex).CopyToString();
            System.Windows.Forms.Clipboard.SetText(actor);
        }

        private void ActorChangedV5(object sender, EventArgs e)
        {
            if (_ignoreActorChanges)
            {
                return;
            }
            _ignoreActorChanges = true;
            ActorVariable1Input.Value = (int)ActorVariable1AInput.Value << 3 | (int)ActorVariable1BInput.Value;
            ActorVariable2Input.Value = ActorVariable2Input2.Value;
            ActorVariable3Input.Value = ActorVariable3Input2.Value;
            ActorVariable4Input.Value = (int)ActorVariable4Input2.Value | (ActorVariable4FlagCheckBox.Checked ? 0x80 : 0);
            _ignoreActorChanges = false;
            ActorChanged(sender, e);
        }

        private void ActorChangedV6(object sender, EventArgs e)
        {
            if (_ignoreActorChanges)
            {
                return;
            }
            _ignoreActorChanges = true;
            ActorVariable1Input.Value = (int)ActorV6Variable1Input.Value << 3 | (int)ActorV6Variable2Input.Value >> 2;
            ActorVariable2Input.Value = ((int)ActorV6Variable2Input.Value & 0x3) << 6 | (int)ActorV6Variable3Input.Value << 1 | (int)ActorV6Variable4Input.Value >> 4;
            ActorVariable3Input.Value = ((int)ActorV6Variable4Input.Value & 0xF) << 4 | (int)ActorV6Variable5Input.Value >> 1;
            ActorVariable4Input.Value = ((int)ActorV6Variable5Input.Value & 0x1) << 7 | (int)ActorV6Variable6Input.Value;
            _ignoreActorChanges = false;
            ActorChanged(sender, e);
        }

        private void ActorChangedV8(object sender, EventArgs e)
        {
            if (_ignoreActorChanges)
            {
                return;
            }
            _ignoreActorChanges = true;
            ActorVariable1Input.Value = (int)ActorV8Variable1aInput.Value << 3 | (int)ActorV8Variable1bInput.Value;
            ActorVariable2Input.Value = (int)ActorV8Variable2aInput.Value << 4 | (int)ActorV8Variable2bInput.Value;
            ActorVariable3Input.Value = (int)ActorV8Variable3aInput.Value << 4 | (int)ActorV8Variable3bInput.Value;
            ActorVariable4Input.Value = (int)ActorV8Variable4aInput.Value << 4 | (int)ActorV8Variable4bInput.Value;
            _ignoreActorChanges = false;
            ActorChanged(sender, e);
        }

        private bool _ignoreActorChanges = false;
        private void ActorChanged(object sender, EventArgs e)
        {
            if (_ignoreActorChanges)
            {
                return;
            }

            UpdateActorPackedVariables();
            bool RefreshActorList = _level.Room.UpdateActor(actorsCheckListBox.SelectedIndex,
                ACTOR_NAMES[ActorNameComboBox.SelectedIndex],
                (byte)ActorLayerInput.Value,
                (byte)ActorXCoordInput.Value,
                (byte)ActorYCoordInput.Value,
                (byte)ActorVariable1Input.Value,
                (byte)ActorVariable2Input.Value,
                (byte)ActorVariable3Input.Value,
                (byte)ActorVariable4Input.Value);

            if (RefreshActorList)
            {
                BuildLayerActorList(true);
            }

            if (ACTOR_NAMES[ActorNameComboBox.SelectedIndex] == "PNPC" || ACTOR_NAMES[ActorNameComboBox.SelectedIndex] == "PNP2")
            {
                UpdateBrushTileBitmap((ushort)(((byte)ActorVariable3Input.Value & 0x3) << 8 | (byte)ActorVariable4Input.Value));
                ActorInfoPictureBox.Refresh();
            }

            UpdateView();
        }

        private void ReloadActors()
        {
            _level.ReloadActors();
            BuildLayerActorList(false);
        }

        private void BuildLayerActorList(bool keepState = true)
        {
            object[] checkedActorIds = actorsCheckListBox.CheckedItems.Cast<object>().ToArray();
            object selectedActorId = actorsCheckListBox.SelectedItem;

            actorsCheckListBox.Items.Clear();

            if (_level.Room == null)
            {
                return;
            }

            actorsCheckListBox.Items.AddRange(_level.Room.GetActors().ToArray());

            if (keepState)
            {
                for (int i = 0; i < actorsCheckListBox.Items.Count; i++)
                {
                    object actor = actorsCheckListBox.Items[i];
                    if (checkedActorIds.Contains(actor))
                        actorsCheckListBox.SetItemChecked(i, true);

                    if (actor == selectedActorId)
                        actorsCheckListBox.SetSelected(i, true);
                }
            }
        }

        private void actorsReloadButton_Click(object sender, EventArgs e)
        {
            ReloadActors();
        }

        private void actorsCheckListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewActorSelected();
        }

        private void actorsCheckListBox_Click(object sender, EventArgs e)
        {
            SelectedActor(actorsCheckListBox.SelectedIndex);
        }

        private void UpdateActorPackedVariables()
        {
            _ignoreActorChanges = true;
            // V5
            ActorVariable1AInput.Value = (int)ActorVariable1Input.Value >> 3;
            ActorVariable1BInput.Value = (int)ActorVariable1Input.Value & 0x7;
            ActorVariable2Input2.Value = ActorVariable2Input.Value;
            ActorVariable3Input2.Value = ActorVariable3Input.Value;
            ActorVariable4Input2.Value = (int)ActorVariable4Input.Value & 0x7F;
            ActorVariable4FlagCheckBox.Checked = ActorVariable4Input.Value >= 0x80;
            // V6
            ActorV6Variable1Input.Value = ActorVariable1AInput.Value;
            ActorV6Variable2Input.Value = (int)ActorVariable1BInput.Value << 2 | (int)ActorVariable2Input.Value >> 6;
            ActorV6Variable3Input.Value = ((int)ActorVariable2Input.Value >> 1) & 0x1F;
            ActorV6Variable4Input.Value = ((int)ActorVariable2Input.Value & 0x1) << 4 | (int)ActorVariable3Input.Value >> 4;
            ActorV6Variable5Input.Value = ((int)ActorVariable3Input.Value & 0xF) << 1 | (int)ActorVariable4Input.Value >> 7;
            ActorV6Variable6Input.Value = (int)ActorVariable4Input.Value & 0x7F;
            // V8
            ActorV8Variable1aInput.Value = ActorVariable1AInput.Value;
            ActorV8Variable1bInput.Value = ActorVariable1BInput.Value;
            ActorV8Variable2aInput.Value = (int)ActorVariable2Input.Value >> 4;
            ActorV8Variable2bInput.Value = (int)ActorVariable2Input.Value & 0xf;
            ActorV8Variable3aInput.Value = (int)ActorVariable3Input.Value >> 4;
            ActorV8Variable3bInput.Value = (int)ActorVariable3Input.Value & 0xf;
            ActorV8Variable4aInput.Value = (int)ActorVariable4Input.Value >> 4;
            ActorV8Variable4bInput.Value = (int)ActorVariable4Input.Value & 0xf;
            _ignoreActorChanges = false;
        }

        private void NewActorSelected()
        {
            var newActor = _level.Room.GetActor(actorsCheckListBox.SelectedIndex);
            if (newActor == null)
            {
                return;
            }

            _ignoreActorChanges = true;
            ActorNameComboBox.SelectedIndex = Array.IndexOf(ACTOR_NAMES, newActor.Name);
            ActorLayerInput.Value = newActor.Layer;
            ActorXCoordInput.Value = newActor.XCoord;
            ActorYCoordInput.Value = newActor.YCoord;
            ActorVariable1Input.Value = newActor.Variable1;
            ActorVariable2Input.Value = ActorVariable2Input2.Value = newActor.Variable2;
            ActorVariable3Input.Value = ActorVariable3Input2.Value = newActor.Variable3;
            ActorVariable4Input.Value = newActor.Variable4;
            UpdateActorPackedVariables();
            _ignoreActorChanges = false;


            if (V4ACTORS.Contains(newActor.Name))
            {
                VariablesTabControl.SelectedIndex = 0;
            }
            else if (V6ACTORS.Contains(newActor.Name))
            {
                VariablesTabControl.SelectedIndex = 2;
            }
            else if (V8ACTORS.Contains(newActor.Name))
            {
                VariablesTabControl.SelectedIndex = 3;
            }
            else
            {
                VariablesTabControl.SelectedIndex = 1;
            }

            //Load the actor info txt into the box
            string actorName = actorsCheckListBox.Text;
            string actorInfoFilePath = Path.Combine(dataDirectory, "actorinfo", $"{actorName}.txt");
            try
            {
                string info = File.ReadAllText(actorInfoFilePath);
                ActorInfoTextBox.Text = info;
            }
            catch (FileNotFoundException)
            {
                ActorInfoTextBox.Clear();
                _logger.AppendLine($"File \"{actorInfoFilePath}\" not found.");
            }

            if (newActor.Name == "PNPC" || newActor.Name == "PNP2")
            {
                UpdateBrushTileBitmap((ushort)((newActor.Variable3 & 0x3) << 8 | newActor.Variable4));
                ActorInfoPictureBox.Image = brushTileBitmap;
            }
            else
            {
                ActorInfoPictureBox.Image = GetActorSpriteOrDefault(newActor);
            }
        }

        private Bitmap GetActorSpriteOrDefault(Actor actor)
        {
            string type;
            switch (actor.Name)
            {
                case "SNPC":
                case "JIJI":
                    type = $"{actor.Variable2 & 0x7F}";
                    break;
                case "BRRY":
                    type = $"{actor.Variable2 & 0x7F}_{actor.Variable4 & 0x7F}";
                    break;
                case "DOOR":
                    int doorType = actor.Variable4 & 0x7F;
                    if (doorType == 5 || doorType == 7)
                        goto default;
                    else
                        type = $"{(actor.Variable3 & 0xf) << 1 | actor.Variable4 >> 7}_{actor.Variable3 >> 4}";
                    break;
                case "RBPN":
                    type = $"{(actor.Variable3 & 0xf) << 1 | actor.Variable4 >> 7}";
                    break;
                case "STBL":
                    type = $"{actor.Variable3 & 0x7F}";
                    break;
                default:
                    type = $"{actor.Variable4 & 0x7F}";
                    break;
            }

            string specificActorName = $"{actor.Name}_{type}";
            if (ACTOR_SPRITES.ContainsKey(specificActorName))
            {
                return ACTOR_SPRITES[specificActorName];
            }
            else if (ACTOR_SPRITES.ContainsKey(actor.Name))
            {
                return ACTOR_SPRITES[actor.Name];
            }
            else
            {
                return ACTOR_SPRITES[DEFAULT_SPRITE];
            }
        }

        // We don't want checking actors during DrawActors to call DrawActors...
        private bool _ignoreActorCheckbox = false;

        private void DrawActors()
        {
            _ignoreActorCheckbox = true;
            actorLayerGraphics.Clear(Color.Transparent);

            for (int i = 0; i < _level.Room.GetActorCount(); i++)
            {
                if (actorsCheckListBox.GetItemChecked(i) == true)
                {
                    var actor = _level.Room.GetActor(i);
                    DrawActor(actor);
                }
            }
            roomLayerGraphics.DrawImage(actorLayerBitmap, 0, 0);
            layerPictureBox.Refresh();
            _ignoreActorCheckbox = false;
        }

        private void actorLayerComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (actorLayerComboBox.SelectedIndex <= 8)
                SelectLayerActors(actorLayerComboBox.SelectedIndex == 0, actorLayerComboBox.SelectedIndex - 1);
            else
                SelectActorsByVariables(actorLayerComboBox.SelectedIndex - 8);
            UpdateView();
        }

        private void SelectLayerActors(bool selectAll, int selectLayer)
        {
            _ignoreActorCheckbox = true;

            for (int i = 0; i < _level.Room.GetActorCount(); i++)
            {
                var actor = _level.Room.GetActor(i);
                bool isChecked = selectAll || actor.Layer == selectLayer;
                actorsCheckListBox.SetItemChecked(i, isChecked);
            }
            _ignoreActorCheckbox = false;
        }

        private void SelectActorsByVariables(int variable)
        {
            _ignoreActorCheckbox = true;

            for (int i = 0; i < _level.Room.GetActorCount(); i++)
            {
                var actor = _level.Room.GetActor(i);
                bool isChecked = actor.Variable1 >> 3 == variable;

                if (!isChecked && V6ACTORS.Contains(actor.Name))
                {
                    if (((actor.Variable1 & 0x7) << 2 | actor.Variable2 >> 6) == variable)// V2
                        isChecked = true;
                    else if (((actor.Variable2 >> 1) & 0x1F) == variable)// V3
                        isChecked = true;
                    else if (((actor.Variable2 & 0x1) << 4 | actor.Variable3 >> 4) == variable)// V4
                        isChecked = true;
                    else if (((actor.Variable3 & 0xF) << 1 | actor.Variable4 >> 7) == variable)// V5
                        isChecked = true;
                }

                actorsCheckListBox.SetItemChecked(i, isChecked);
            }
            _ignoreActorCheckbox = false;
        }

        private void tabControl_TabIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 2)
            {
                layerPictureBox.ContextMenuStrip = actorContextMenuStrip;
                actorContextMenuStrip.Enabled = true;
            }
            else
            {
                layerPictureBox.ContextMenuStrip = null;
                actorContextMenuStrip.Enabled = false;
            }
        }

        private void DrawActor(Actor actor)
        {
            int width, height;
            int? currentLayer = GetHighestActiveLayerIndex() % 8;
            bool isOnCurrentLayer = currentLayer == actor.Layer;
            bool isSelectedActor = IsSelectedActor(actor);
            Point actorPixelPosition = new Point(actor.XCoord * ACTOR_PIXELS_PER_COORDINATE, actor.YCoord * ACTOR_PIXELS_PER_COORDINATE);

            // Are always checked
            switch (actor.Name)
            {
                case "WARP":
                case "MJGR":
                case "CIRC":
                    int layer = actor.Variable1 & 0x7;
                    if (actor.Name == "CIRC" && layer != 0)
                        layer = 1;
                    bool isZero = actor.Variable2 == 0 && actor.Variable3 == 0;
                    int x = isZero ? actorPixelPosition.X : actor.Variable2 * ACTOR_PIXELS_PER_COORDINATE;
                    int y = isZero ? actorPixelPosition.Y : actor.Variable3 * ACTOR_PIXELS_PER_COORDINATE;
                    if (currentLayer == layer)
                    {
                        Color color = isSelectedActor ? Color.Red : Color.White;
                        actorLayerGraphics.DrawRectangleWithDropShadow(color,
                            x,
                            y,
                            ACTOR_PIXELS_PER_COORDINATE * 2,
                            ACTOR_PIXELS_PER_COORDINATE * 2);
                    }
                    break;
                default:
                    break;
            }

            // Only if on current layer
            if (alwaysShowActorsToolStripMenuItem.Checked || isOnCurrentLayer)
            {
                switch (actor.Name)
                {
                    case "PNPC":
                        if (isOnCurrentLayer)
                            DrawTile(actorLayerBitmap, tileSheetBitmap, actor.XCoord / 2, actor.YCoord / 2, (ushort)((actor.Variable3 & 0x3) << 8 | actor.Variable4));
                        break;
                    case "PTMI":
                        actorLayerGraphics.DrawRectangleWithDropShadow(Color.LightCyan,
                            actorPixelPosition.X,
                            actorPixelPosition.Y,
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.Variable4),
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.Variable3));
                        break;
                    case "TOMI":
                        actorLayerGraphics.DrawRectangleWithDropShadow(Color.LightPink,
                            actorPixelPosition.X,
                            actorPixelPosition.Y,
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.Variable4),
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.Variable3));
                        break;
                    case "BLDF":
                        width = ACTOR_PIXELS_PER_COORDINATE * (1 + actor.Variable3);
                        height = ACTOR_PIXELS_PER_COORDINATE * (1 + actor.Variable2);
                        actorLayerGraphics.DrawRectangleWithDropShadow(Color.Black,
                            actorPixelPosition.X - (width / 2),
                            actorPixelPosition.Y - (height / 2),
                            width,
                            height);

                        if ((actor.Variable1 & 7) == 4)
                        {
                            Color color = isSelectedActor ? Color.LightCoral : Color.White;
                            actorLayerGraphics.DrawRectangleWithDropShadow(color, actorPixelPosition.X, actorPixelPosition.Y, 7, 7);
                            return;
                        }
                        break;

                    case "SWTH":
                        if (isOnCurrentLayer && actor.Variable4 == 7)
                        {
                            width = ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + (actor.Variable2 >> 4));
                            height = ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + (actor.Variable2 & 0xF));
                            actorLayerGraphics.DrawRectangleWithDropShadow(Color.White,
                                actorPixelPosition.X - (width / 2),
                                actorPixelPosition.Y - (height / 2),
                                width,
                                height);
                        }
                        break;
                    case "LOTS":
                        width = ((actor.Variable3 >> 4) + (actor.Variable2 & 0xF) * 16) * ACTOR_PIXELS_PER_COORDINATE * 2;
                        if (width != 0)
                        {
                            GraphicsEX.Direction direction = (GraphicsEX.Direction)(actor.Variable3 & 0xF);
                            Point targetPosition = actorPixelPosition.MovePointInDirection(direction, width);
                            actorLayerGraphics.DrawLineWithDropShadow(Color.Aquamarine, actorPixelPosition, targetPosition);
                        }
                        break;
                    case "RUSA":
                        switch (actor.Variable4 & 0xF)
                        {
                            case 0:
                                width = 2 * ACTOR_PIXELS_PER_COORDINATE;
                                break;
                            case 1:
                                width = 4 * ACTOR_PIXELS_PER_COORDINATE;
                                break;
                            default:
                                width = 8 * ACTOR_PIXELS_PER_COORDINATE;
                                break;
                        }
                        actorLayerGraphics.DrawCircleWithDropShadow(Color.Honeydew, actorPixelPosition, width);
                        break;
                    case "BMST":
                        if (actor.Variable4 != 0)
                        {
                            Color color = isSelectedActor ? Color.LightPink : Color.DarkRed;
                            width = Math.Min(actor.Variable4, (byte)15) * ACTOR_PIXELS_PER_COORDINATE * 2 - ACTOR_PIXELS_PER_COORDINATE;
                            actorLayerGraphics.DrawCircleWithDropShadow(color, actorPixelPosition, width);
                        }
                        break;
                    default:
                        break;
                }

                Bitmap actorSprite = GetActorSpriteOrDefault(actor);

                actorLayerGraphics.DrawImage(actorSprite,
                    actorPixelPosition.X - (actorSprite.Width / 2),
                    actorPixelPosition.Y - (actorSprite.Height / 2),
                    actorSprite.Width,
                    actorSprite.Height);
            }

            // Always highlights selected Actor
            if (isSelectedActor)
            {
                actorLayerGraphics.DrawRectangleWithDropShadow(Color.LightCoral, actorPixelPosition.X, actorPixelPosition.Y, 7, 7);
            }
        }

        private void actorsSaveButton_Click(object sender, EventArgs e)
        {
            _level.SaveActors();
            BuildLayerActorList(true);
            MessageBox.Show("Changes Saved");
        }

        private void bilinearToolStripMenuItem_Click(object sender, EventArgs e) => SetInterpolationMode(InterpolationMode.Bilinear);

        private void bicubicToolStripMenuItem_Click(object sender, EventArgs e) => SetInterpolationMode(InterpolationMode.Bicubic);

        private void nearestNeighborToolStripMenuItem_Click(object sender, EventArgs e) => SetInterpolationMode(InterpolationMode.NearestNeighbor);

        private void SetInterpolationMode(InterpolationMode interpolationMode)
        {
            bilinearToolStripMenuItem.Checked = bicubicToolStripMenuItem.Checked = nearestNeighborToolStripMenuItem.Checked = false;
            switch (interpolationMode)
            {
                case InterpolationMode.Bilinear:
                    bilinearToolStripMenuItem.Checked = true;
                    break;
                case InterpolationMode.Bicubic:
                    bicubicToolStripMenuItem.Checked = true;
                    break;
                case InterpolationMode.NearestNeighbor:
                    nearestNeighborToolStripMenuItem.Checked = true;
                    break;
            }
            layerPictureBox.InterpolationMode = interpolationMode;
            layerPictureBox.Refresh();
        }

        private void roomImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog { Filter = "Room actors files|d_enemy_map*.bin" };
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string name = Path.GetFileName(openDialog.FileName);
            string mapNr = name.Substring(11, 3);
            int RoomNR = int.Parse(name.Substring(15, 2));
            string basePath = openDialog.FileName.Remove(openDialog.FileName.LastIndexOf("bin" + Path.DirectorySeparatorChar));
            int freeRoomNr = _level.GetNextFreeRoom();

            string actors = Room.GetActorFilePath(basePath, mapNr, RoomNR);
            string newActors = Room.GetActorFilePath(RootFolderPathTextBox.Text, _level.Map.Number, freeRoomNr);
            File.Copy(actors, newActors);

            for (int la = 0; la < 8; la++)
            {
                for (int lv = 1; lv <= 2; lv++)
                {
                    string layer = Room.GetLayerFilePath(basePath, mapNr, RoomNR, lv, la);
                    string newLayer = Room.GetLayerFilePath(RootFolderPathTextBox.Text, _level.Map.Number, freeRoomNr, lv, la);
                    File.Copy(layer, newLayer);
                }
            }

            MessageBox.Show($"Room successfully imported to room {freeRoomNr}.");
            if (MapRoomNewButton.Enabled == true)
            {
                var result = MessageBox.Show($"Should the imported room be set to the selected position [{selectedRoomCoordinates.x},{selectedRoomCoordinates.y}]?",
                                             $"Set to position?",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MapRoomNumberInput.Value = freeRoomNr;
                    UpdateMapRoomNumber((byte)freeRoomNr);
                }
            }
        }

        private void actorsImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog { Filter = "Room actors files|d_enemy_map*.bin" };
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            List<Actor> actors = Room.ReadActors(openDialog.FileName);
            _level.Room.AddActors(actors);
            BuildLayerActorList(true);
            DrawActors();

            MessageBox.Show($"{actors.Count} actors have been successfully added to the current room.");
        }

        private void SelectedActor(Actor actor)
        {
            int index = _level.Room.IndexOf(actor);
            SelectedActor(index);
        }

        private void SelectedActor(int index)
        {
            if (index < 0)
            {
                actorsCheckListBox.SelectedIndex = -1;
                actorAttributesgroupBox.Enabled = false;
            }
            else
            {
                actorsCheckListBox.SetItemChecked(index, true);
                actorsCheckListBox.SelectedIndex = index;
                actorAttributesgroupBox.Enabled = true;
            }
            UpdateView();
        }

        private bool IsSelectedActor(Actor actor)
        {
            int index = _level.Room.IndexOf(actor);
            return actorsCheckListBox.SelectedIndex == index;
        }

        private void actorContextMenuStrip_Paint(object sender, PaintEventArgs e)
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                string actor = System.Windows.Forms.Clipboard.GetText();
                pastToolStripMenuItem.Enabled = Actor.IsStringActor(actor);
            }
            else
            {
                pastToolStripMenuItem.Enabled = false;
            }
        }

        private void pastToolStripMenuItem_Click(object sender, EventArgs e)
            => AddNewActorFromCode(System.Windows.Forms.Clipboard.GetText());

        private void AddActorStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = (sender as ToolStripMenuItem)?.Tag as string;
            AddNewActorFromCode(code);
        }

        private void AddNewActorFromCode(string code)
        {
            Actor actor = new Actor();
            if (actor.PasteFromString(code))
            {
                int? activeLayer = GetHighestActiveLayerIndex();
                int baseLayer = activeLayer.HasValue
                    ? (activeLayer > 7 ? activeLayer - 8 : activeLayer).Value
                    : 0;

                actor.Update((byte)baseLayer, (byte)lastActorCoordinates.x, (byte)lastActorCoordinates.y);
                _level.Room.AddActor(actor);
                BuildLayerActorList(true);
                DrawActors();
                SelectedActor(actor);
            }
        }

        private void actorDeleteButton_Click(object sender, EventArgs e)
        {
            if (_level.Room.RemoveActorAt(actorsCheckListBox.SelectedIndex))
            {
                BuildLayerActorList(true);
                if (actorsCheckListBox.Items.Count > 0)
                {
                    actorsCheckListBox.SetSelected(0, true);
                }
                UpdateView();
            }
        }

        private void actorsCheckListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_ignoreActorCheckbox)
            {
                return;
            }
            // Need to delay redraw because right now the newly checked actor won't have checked=true
            this.BeginInvoke((MethodInvoker)(() => UpdateView()));
        }

        private void buttonActorsSelectNone_Click(object sender, EventArgs e)
        {
            actorLayerComboBox.SelectedIndex = -1;
            _ignoreActorCheckbox = true;
            for (int i = 0; i < actorsCheckListBox.Items.Count; i++)
            {
                actorsCheckListBox.SetItemChecked(i, false);
            }
            _ignoreActorCheckbox = false;
            UpdateView();
        }

        private void actorsCheckListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _level.Room.RemoveActorAt(actorsCheckListBox.SelectedIndex))
            {
                BuildLayerActorList(true);
                if (actorsCheckListBox.Items.Count > 0)
                {
                    actorsCheckListBox.SetSelected(0, true);
                }
                UpdateView();
            }
        }

        #endregion Actors
    }
}