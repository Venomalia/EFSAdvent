using EFSAdvent.FourSwords;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace EFSAdvent
{
    public partial class Form1 : Form
    {
        const string VERSION = "1.2";
        private string BaseTitel = $"EFSAdvent {VERSION}";

        const int ACTOR_PIXELS_PER_COORDINATE = 8;
        const string DEFAULT_SPRITE = "ActorDefault";
        const int LAYER_DIMENSION_IN_PIXELS = Layer.DIMENSION * TILE_DIMENSION_IN_PIXELS;
        const int MAP_ROOM_DIMENSION_IN_PIXELS = 20;
        const int TILE_DIMENSION_IN_PIXELS = 16;

        Bitmap mapBitmap, tileSheetBitmap, roomLayerBitmap, brushTileBitmap, actorLayerBitmap, currentActorBitmap;
        Graphics mapGraphics, roomLayerGraphics, actorLayerGraphics;

        ushort brushTileValue;

        private Level _level;
        private History _history;
        private Clipboard _clipboard;
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

        public Form1()
        {
            InitializeComponent();
            this.Text = BaseTitel;

            tileSheetBitmap = new Bitmap("data\\Tile Sheet 00.PNG");
            tileSheetPictureBox.Image = tileSheetBitmap;

            brushTileBitmap = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BrushTilePictureBox.Image = brushTileBitmap;

            roomLayerBitmap = new Bitmap(LAYER_DIMENSION_IN_PIXELS, LAYER_DIMENSION_IN_PIXELS, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            roomLayerGraphics = Graphics.FromImage(roomLayerBitmap);
            layerPictureBox.Image = roomLayerBitmap;

            mapBitmap = new Bitmap(200, 200, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            mapGraphics = Graphics.FromImage(mapBitmap);
            mapPictureBox.Image = mapBitmap;

            currentTileSheetComboBox.SelectedIndex = 0;
            BrushSizeComboBox.SelectedIndex = 0;

            actorLayerBitmap = new Bitmap(LAYER_DIMENSION_IN_PIXELS, LAYER_DIMENSION_IN_PIXELS, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            actorLayerGraphics = Graphics.FromImage(actorLayerBitmap);

            currentActorBitmap = new Bitmap(64, 64);
            ActorInfoPictureBox.Image = currentActorBitmap;

            _history = new History(500);
            _clipboard = new Clipboard(_history);
            _logger = new Logger(loggerTextBox);

            string exePath = Application.ExecutablePath;
            string directory = Path.GetDirectoryName(exePath);
            string spriteFolder = Path.Combine(directory, "data", "actorsprites");
            var spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var spritePath in spritePaths)
            {
                var sprite = new Bitmap(spritePath);
                ACTOR_SPRITES.Add(spritePath.Split(Path.DirectorySeparatorChar).Last().Split('.')[0], sprite);
            }

            var names = File.ReadLines("data\\FSA Actor Namelist.txt");
            ACTOR_NAMES = names.Select(n => n.Trim()).ToArray();
            ActorNameComboBox.Items.AddRange(ACTOR_NAMES);

            ResetVarsForNewLevel();
        }

        private void OpenLevel(object sender, EventArgs e)
        {
            if (_level?.IsDirty ?? false)
            {
                bool cancelled = ShowSaveChangesDialog();
                if (cancelled)
                {
                    return;
                }
            }
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
            _ignoreMapVariableUpdates = true;

            ResetVarsForNewLevel();
            _level = new Level(openDialog.FileName, _logger);
            _level.LoadMap();

            this.Text = $"{BaseTitel} - {_level.Map.Name}";
            SetMapVariableInput(MapVariableStartX, _level.Map.StartX);
            SetMapVariableInput(MapVariableStartY, _level.Map.StartY);
            SetMapVariableInput(MapVariableMusic, _level.Map.BackgroundMusicId);
            SetMapVariableInput(MapVariableE3Banner, _level.Map.ShowE3Banner);
            SetMapVariableInput(MapVariableOverlay, _level.Map.OverlayTextureId);
            SetMapVariableInput(MapVariableTileSheet, _level.Map.TileSheetId);
            SetMapVariableInput(MapVariableNPCSheetID, _level.Map.NPCSheetID);
            SetMapVariableInput(MapVariableUnknown2, _level.Map.Unknown2);
            SetMapVariableInput(MapVariableDisallowTingle, _level.Map.DisallowTingle);

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

            MapVariablesGroupBox.Text = _level.Map.Name;

            //Get a string which is just the root bossxxx filepath for loading other files
            RootFolderPathTextBox.Text = openDialog.FileName.Remove(openDialog.FileName.LastIndexOf("\\map\\") + 1);

            ChangeTileSheet(_level.TileSheetId);
            DrawMap();
            layerPictureBox.Refresh();

			MapRoomLoadButton.Enabled = false;
			MapRoomUpdateButton.Enabled = !_level.Map.IsShadowBattle && true;
			MapSaveButton.Enabled = !_level.Map.IsShadowBattle && true;
			ExportMenuItem.Enabled = true;
			SaveMenuItem.Enabled = true;
			SaveAsMenuItem.Enabled = true;
            tabControl.Enabled = true;
            layerPictureBox.Enabled = true;
            rightSideGroupBox.Enabled = true;

            MapVariableStartX.Enabled = !_level.Map.IsShadowBattle;
            MapVariableStartY.Enabled = !_level.Map.IsShadowBattle;
            MapVariableMusic.Enabled = !_level.Map.IsShadowBattle;
            MapVariableE3Banner.Enabled = !_level.Map.IsShadowBattle;
            MapVariableOverlay.Enabled = !_level.Map.IsShadowBattle;
            MapVariableTileSheet.Enabled = !_level.Map.IsShadowBattle;
            MapVariableNPCSheetID.Enabled = !_level.Map.IsShadowBattle;
            MapVariableUnknown2.Enabled = !_level.Map.IsShadowBattle;
            MapVariableDisallowTingle.Enabled = !_level.Map.IsShadowBattle;
            MapRoomNumberInput.Enabled = !_level.Map.IsShadowBattle;

            MapRoomNumberInput.Value = _level.Map.GetRoomValue(_level.Map.StartX, _level.Map.StartY);
			LoadRoom(sender, e);

            _ignoreMapVariableUpdates = false;
		}

        private void ResetVarsForNewLevel()
        {
            roomLayerGraphics.Clear(Color.Transparent);
            _history.Reset();
            actorMouseDownOnIndex = null;
            currentRoomNumber = Map.EMPTY_ROOM_VALUE;
            selectedRoomCoordinates = (0, 0);
        }

        private void LoadRoom(object sender, EventArgs e)
        {
            if (_level.ActorsAreDirty || _level.LayersAreDirty)
            {
                string dirtyData = (_level.ActorsAreDirty ? "Actor data" : string.Empty)
                    + (_level.ActorsAreDirty && _level.LayersAreDirty ? " and " : string.Empty)
                    + (_level.LayersAreDirty ? "Layer data" : string.Empty);

                var result = MessageBox.Show($"{dirtyData} has been changed, save changes first?",
                                             "Save changes?",
                                             MessageBoxButtons.YesNoCancel,
                                             MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        _level.SaveActors();
                        _level.SaveLayers();
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return;
                }
            }

            byte newRoomNumber = (byte)MapRoomNumberInput.Value;
            if (_level.LoadRoom(newRoomNumber))
            {
                currentRoomNumber = newRoomNumber;
                _history.Reset();
                LoadActors();
                for (int i = 1; i < 16; i++)
                {
                    layersCheckList.SetItemChecked(i, false);
                }
                layersCheckList.SetItemChecked(0, true);
                layersCheckList.SetItemChecked(8, true);

                UpdateView(false);
            }
        }

        private void SaveActionToHistory(int layer, int x, int y, int brushWidth)
        {
            var oldValues = new List<ushort>();
            var newValues = new List<ushort>();
            var coordinates = new List<(int x, int y)>();
            bool tileChanged = false;
            for (int testY = y; testY < y + brushWidth; testY++)
            {
                for (int testX = x; testX < x + brushWidth; testX++)
                {
                    var currentTile = _level.Room.GetLayerTile(layer, testX, testY);
                    if (currentTile.HasValue)
                    {
                        tileChanged |= currentTile != this.brushTileValue;
                        coordinates.Add((testX, testY));
                        oldValues.Add(currentTile.Value);
                        newValues.Add(this.brushTileValue);
                    }
                }
            }

            if (!tileChanged)
            {
                return;
            }

            _history.StoreTileChange(coordinates.ToArray(), oldValues.ToArray(), newValues.ToArray(), layer);
        }

        private void UpdateView(bool actorLayerChanged, int? layer = null)
        {
            roomLayerGraphics.Clear(Color.Transparent);
            for (int i = 0; i < 8; i++)
            {
                for (int n = 0; n <= 8; n += 8)
                {
                    if ((layer == null && layersCheckList.GetItemChecked(i + n)) || (i + n) == layer)
                    {
                        DrawLayer(i + n);
                    }
                }
            }
            DrawActors(actorLayerChanged);
        }

        #region Map

        private unsafe void DrawMap()
        {
            byte roomColour;
            var bitmapLock = mapBitmap.LockBits(
                new Rectangle(0, 0, 200, 200),
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
            _logger.Clear();

            int roomWidthInPixels = mapPictureBox.Width / _level.Map.XDimension;
            int roomHeightInPixels = mapPictureBox.Height / _level.Map.YDimension;
            //When the user clicks in the map load the value of the clicked room into the edit box
            selectedRoomCoordinates = (e.X / roomWidthInPixels, e.Y / roomHeightInPixels);
            byte roomValue = _level.Map.GetRoomValue(selectedRoomCoordinates.x, selectedRoomCoordinates.y);
            MapRoomNumberInput.Value = roomValue;

            _logger.AppendText($"Map coordinates: x{selectedRoomCoordinates.x} y{selectedRoomCoordinates.y}");

            DrawMapWithSelectedRoomHighlighted();
        }

        private void SelectRoomNumber(object sender, EventArgs e)
        {
            MapRoomLoadButton.Enabled = _level.RoomExists((byte)MapRoomNumberInput.Value);
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

        private void UpdateMapRoomNumber(object sender, EventArgs e)
        {
            byte newValue = (byte)MapRoomNumberInput.Value;
            _level.Map.SetRoomValue(selectedRoomCoordinates.x, selectedRoomCoordinates.y, newValue);
            DrawMap();
            DrawMapWithSelectedRoomHighlighted();
        }

        private void SaveMap(object sender, EventArgs e)
        {
            _level.SaveMap();
        }

		private void UpdateMapVariables(object sender, EventArgs e)
		{
			if (_ignoreMapVariableUpdates)
			{
				return;
			}
			_level.Map.SetVariables(
				(int)MapVariableStartX.Value,
				(int)MapVariableStartY.Value,
				(int)MapVariableMusic.Value,
				(int)MapVariableE3Banner.Value,
				(int)MapVariableTileSheet.Value,
                (int)MapVariableNPCSheetID.Value,
                (int)MapVariableOverlay.Value,
                (int)MapVariableUnknown2.Value,
                (int)MapVariableDisallowTingle.Value
            );
            DrawMap();
        }

        #endregion Map

        #region Layers

        private MouseEventArgs ScaleEventToLayerRealSize(MouseEventArgs e)
        {
            float widthScale = (float)LAYER_DIMENSION_IN_PIXELS / layerPictureBox.Width;
            float heightScale = (float)LAYER_DIMENSION_IN_PIXELS / layerPictureBox.Height;
            return new MouseEventArgs(e.Button, e.Clicks, (int)(e.X * widthScale), (int)(e.Y * heightScale), e.Delta);
        }

        private void layersPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                return;
            }

            MouseEventArgs scaledEvent = ScaleEventToLayerRealSize(e);
            switch (tabControl.SelectedTab.TabIndex)
            {
                case 1: // Tile tab
                    if (brushRadioButton.Checked)
                    {
                        DoTileAction(scaledEvent);
                    }
                    else if (clipboardRadioButton.Checked)
                    {
                        UpdateClipboardSelection();
                    }
                    break;
                case 2: // Actor tab
                    MoveActor();
                    break;
                default:

                    break;
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

                UpdateView(false);
            }

            void UpdateClipboardSelection()
            {
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

                DrawLayerWithTileSelection();
            }
        }

        private void DrawLayer(int layer)
        {
            ushort tile;
            foreach (int y in Enumerable.Range(0, Layer.DIMENSION))
            {
                foreach (int x in Enumerable.Range(0, Layer.DIMENSION))
                {
                    tile = _level.Room.GetLayerTile(layer, x, y).Value;
                    DrawTile(x, y, tile);
                }
            }
        }

        private unsafe void DrawTile(int x, int y, int tileNo)
        {
            int srcX, srcY, dstX, dstY;
            srcX = (tileNo % TILE_DIMENSION_IN_PIXELS) * TILE_DIMENSION_IN_PIXELS;
            srcY = (tileNo / TILE_DIMENSION_IN_PIXELS) * TILE_DIMENSION_IN_PIXELS;
            dstY = y * TILE_DIMENSION_IN_PIXELS;
            dstX = x * TILE_DIMENSION_IN_PIXELS;

            var tileSource = tileSheetBitmap.LockBits(new Rectangle(srcX, srcY, 16, 16), System.Drawing.Imaging.ImageLockMode.ReadOnly, tileSheetBitmap.PixelFormat);
            var lockedLayersBitmap = roomLayerBitmap.LockBits(new Rectangle(dstX, dstY, 16, 16), System.Drawing.Imaging.ImageLockMode.WriteOnly, roomLayerBitmap.PixelFormat);
            for (int py = 0; py < tileSource.Height; py++)
            {
                byte* srcRow = (byte*)tileSource.Scan0 + (py * tileSource.Stride);
                byte* dstRow = (byte*)lockedLayersBitmap.Scan0 + (py * lockedLayersBitmap.Stride);
                for (int px = 0; px < lockedLayersBitmap.Width; px++)
                {
                    if (srcRow[px * 4] > 0)
                    {
                        dstRow[px * 4] = srcRow[px * 4];
                        dstRow[(px * 4) + 1] = srcRow[(px * 4) + 1];
                        dstRow[(px * 4) + 2] = srcRow[(px * 4) + 2];
                        dstRow[(px * 4) + 3] = srcRow[(px * 4) + 3];
                    }
                }
            }
            tileSheetBitmap.UnlockBits(tileSource);
            roomLayerBitmap.UnlockBits(lockedLayersBitmap);
        }

        private void layersPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventArgs scaledEvent = ScaleEventToLayerRealSize(e);

            switch (tabControl.SelectedTab.TabIndex)
            {
                case 1: // Tile tab
                    if (brushRadioButton.Checked)
                    {
                        DoTileAction(scaledEvent);
                    }
                    else if (clipboardRadioButton.Checked)
                    {
                        SetClipboardOrigin();
                    }
                    break;
                case 2: // Actor tab
                    SetMouseDownActor();
                    break;
                default:

                    break;
            }

            void SetClipboardOrigin()
            {
                if (e.Button != MouseButtons.Left)
                {
                    return;
                }

                _tileSelection.X = _tileSelectionOrigin.x = scaledEvent.X / TILE_DIMENSION_IN_PIXELS;
                _tileSelection.Y = _tileSelectionOrigin.y = scaledEvent.Y / TILE_DIMENSION_IN_PIXELS;
                _tileSelection.Width = 1;
                _tileSelection.Height = 1;
                DrawLayerWithTileSelection();
            }

            void SetMouseDownActor()
            {
                lastActorCoordinates = (scaledEvent.X / ACTOR_PIXELS_PER_COORDINATE, scaledEvent.Y / ACTOR_PIXELS_PER_COORDINATE);
                _logger.Clear();
                _logger.AppendText($"Actor coordinates: x{lastActorCoordinates.x} y{lastActorCoordinates.y}");

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
                        if (actor.XCoord == lastActorCoordinates.x && actor.YCoord == lastActorCoordinates.y)
                        {
                            actorMouseDownOnIndex = i;
                            actorsCheckListBox.SelectedIndex = i;
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

            Thread.Sleep(400);
            UpdateView(false);
            if (clipboardRadioButton.Checked)
            {
                DrawTileSelection();
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
            _logger.Clear();
            _logger.AppendText($"Tile coordinates: x{eventX} y{eventY}");

            //If right click set the brush tile to the clicked tile
            if (scaledEvent.Button == MouseButtons.Right)
            {
                //Write the value into the brush tile label and update the brush tile image
                UpdateBrushTileBitmap(_level.Room.GetLayerTile(layer.Value, eventX, eventY).Value);
            }
            else if (scaledEvent.Button == MouseButtons.Left) //If left button is pressed change the tile
            {
                int brushWidth = BrushSizeComboBox.SelectedIndex + 1;

                SaveActionToHistory(layer.Value, eventX, eventY, brushWidth);
                for (int y = eventY; y < eventY + brushWidth; y++)
                {
                    for (int x = eventX; x < eventX + brushWidth; x++)
                    {
                        ChangeTile(layer.Value, x, y, brushTileValue);
                    }
                }
                UpdateView(false, layer);
            }
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

        private void DrawLayerWithTileSelection()
        {
            int? layer = GetHighestActiveLayerIndex();
            if (layer == null)
            {
                return;
            }

            UpdateView(false, layer.Value);
            DrawTileSelection();
        }

        private void DrawTileSelection()
        {
            var brush = new SolidBrush(Color.FromArgb(50, 255, 255, 255));
            roomLayerGraphics.FillRectangle(
                brush,
                _tileSelection.X * TILE_DIMENSION_IN_PIXELS,
                _tileSelection.Y * TILE_DIMENSION_IN_PIXELS,
                _tileSelection.Width * TILE_DIMENSION_IN_PIXELS,
                _tileSelection.Height * TILE_DIMENSION_IN_PIXELS);
            var pen = new Pen(Color.White, 1f)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };
            roomLayerGraphics.DrawRectangle(
                pen,
                _tileSelection.X * TILE_DIMENSION_IN_PIXELS,
                _tileSelection.Y * TILE_DIMENSION_IN_PIXELS,
                _tileSelection.Width * TILE_DIMENSION_IN_PIXELS,
                _tileSelection.Height * TILE_DIMENSION_IN_PIXELS);
            layersPanel.Refresh();
        }

        private void ChangeTile(int layer, int x, int y, ushort newTileValue)
        {
            if (_level.Room.SetLayerTile(layer, x, y, newTileValue))
            {
                buttonSaveLayers.Enabled = true;
            }
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

            brushTileValue = tileID;
            BrushTileLabel.Text = Convert.ToString(brushTileValue);
            int brushTileX = ((brushTileValue % 16) * 16);
            int brushTileY = ((brushTileValue / 16) * 16);
            for (int px = 0; px < 16; px++)
            {
                for (int py = 0; py < 16; py++)
                {
                    Color color = tileSheetBitmap.GetPixel(brushTileX + px, brushTileY + py);
                    brushTileBitmap.SetPixel(px, py, color);
                }
            }

            BrushTilePictureBox.Refresh();
        }

        private void updateLayersButton_Click(object sender, EventArgs e)
        {
            UpdateView(false);
        }

        private void copyTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int? layer = GetHighestActiveLayerIndex();

            if (layer.HasValue)
            {
                _clipboard.Copy(_tileSelection, _level, layer.Value);
            }

        }

        private void pasteTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int? layer = GetHighestActiveLayerIndex();

            if (layer.HasValue)
            {
                _clipboard.Paste(_tileSelection, _level, layer.Value);
                UpdateView(false);
            }
        }

        private void LayersCheckList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Need to delay redraw because right now the newly checked layer won't have checked=true
            this.BeginInvoke((MethodInvoker)(() => UpdateView(false)));
        }

        private void currentTileSheetComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int newTileSheetId = ((ComboBox)sender).SelectedIndex;
            ChangeTileSheet(newTileSheetId);
            MapVariableTileSheet.Value = newTileSheetId;
            UpdateView(false);
            UpdateMapVariables(null, null);
        }

        private void ChangeTileSheet(int tileSheetIndex)
        {
            currentTileSheetComboBox.SelectedIndex = tileSheetIndex;

            string exePath = Application.ExecutablePath;
            string directory = Path.GetDirectoryName(exePath);
            var tileSheetPath = Path.Combine(directory, "data", $"Tile Sheet {currentTileSheetComboBox.SelectedIndex:D2}.PNG");
            var tileSheet = new Bitmap(tileSheetPath);
            var graphics = Graphics.FromImage(tileSheetBitmap);
            graphics.Clear(Color.FromArgb(00000000));
            graphics.DrawImage(tileSheet, 0, 0);
            tileSheetPictureBox.Image = tileSheetBitmap;
        }

        private void buttonSaveLayers_Click(object sender, EventArgs e)
        {
            _level.Room.SaveLayers();
            MessageBox.Show("Changes Saved");
        }

        private void tileSheetPictureBox_MouseEnter(object sender, EventArgs e)
        {
            tileSheetPictureBox.Focus(); // So mousewheel can be used to scroll
        }

        #endregion Layers

        #region Menu/Form

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO I think this needs more work
            // Where does a new map save to??
            DrawLayer(0);
            layerPictureBox.Refresh();
            //level = new Level();
        }

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
            switch (MessageBox.Show("Save all data before exporting?",
                                            "Save changes?",
                                            MessageBoxButtons.YesNo,
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
            }

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
            MessageBox.Show($"Editor for Four Swords Adventures by JaytheHam. v{VERSION}"
                + "\nwww.jaytheham.com"
                + "\nSend comments, bug reports etc to: jaytheham@gmail.com", $"EFSAdvent version {VERSION}");
        }

        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveChangesDialog("Save all open data?");
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_level?.IsDirty ?? false)
            {
                e.Cancel = ShowSaveChangesDialog();
            }
        }

        private bool ShowSaveChangesDialog(string message = "Save all data before exiting?")
        {
            if (_level != null)
            {
                switch (MessageBox.Show(message,
                                                    "Save changes?",
                                                    MessageBoxButtons.YesNoCancel,
                                                    MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        _level.SaveActors();
                        _level.SaveLayers();
                        _level.Map.Save();
                        break;

                    case DialogResult.Cancel:
                        return true;

                    case DialogResult.No:
                        break;
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
                    ChangeTile(action.Layer, tile.x, tile.y, tile.oldValue);
                }
                UpdateView(false);
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_history.TryGetRedoAction(out HistoryAction action))
            {
                foreach (var tile in action.Tiles)
                {
                    ChangeTile(action.Layer, tile.x, tile.y, tile.oldValue);
                }
                UpdateView(false);
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
            ActorVariable4Input.Value = ActorVariable4Input2.Value;
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

        private bool _ignoreActorChanges = false;
        private void ActorChanged(object sender, EventArgs e)
        {
            if (_ignoreActorChanges)
            {
                return;
            }

            _level.Room.UpdateActor(actorsCheckListBox.SelectedIndex,
                ACTOR_NAMES[ActorNameComboBox.SelectedIndex],
                (byte)ActorLayerInput.Value,
                (byte)ActorXCoordInput.Value,
                (byte)ActorYCoordInput.Value,
                (byte)ActorVariable1Input.Value,
                (byte)ActorVariable2Input.Value,
                (byte)ActorVariable3Input.Value,
                (byte)ActorVariable4Input.Value);

            UpdateActorPackedVariables();

            if (ACTOR_NAMES[ActorNameComboBox.SelectedIndex] == "PNPC" || ACTOR_NAMES[ActorNameComboBox.SelectedIndex] == "PNP2")
            {
                UpdateBrushTileBitmap((ushort)(((byte)ActorVariable3Input.Value & 0x3)  << 8 | (byte)ActorVariable4Input.Value));
                ActorInfoPictureBox.Refresh();
            }

            if (!(sender is NumericUpDown input) || input.Name == "ActorLayerInput")
            {
                BuildLayerActorList(true);
            }

            UpdateView(false);
        }

        private void LoadActors()
        {
            _level.LoadActors();
            BuildLayerActorList(false);

            //Enable all the actor buttons now that data to work with exists
            actorDeleteButton.Enabled = true;
            actorsSaveButton.Enabled = true;
            actorsReloadButton.Enabled = true;
            actorLayerComboBox.Enabled = true;
        }

        private void BuildLayerActorList(bool keepState = true)
        {
            actorsCheckListBox.Items.Clear();

            if (_level.Room == null)
            {
                return;
            }

            Guid[] checkedActorIds = actorsCheckListBox.CheckedItems.Cast<Actor>().Select(actor => actor.Id).ToArray();

            Guid selectedActorId = ((Actor)actorsCheckListBox.SelectedItem)?.Id ?? Guid.Empty;

            actorsCheckListBox.Items.AddRange(_level.Room.GetActors().Cast<object>().ToArray());

            if (!keepState)
            {
                return;
            }

            for (int i = 0; i < actorsCheckListBox.Items.Count; i++)
            {
                if ((actorsCheckListBox.Items[i] is Actor actor))
                {
                    if (checkedActorIds.Contains(actor.Id))
                    {
                        actorsCheckListBox.SetItemChecked(i, true);
                    }

                    if (actor.Id == selectedActorId)
                    {
                        actorsCheckListBox.SetSelected(i, true);
                    }
                }
            }
        }

        private void actorsReloadButton_Click(object sender, EventArgs e)
        {
            LoadActors();
        }

        private void actorsCheckListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewActorSelected();
        }

        private void actorsCheckListBox_Click(object sender, EventArgs e)
        {
            if (actorsCheckListBox.SelectedIndex == -1)
            {
                return;
            }

            if (!actorsCheckListBox.GetItemChecked(actorsCheckListBox.SelectedIndex))
            {
                actorsCheckListBox.SetItemChecked(actorsCheckListBox.SelectedIndex, true);
            }
        }

        private void UpdateActorPackedVariables()
        {
            _ignoreActorChanges = true;
            ActorVariable1AInput.Value = (int)ActorVariable1Input.Value >> 3;
            ActorVariable1BInput.Value = (int)ActorVariable1Input.Value & 0x7;
            ActorV6Variable1Input.Value = ActorVariable1AInput.Value;
            ActorV6Variable2Input.Value = (int)ActorVariable1BInput.Value << 2 | (int)ActorVariable2Input.Value >> 6;
            ActorV6Variable3Input.Value = ((int)ActorVariable2Input.Value >> 1) & 0x1F;
            ActorV6Variable4Input.Value = ((int)ActorVariable2Input.Value & 0x1) << 4 | (int)ActorVariable3Input.Value >> 4;
            ActorV6Variable5Input.Value = ((int)ActorVariable3Input.Value & 0xF) << 1 | (int)ActorVariable4Input.Value >> 7;
            ActorV6Variable6Input.Value = (int)ActorVariable4Input.Value & 0x7F;
            _ignoreActorChanges = false;
        }

        private void NewActorSelected()
        {
            var newActor = _level.Room.GetActor(actorsCheckListBox.SelectedIndex);
            if (newActor == null )
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
            ActorVariable4Input.Value = ActorVariable4Input2.Value = newActor.Variable4;
            UpdateActorPackedVariables();
            _ignoreActorChanges = false;

            if (newActor.Name == "SWT4" || newActor.Name == "HOUS" || newActor.Name == "SHTR" || newActor.Name == "BPOH" || newActor.Name == "BPH2" || newActor.Name == "SPOT" || newActor.Name == "GLBS" || newActor.Name == "LCLS" || newActor.Name == "OFFS" || newActor.Name == "CKSW" || newActor.Name == "WTSW" || newActor.Name == "LOSW")
            {
                VariablesTabControl.SelectedIndex = 2;
            }
            else if (newActor.Name == "TZOK")
            {
                VariablesTabControl.SelectedIndex = 0;
            }
            else
            {
                VariablesTabControl.SelectedIndex = 1;
            }

            //Load the actor info txt into the box
            string exePath = Application.ExecutablePath;
            string directory = Path.GetDirectoryName(exePath);
            string actorName = actorsCheckListBox.Text;
            string actorInfoFilePath = Path.Combine(directory, "data", "actorinfo", $"{actorName}.txt");
            try
            {
                string info = File.ReadAllText(actorInfoFilePath);
                ActorInfoTextBox.Text = info;
            }
            catch (FileNotFoundException)
            {
                ActorInfoTextBox.Clear();
                _logger.AppendLine($"File data/actorinfo/{actorName}.txt not found.");
            }

            if (newActor.Name == "PNPC" || newActor.Name == "PNP2")
            {
                UpdateBrushTileBitmap((ushort)((newActor.Variable3 & 0x3) << 8 | newActor.Variable4));
                ActorInfoPictureBox.Image = brushTileBitmap;
            }
            else
            {
                ActorInfoPictureBox.Image = GetActorSpriteOrDefault(actorName, newActor.Variable4);
            }
        }

        private Bitmap GetActorSpriteOrDefault(string actorName, byte type)
        {
            string specificActorName = $"{actorName}_{type & 0x7F}";
            if (ACTOR_SPRITES.ContainsKey(specificActorName))
            {
                return ACTOR_SPRITES[specificActorName];
            }
            else if (ACTOR_SPRITES.ContainsKey(actorName))
            {
                return ACTOR_SPRITES[actorName];
            }
            else
            {
                return ACTOR_SPRITES[DEFAULT_SPRITE];
            }
        }

        // We don't want checking actors during DrawActors to call DrawActors...
        private bool _ignoreActorCheckbox = false;

        private void DrawActors(bool onlyDrawActorLayerActors)
        {
            _ignoreActorCheckbox = true;
            actorLayerGraphics.Clear(Color.Transparent);

            for (int i = 0; i < _level.Room.GetActorCount(); i++)
            {
                var actor = _level.Room.GetActor(i);
                //If we are here because the layer box has been changed then only draw actors of that layer
                if (onlyDrawActorLayerActors)
                {
                    if (actor.Layer == actorLayerComboBox.SelectedIndex)
                    {
                        //Check boxes of all actors that are being drawn
                        actorsCheckListBox.SetItemChecked(i, true);
                        DrawActor(actor);
                    }
                    else
                    {
                        actorsCheckListBox.SetItemChecked(i, false);
                    }
                }
                else if (actorsCheckListBox.GetItemChecked(i) == true)
                {
                    DrawActor(actor);
                }
            }
            roomLayerGraphics.DrawImage(actorLayerBitmap, 0, 0);
            layerPictureBox.Refresh();
            _ignoreActorCheckbox = false;
        }

        private void actorLayerComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateView(true);
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
            Bitmap actorSprite = GetActorSpriteOrDefault(actor.Name, actor.Variable4);

            int destinationX = (actor.XCoord * ACTOR_PIXELS_PER_COORDINATE) - (actorSprite.Width / 2);
            int destinationY = (actor.YCoord * ACTOR_PIXELS_PER_COORDINATE) - (actorSprite.Height / 2);

            actorLayerGraphics.DrawImage(
                actorSprite,
                destinationX,
                destinationY,
                actorSprite.Width,
                actorSprite.Height);
        }

        private void actorsSaveButton_Click(object sender, EventArgs e)
        {
            _level.SaveActors();
            BuildLayerActorList(true);
            MessageBox.Show("Changes Saved");
        }

        private void actorsAddNewButton_Click(object sender, EventArgs e)
        {
            int? activeLayer = GetHighestActiveLayerIndex();
            int baseLayer = activeLayer.HasValue
                ? (activeLayer > 7 ? activeLayer - 8 : activeLayer).Value
                : 0;

            Actor actor = _level.Room.AddActor(ACTOR_NAMES[0], (byte)baseLayer, (byte)lastActorCoordinates.x, (byte)lastActorCoordinates.y);

            BuildLayerActorList(true);
            DrawActors(true);
            SelectedActor(actor);

        }

        private void SelectedActor(Actor actor)
        {
            int index = _level.Room.IndexOf(actor);
            actorsCheckListBox.SetItemChecked(index, true);
            actorsCheckListBox.SelectedIndex = index;
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
        {
            Actor actor = new Actor();
            if (actor.PasteFromString(System.Windows.Forms.Clipboard.GetText()))
            {
                int? activeLayer = GetHighestActiveLayerIndex();
                int baseLayer = activeLayer.HasValue
                    ? (activeLayer > 7 ? activeLayer - 8 : activeLayer).Value
                    : 0;

                actor.Update((byte)baseLayer, (byte)lastActorCoordinates.x, (byte)lastActorCoordinates.y);
                _level.Room.AddActor(actor);
                BuildLayerActorList(true);
                DrawActors(true);
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
                UpdateView(false);
            }
        }

        private void actorsCheckListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_ignoreActorCheckbox)
            {
                return;
            }
            // Need to delay redraw because right now the newly checked actor won't have checked=true
            this.BeginInvoke((MethodInvoker)(() => UpdateView(false)));
        }

        private void buttonActorsSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < actorsCheckListBox.Items.Count; i++)
            {
                actorsCheckListBox.SetItemChecked(i, false);
            }
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
                UpdateView(false);
            }
        }

        #endregion Actors
    }
}