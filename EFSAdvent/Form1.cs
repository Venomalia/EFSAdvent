using AuroraLib.Core.Format.Identifier;
using AuroraLib.Pixel.BitmapExtension;
using AuroraLib.Pixel.Image;
using AuroraLib.Pixel.PixelProcessor;
using AuroraLib.Pixel.Processing;
using EFSAdvent.Controls;
using FSALib;
using FSALib.AssetDefinitions;
using FSALib.Renderer;
using FSALib.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BGRA32 = AuroraLib.Pixel.PixelFormats.BGRA<byte>;

namespace EFSAdvent
{
    public partial class Form1 : Form
    {
        private const string VERSION = "2.3";
        private const string BaseTitel = "EFSAdvent " + VERSION + " [Venomalia]";
        private const string WikiUrl = "https://github.com/Venomalia/EFSAdvent/wiki";
        private const string SourceCodeUrl = "https://github.com/Venomalia/EFSAdvent";

        const int ACTOR_PIXELS_PER_COORDINATE = 8;
        const int LAYER_DIMENSION_IN_PIXELS = Layer.DIMENSION * TILE_DIMENSION_IN_PIXELS;
        const int TILE_DIMENSION_IN_PIXELS = 16;

        private readonly History _history;
        private readonly TileBrush _tileBrush;
        private readonly Logger _logger;
        private readonly Identifier32[] _actorIDs;
        private readonly ToolTip _actorInfoToolTip = new ToolTip();

        private Stage _level = new Stage();
        private int _currentRoomIndex = -1;
        private bool _levelIsDirty = false;
        private string _levelFilePaht;
        private Rectangle _tileSelection;
        private (int x, int y) _tileSelectionOrigin;

        readonly Rarc dataRarc;
        readonly TilesetRenderer<BGRA32> tilesetRendererTV;
        readonly TilesetRenderer<BGRA32> tilesetRendererGBA;
        private TilesetRenderer<BGRA32> TilesetRenderer => (GetHighestActiveLayerIndex() % 8) == 0 ? tilesetRendererTV : tilesetRendererGBA;

        readonly Bitmap tileSheetBitmap, tileSheetBitmapGBA, roomLayerBitmap, brushTileBitmap;

        readonly SpriteRenderer<BGRA32> spriteRendererTV;
        readonly SpriteRenderer<BGRA32> spriteRendererGBA;
        readonly Bitmap actorLayerBitmap, actorBitmap;
        readonly Graphics roomLayerGraphics, actorLayerGraphics;

        Bitmap overlayBitmap;

        (int x, int y) lastActorCoordinates;

        int actorMouseDownOnIndex;

        private readonly Dictionary<string, Bitmap> ACTOR_SPRITES = new Dictionary<string, Bitmap>();

        private readonly HashSet<string> V6ACTORS = new HashSet<string>();

        private readonly string dataDirectory;

        public Form1()
        {
            InitializeComponent();
            ActorVariableFullInput.Controls[0].Enabled = false;
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

            string dataArcPath = Path.Combine(dataDirectory, "data.arc");
            if (!File.Exists(dataArcPath))
            {

                var result = MessageBox.Show("The required 'data.arc' file could not be found.\n" + "Please select the original 'data.arc' from your FSA game files.",
                                             "Missing data.arc",
                                             MessageBoxButtons.OKCancel,
                                             MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    var openDialog = new OpenFileDialog
                    {
                        Filter = "DATA RARC archive (data.arc)|data.arc",
                        CheckFileExists = true,
                        FileName = "data.arc"
                    };
                    if (openDialog.ShowDialog() != DialogResult.OK)
                        Close();
                    File.Copy(openDialog.FileName, dataArcPath);
                }
                else
                {
                    Close();
                }
            }
            using FileStream dataStream = File.OpenRead(dataArcPath);
            dataRarc = new Rarc(dataStream);
            tilesetRendererTV = new TilesetRenderer<BGRA32>(dataRarc);
            tilesetRendererGBA = new TilesetRenderer<BGRA32>(dataRarc);
            spriteRendererTV = new SpriteRenderer<BGRA32>(dataRarc);
            spriteRendererGBA = new SpriteRenderer<BGRA32>(dataRarc);

            tileSheetBitmap = new Bitmap(256, 1024);
            tileSheetBitmapGBA = new Bitmap(256, 1024);
            tileSheetPictureBox.Image = tileSheetBitmap;

            brushTileBitmap = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BrushTilePictureBox.Image = brushTileBitmap;

            roomLayerBitmap = new Bitmap(LAYER_DIMENSION_IN_PIXELS, LAYER_DIMENSION_IN_PIXELS);
            roomLayerGraphics = Graphics.FromImage(roomLayerBitmap);
            layerPictureBox.Image = roomLayerBitmap;

            BrushSizeComboBox.SelectedIndex = 0;

            actorLayerBitmap = new Bitmap(LAYER_DIMENSION_IN_PIXELS, LAYER_DIMENSION_IN_PIXELS, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            actorLayerGraphics = Graphics.FromImage(actorLayerBitmap);
            actorBitmap = new Bitmap(64 + 16, 64 + 16, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            _history = new History(500);
            _tileBrush = new TileBrush(_history);
            _logger = new Logger(loggerTextBox);

            // Load Actor Namelist from Assets
            _actorIDs = Assets.Actors.Keys.ToArray();
            foreach (var item in Assets.Actors)
            {
                ActorNameComboBox.Items.Add(item);
            }

            // Load V6 Typ Actors
            string V6ActorListPath = Path.Combine(dataDirectory, "V6 Typ Actors.txt");
            if (File.Exists(V6ActorListPath))
            {
                var names = File.ReadLines(V6ActorListPath);
                V6ACTORS = new HashSet<string>(names.Select(n => n.Trim()));
            }

            // Load Actor Sprites
            string spriteFolder = Path.Combine(dataDirectory, "actorsprites");
            var spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var spritePath in spritePaths)
            {
                var sprite = new Bitmap(spritePath);
                ACTOR_SPRITES.Add(spritePath.Split(Path.DirectorySeparatorChar).Last().Split('.')[0], sprite);
            }

            // Load Stamps
            string stampsFolder = Path.Combine(dataDirectory, "Stamps");
            if (Directory.Exists(stampsFolder))
            {
                foreach (var filePath in Directory.GetFiles(stampsFolder, "*.bin"))
                {
                    TileStampFlowLayoutPanel.Add(filePath);
                }
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

            ResetVarsForNewLevel();
            NewToolStripMenuItem_Click(this, null); // new
        }

        private void ResetVarsForNewLevel()
        {
            roomLayerGraphics.Clear(Color.Transparent);
            _history.Reset();
            actorMouseDownOnIndex = -1;
        }

        #region Dialogs

        private bool ShowSaveChangesDialog(string message = "Save all changes?")
        {
            if (_levelIsDirty)
            {
                var result = MessageBox.Show($"This level {_level.Index} has been changed, save changes first?", message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        saveChangesToolStripMenuItem_Click(this, null);
                        break;
                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region MenuStrip

        #region MenuStrip-File

        private void OpenLevelFile()
        {
            ResetVarsForNewLevel();

            if (_level != null)
                _level.Map.PropertyChanged -= Map_PropertyChanged;

            _levelIsDirty = false;
            _currentRoomIndex = -1;

            MapEditor.SetMap(_level.Map, _level);
            if (_level.MapSingleplayer is null)
            {
                if (MapTabControl.TabPages.Contains(MapSinglelplayerTabPage))
                    MapTabControl.TabPages.Remove(MapSinglelplayerTabPage);
                MapEditorSinglelplayer.Enabled = false;
            }
            else
            {
                if (!MapTabControl.TabPages.Contains(MapSinglelplayerTabPage))
                    MapTabControl.TabPages.Add(MapSinglelplayerTabPage);
                MapEditorSinglelplayer.Enabled = true;
                MapEditorSinglelplayer.SetMap(_level.MapSingleplayer, _level);
            }

            _level.Map.PropertyChanged += Map_PropertyChanged;

            ChangeOverlay();
            ChangeTileSheet();

            //Get a string which is just the root bossxxx filepath for loading other files
            RootFolderPathTextBox.Text = _levelFilePaht;
            if (Assets.Stages.ContainsKey(_level.Resources.Name))
                this.Text = $"{BaseTitel} - {Assets.Stages[_level.Resources.Name].Name}";
            else
                this.Text = $"{BaseTitel} - {_level.Resources.Name}";

            layerPictureBox.Refresh();

            SaveMenuItem.Enabled = true;
            SaveAsMenuItem.Enabled = true;
            tabControl.Enabled = true;
            rightSideGroupBox.Enabled = true;
            importToolStripMenuItem.Enabled = true;
            LoadRoom(_level.Map[_level.Map.StartX, _level.Map.StartY], false);
        }


        private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _levelIsDirty = true;
            switch (e.PropertyName)
            {
                case "OverlayTextureId":
                    ChangeOverlay();
                    break;
                case "TileSheetId":
                case "NPCSheetID":
                    ChangeTileSheet();
                    break;
                default:
                    break;
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog())
                return;

            _level.Dispose();
            _level = new Stage();
            _level.Map.StartX = 4;
            _level.Map.StartY = 4;
            OpenLevelFile();
        }

        private void OpenLevel(object sender, EventArgs e)
        {
            if (ShowSaveChangesDialog())
                return;

            var openDialog = new OpenFileDialog
            {
                Filter = "Supported files (*.arc;*.csv)|*.arc;*.csv|" + "RARC archive files (*.arc)|*.arc|" + "CSV map files (*.csv)|*.csv",
                CheckFileExists = true
            };


            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                if (openDialog.FileName.EndsWith("csv")) // Load folder
                {
                    _levelFilePaht = openDialog.FileName.Remove(openDialog.FileName.LastIndexOf("map" + Path.DirectorySeparatorChar)); ;
                    _level.Dispose();
                    _level = new Stage(new Rarc(new DirectoryInfo(_levelFilePaht)));
                }
                else
                {
                    _levelFilePaht = openDialog.FileName;
                    using FileStream data = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read);
                    _level.ReadFromStream(data);
                }
                OpenLevelFile();
            }
            catch (Exception err)
            {
                MessageBox.Show($"Failed to load \"{openDialog.FileName}\",\n {err.Message}.");
                return;
            }

        }

        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(_levelFilePaht))
            {
                _level.WriteToDirectory(_levelFilePaht);
            }
            if (File.Exists(_levelFilePaht))
            {
                using FileStream data = new FileStream(_levelFilePaht, FileMode.Create, FileAccess.ReadWrite);
                _level.WriteToStream(data);
            }
            else
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            _levelIsDirty = false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "RARC archive (*.arc)|*.arc",
                FileName = $"boss{_level.Map.Index:D3}.arc",
                DefaultExt = "arc",
                AddExtension = true,
                CheckFileExists = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _levelFilePaht = dialog.FileName;
                string fileName = Path.GetFileName(_levelFilePaht);
                if (fileName.Length >= 7 && int.TryParse(fileName.Substring(4, 3), out int index))
                    _level.Index = index;

                using FileStream data = new FileStream(_levelFilePaht, FileMode.Create, FileAccess.ReadWrite);
                _level.WriteToStream(data);
                OpenLevelFile();
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
            => Application.Exit();

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
            => e.Cancel = ShowSaveChangesDialog("Save all changes before exiting?");

        #endregion

        #region MenuStrip-Export

        private void ExportRoomAsTmx_Click(object sender, EventArgs e)
        {
            var saveTmx = new SaveFileDialog
            {
                DefaultExt = ".tmx",
                AddExtension = true,
                Filter = "Tiled map files|*.tmx;*.xml",
                FileName = $"boss{_level.Map.Index:D3}_Room{_currentRoomIndex}"
            };

            if (saveTmx.ShowDialog() == DialogResult.OK)
            {
                int TileSheetId = _level.Map.GetRoomProperties(_currentRoomIndex).TileSheetId;
                string tilesetSource = $"Tile Sheet {TileSheetId:D2}.tsx";
                string tsxFilePath = Path.Combine(Path.GetDirectoryName(saveTmx.FileName), tilesetSource);
                ExportMapTilesetAsTsx(tsxFilePath);
                Tiled.ExportAsTMX(_level.Rooms[_currentRoomIndex], saveTmx.FileName, tilesetSource);
            }
        }

        private void ExportMapTilesetAsTsx(string savePath)
        {
            int TileSheetId = _level.Map.GetRoomProperties(_currentRoomIndex).TileSheetId;
            string tiledBaseSheet = Path.Combine(dataDirectory, "TiledBaseSheet.xml");

            string tileSheetFileName = $"TileSheet_{Assets.Tilesets[TileSheetId].ID}.PNG";
            string tileSheetPath = Path.Combine(Path.GetDirectoryName(savePath), tileSheetFileName);
            tileSheetBitmap.Save(tileSheetPath);
            Tiled.UpdateTilesetImage(tiledBaseSheet, tileSheetFileName, savePath);
        }

        private void ExportViewAsPng(object sender, EventArgs e)
        {
            var savePng = new SaveFileDialog
            {
                DefaultExt = ".png",
                AddExtension = true,
                Filter = "Portable Network Graphics|*.png",
                FileName = $"boss{_level.Map.Index:D3}_{_currentRoomIndex}"
            };

            if (savePng.ShowDialog() == DialogResult.OK)
            {
                roomLayerBitmap.Save(savePng.FileName);
            }
        }

        private void ExportLevelAsPng(object sender, EventArgs e)
        {
            var savePng = new SaveFileDialog
            {
                DefaultExt = ".png",
                AddExtension = true,
                Filter = "Portable Network Graphics|*.png",
                FileName = $"boss{_level.Map.Index:D3}"
            };

            if (savePng.ShowDialog() == DialogResult.OK)
            {

                bool autoLoadActors = autoSelectToolStripMenuItem.Checked;
                int lastRoom = _currentRoomIndex;
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
                            int roomValue = _level.Map[x, y];

                            if (roomValue != MapLayout.EMPTY_ROOM_VALUE)
                            {
                                LoadRoom(roomValue, false);

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

                autoSelectToolStripMenuItem.Checked = autoLoadActors;
                LoadRoom(lastRoom, false);
            }
        }

        private void ExportRoomsAsPng(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select a save location for the images.";
            folderDialog.ShowNewFolderButton = true;

            if (folderDialog.ShowDialog() != DialogResult.OK)
                return;

            bool autoLoadActors = autoSelectToolStripMenuItem.Checked;
            bool overlay = displayOverlayToolStripMenuItem.Checked;
            int lastRoom = _currentRoomIndex;
            autoSelectToolStripMenuItem.Checked = sender is ToolStripItem csender && csender.Name == "allRoomsAndActorsAspngToolStripMenuItem";
            displayOverlayToolStripMenuItem.Checked = false;

            for (int room = 0; room < byte.MaxValue; room++)
            {
                if (_level.Rooms[(byte)room] != null)
                {
                    LoadRoom(room, false);
                    string baseFileName = $"boss{_level.Map.Index:D3}_room{room}";

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
                                using Bitmap baselayer = roomLayerBitmap.Clone(new Rectangle(0, 0, 512, 384), roomLayerBitmap.PixelFormat);
                                baselayer.Save(path, System.Drawing.Imaging.ImageFormat.Png);
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

            autoSelectToolStripMenuItem.Checked = autoLoadActors;
            displayOverlayToolStripMenuItem.Checked = overlay;
            LoadRoom(lastRoom, false);
        }
        #endregion

        #region MenuStrip-Import
        private void ImportRoomFromTmx_Click(object sender, EventArgs e)
        {
            var openTmx = new OpenFileDialog
            {
                DefaultExt = ".tmx",
                Filter = "Tiled map files|*.tmx;*.xml",
                FileName = $"boss{_level.Map.Index:D3}_Room{_currentRoomIndex}.tmx"
            };

            if (openTmx.ShowDialog() == DialogResult.OK)
            {
                _level.Rooms[_currentRoomIndex] = Tiled.ImportRoomFromTMX(openTmx.FileName);
                UpdateView();
            }
        }

        private void roomImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog { Filter = "Room actors files|d_enemy_map*.bin" };
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string name = Path.GetFileName(openDialog.FileName);
            int mapNr = int.Parse(name.Substring(11, 3));
            int RoomNR = int.Parse(name.Substring(15, 2));
            string basePath = openDialog.FileName.Remove(openDialog.FileName.LastIndexOf("bin" + Path.DirectorySeparatorChar));
            int freeRoomNr = _level.GetNextFreeRoom();

            string actors = ActorList.GetFilePath(basePath, mapNr, RoomNR);
            string newActors = ActorList.GetFilePath(RootFolderPathTextBox.Text, _level.Map.Index, freeRoomNr);
            File.Copy(actors, newActors);

            for (int la = 0; la < 8; la++)
            {
                for (int lv = 1; lv <= 2; lv++)
                {
                    string layer = Layer.GetFilePath(basePath, mapNr, RoomNR, lv, la);
                    string newLayer = Layer.GetFilePath(RootFolderPathTextBox.Text, _level.Map.Index, freeRoomNr, lv, la);
                    File.Copy(layer, newLayer);
                }
            }

            MessageBox.Show($"Room successfully imported to room {freeRoomNr}.");
            if (MapEditor.SelectedRoomCoordinates != (-1, -1))
            {
                var result = MessageBox.Show($"Should the imported room be set to the selected position [{MapEditor.SelectedRoomCoordinates.X},{MapEditor.SelectedRoomCoordinates.Y}]?",
                                             $"Set to position?",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _level.Map[MapEditor.SelectedRoomCoordinates.X, MapEditor.SelectedRoomCoordinates.Y] = freeRoomNr;
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

            if (!File.Exists(openDialog.FileName))
            {
                MessageBox.Show($"File not found: {openDialog.FileName}");
            }
            else
            {
                using (FileStream actorListStream = File.Open(openDialog.FileName, FileMode.Open))
                {
                    _level.Rooms[_currentRoomIndex].Actors.ReadFromStream(actorListStream);
                }
                BuildLayerActorList(true);
                DrawActors();

                MessageBox.Show($"{_level.Rooms[_currentRoomIndex].Actors.Count} actors have been successfully added to the current room.");
            }
        }

        #endregion

        #region MenuStrip-Edit

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_history.TryUndoAction(_level.Rooms[_currentRoomIndex], out int layer))
            {
                if (actorsCheckListBox.Items.Count != _level.Rooms[_currentRoomIndex].Actors.Count)
                {
                    BuildLayerActorList();
                    actorLayerComboBox_SelectionChangeCommitted(sender, e);
                }
                UpdateLayerCheckListColor(layer);
                UpdateView(layer);
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_history.TryRedoAction(_level.Rooms[_currentRoomIndex], out int layer))
            {
                if (actorsCheckListBox.Items.Count != _level.Rooms[_currentRoomIndex].Actors.Count)
                {
                    BuildLayerActorList();
                    actorLayerComboBox_SelectionChangeCommitted(sender, e);
                }
                UpdateLayerCheckListColor(layer);
                UpdateView(layer);
            }
        }
        #endregion

        #region MenuStrip-View

        private void OneXSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Size = new Size(974, 614);
        }

        private void TwoXSizeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Size = new Size(1486, 1126);
        }

        private void UpdateView_Click(object sender, EventArgs e)
            => UpdateView();

        private void bilinearToolStripMenuItem_Click(object sender, EventArgs e)
            => SetInterpolationMode(InterpolationMode.Bilinear);

        private void bicubicToolStripMenuItem_Click(object sender, EventArgs e)
            => SetInterpolationMode(InterpolationMode.Bicubic);

        private void nearestNeighborToolStripMenuItem_Click(object sender, EventArgs e)
            => SetInterpolationMode(InterpolationMode.NearestNeighbor);

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

        #endregion

        #region MenuStrip-Help
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Editor for Four Swords Adventures by JaytheHam & Venomalia. v{VERSION}"
                + "\nwww.jaytheham.com"
                + "\nSend comments, bug reports etc to: jaytheham@gmail.com", $"EFSAdvent version {VERSION}");
        }

        private void WikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open the Wiki page in the default web browser
            System.Diagnostics.Process.Start(WikiUrl);
        }

        private void sourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open the Source code page in the default web browser
            System.Diagnostics.Process.Start(SourceCodeUrl);
        }
        #endregion

        #endregion

        #region Taps

        private void tabControl_TabIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == (int)TabControlIndex.Actor)
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

        #region MapInfo Tab

        private void mapEditor_LoadRoom(object sender, Controls.MapEditor e) => LoadRoom(e.SelectedRoomID, false);
        private void mapEditor_NewRoom(object sender, Controls.MapEditor e) => LoadRoom(e.SelectedRoomID, true);

        private void mapEditor_RemoveRoom(object sender, Controls.MapEditor e)
        {
            int roomToRemove = e.SelectedRoomID;
            var result = MessageBox.Show(
                $"Room {roomToRemove} is not used anywhere in the level map.\n\n" +
                "Do you want to delete it completely? This action cannot be undone!",
                $"Delete room {roomToRemove} permanently?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                _level.Rooms[roomToRemove] = null;
                if (roomToRemove == _currentRoomIndex)
                {
                    LoadRoom(-1, false);
                }
            }
        }

        private void mapEditor_SelectedRoomCoordinatesChanged(object sender, Controls.MapEditor e)
        {
            CoridinatesTextBox.Clear();
            CoridinatesTextBox.AppendText($"Map coordinates: x{e.SelectedRoomCoordinates.X} y{e.SelectedRoomCoordinates.Y}");
        }

        private void LoadRoom(int roomID, bool newRoom)
        {
            actorAttributesgroupBox.Enabled = false;
            if (!newRoom && roomID == MapLayout.EMPTY_ROOM_VALUE)
            {
                roomLayerBitmap.Clear(Color.Transparent);
                layerPictureBox.Enabled = false;
                actorsCheckListBox.Enabled = false;
                ExportMenuItem.Enabled = false;
                UpdateView();
                return;
            }

            int newRoomNumber = newRoom ? (_level.IsRoomInUse(roomID) ? (byte)_level.GetNextFreeRoom() : roomID) : roomID;
            if (newRoomNumber != MapLayout.EMPTY_ROOM_VALUE)
            {
                _currentRoomIndex = newRoomNumber;

                _history.Reset();
                BuildLayerActorList(false);

                if (_level.Map.IsShadowBattle)
                {
                    ChangeTileSheet();
                    ChangeOverlay();
                }

                //Enable all the actor buttons now that data to work with exists
                actorDeleteButton.Enabled = true;
                actorLayerComboBox.Enabled = true;
                actorsCheckListBox.Enabled = true;
                layerPictureBox.Enabled = true;
                ExportMenuItem.Enabled = true;

                for (int i = 1; i < 16; i++)
                {
                    layersCheckList.SetItemChecked(i, false);
                }
                layersCheckList.SetItemChecked(0, true);
                layersCheckList.SetItemChecked(8, true);
                if (newRoom)
                {
                    _level.Rooms[newRoomNumber] = new Room();
                    var baseLayer = _level.Rooms[newRoomNumber].Layers[0];
                    for (int y = 0; y < 24; y++)
                    {
                        for (int x = 0; x < Layer.DIMENSION; x++)
                        {
                            baseLayer[x, y] = 432;
                        }
                    }
                }

                for (int i = 0; i < 16; i++)
                {
                    Color color = _level.Rooms[newRoomNumber].Layers[i].IsEmpty ? Color.Gray : Color.Black;
                    layersCheckList.SetItemColor(i, color);
                }
                layersCheckList.Refresh();

                if (autoSelectToolStripMenuItem.Checked)
                {
                    SelectAllLayerActors();
                    actorLayerComboBox.SelectedIndex = 0;
                }
                else
                {
                    actorLayerComboBox.SelectedIndex = -1;
                }

                UpdateView();
            }
        }

        #endregion

        #region TileSheet Tap

        private void BrushSizeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
            => UpdateBrushTileBitmap(_tileBrush.TileValue);

        //When a TileSheet tile is clicked on load its value into the brush
        private void tilePictureBox_MouseClick(object sender, MouseEventArgs e)
            => UpdateBrushTileBitmap((ushort)(((e.Y / TILE_DIMENSION_IN_PIXELS) * TILE_DIMENSION_IN_PIXELS) + (e.X / TILE_DIMENSION_IN_PIXELS)));

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

            {
                using var tileImage = (MemoryImage<BGRA32>)brushTileBitmap.AsAuroraImage();
                tileImage.Clear();
                TilesetRenderer.DrawTile(tileImage, 0, 0, _tileBrush.TileValue);
            }

            _logger.Clear();
            if (Assets.TileProperties.TryGetValue(_tileBrush.TileValue, out TilePropertyDefinition propertie))
            {
                _logger.AppendLine(propertie.Name);
                _logger.AppendLine(string.Empty);
                _logger.AppendLine(propertie.Description);
                _logger.AppendLine(string.Empty);
                if (propertie.RequiredActorID.HasValue)
                {
                    _logger.AppendLine($"Required Actor: '{propertie.RequiredActorID}'");
                }
            }

            BrushTilePictureBox.Refresh();
        }

        #endregion

        #region Actor Tap

        #region All Actors

        private void ActorsCheckListBox_SelectedIndexChanged(object sender, EventArgs e)
            => NewActorSelected();

        private void BuildLayerActorList(bool keepState = true)
        {
            object[] checkedActorIds = keepState ? actorsCheckListBox.CheckedItems.Cast<object>().ToArray() : null;
            object selectedActorId = actorsCheckListBox.SelectedItem;

            actorsCheckListBox.BeginUpdate();

            try
            {
                actorsCheckListBox.Items.Clear();

                if (_level.Rooms[_currentRoomIndex] == null)
                    return;

                foreach (var actor in _level.Rooms[_currentRoomIndex].Actors)
                {
                    actorsCheckListBox.Items.Add(actor);
                }

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
            finally
            {
                actorsCheckListBox.EndUpdate();
            }
        }


        private void actorLayerComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (actorLayerComboBox.SelectedIndex == 0)
                SelectAllLayerActors();
            else
                SelectActorsByVariables(actorLayerComboBox.SelectedIndex);
            UpdateView();
        }

        private void SelectAllLayerActors()
        {
            _ignoreActorCheckbox = true;

            for (int i = 0; i < _level.Rooms[_currentRoomIndex].Actors.Count; i++)
                actorsCheckListBox.SetItemChecked(i, true);

            _ignoreActorCheckbox = false;
        }

        private void SelectActorsByVariables(int variable)
        {
            _ignoreActorCheckbox = true;

            for (int i = 0; i < _level.Rooms[_currentRoomIndex].Actors.Count; i++)
            {
                var actor = _level.Rooms[_currentRoomIndex].Actors[i];
                bool isChecked = actor.VariableByte4 >> 3 == variable;

                if (!isChecked && V6ACTORS.Contains(actor.Name))
                {
                    if (((actor.VariableByte4 & 0x7) << 2 | actor.VariableByte3 >> 6) == variable)// V2
                        isChecked = true;
                    else if (((actor.VariableByte3 >> 1) & 0x1F) == variable)// V3
                        isChecked = true;
                    else if (((actor.VariableByte3 & 0x1) << 4 | actor.VariableByte2 >> 4) == variable)// V4
                        isChecked = true;
                    else if (((actor.VariableByte2 & 0xF) << 1 | actor.VariableByte1 >> 7) == variable)// V5
                        isChecked = true;
                }

                actorsCheckListBox.SetItemChecked(i, isChecked);
            }
            _ignoreActorCheckbox = false;
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

        // We don't want checking actors during DrawActors to call DrawActors...
        private bool _ignoreActorCheckbox = false;

        private void DrawActors()
        {
            _ignoreActorCheckbox = true;
            actorLayerGraphics.Clear(Color.Transparent);
            var actors = _level.Rooms[_currentRoomIndex].Actors.AsSpan();
            bool hasPathActors = false;

            for (int i = 0; i < actors.Length; i++)
            {
                if (actorsCheckListBox.GetItemChecked(i) == true)
                {
                    var actor = actors[i];
                    DrawActor(actor);

                    if (actor.Name == "FSPO" || actor.Name == "RAIL")
                        hasPathActors = true;
                }
            }

            if (hasPathActors && tabControl.SelectedIndex == (int)TabControlIndex.Actor)
            {
                var paths = new Dictionary<ushort, List<int>>();
                BuildPathActors(actors, paths);
                DrawPathActors(actorLayerGraphics, actors, paths, GetHighestActiveLayerIndex().Value % 8);
            }

            roomLayerGraphics.DrawImage(actorLayerBitmap, 0, 0);
            layerPictureBox.Refresh();
            _ignoreActorCheckbox = false;
        }

        private void DrawPathActors(Graphics layerGraphics, ReadOnlySpan<Actor> actors, Dictionary<ushort, List<int>> paths, int layer)
        {
            int ci = 0;

            foreach (var path in paths.Values)
            {
                Color color = GetNextColor();
                int change = 160 / path.Count;

                int start, end;
                for (int i = 1; i < path.Count; i++)
                {
                    start = path[i - 1];
                    end = path[i];
                    if (start == -1 || end == -1)
                        continue;

                    DrawLineBetweenActors(color, actors[start], actors[end]);

                    color = Color.FromArgb(color.A - change, color.R, color.G, color.B);
                }

                start = path[0];
                end = path[path.Count - 1];
                if (start != -1 && end != -1 && actors[start].Name == "RAIL")
                {
                    DrawLineBetweenActors(color, actors[start], actors[end]);
                }
            }
            return;

            Color GetNextColor() => ci++ switch
            {
                0 => Color.Lime,
                1 => Color.Red,
                2 => Color.Cyan,
                3 => Color.Magenta,
                4 => Color.Yellow,
                _ => Color.HotPink,
            };

            void DrawLineBetweenActors(Color color, Actor startActor, Actor endActor)
            {
                if (startActor.Layer == layer && endActor.Layer == layer)
                {
                    Point startPosition = new Point(startActor.XCoord * ACTOR_PIXELS_PER_COORDINATE, startActor.YCoord * ACTOR_PIXELS_PER_COORDINATE);
                    Point endPosition = new Point(endActor.XCoord * ACTOR_PIXELS_PER_COORDINATE, endActor.YCoord * ACTOR_PIXELS_PER_COORDINATE);

                    layerGraphics.DrawLineWithDropShadow(color, startPosition, endPosition);
                }
            }
        }

        private static void BuildPathActors(ReadOnlySpan<Actor> actors, Dictionary<ushort, List<int>> paths)
        {
            paths.Clear();

            ushort id, index;
            for (int i = 0; i < actors.Length; i++)
            {
                var actor = actors[i];

                switch (actor.Name)
                {
                    case "FSPO":
                        id = (ushort)(actor.VariableByte4 + byte.MaxValue);
                        index = actor.VariableByte1;
                        break;
                    case "RAIL":
                        id = actor.VariableByte2;
                        index = actor.VariableByte3;
                        break;
                    default:
                        continue;
                }

                if (!paths.TryGetValue(id, out var points))
                {
                    points = new List<int>(16);
                    paths.Add(id, points);
                }

                while (points.Count <= index)
                    points.Add(-1);

                points[index] = i;
            }
        }

        private void AddNewActorFromCode(string code)
        {
            if (Actor.PasteFromString(code, out Actor actor))
            {
                int? activeLayer = GetHighestActiveLayerIndex();
                int baseLayer = activeLayer.HasValue
                    ? (activeLayer > 7 ? activeLayer - 8 : activeLayer).Value
                    : 0;

                actor.Layer = (byte)baseLayer;
                actor.XCoord = (byte)lastActorCoordinates.x;
                actor.YCoord = (byte)lastActorCoordinates.y;
                _level.Rooms[_currentRoomIndex].Actors.Add(actor);
                int newIndex = _level.Rooms[_currentRoomIndex].Actors.IndexOf(actor);
                actorsCheckListBox.Items.Insert(newIndex, actor);
                SelectedActor(newIndex);
            }
        }

        private void actorDeleteButton_Click(object sender, EventArgs e)
        {
            if (_level.Rooms[_currentRoomIndex].Actors.Count > actorsCheckListBox.SelectedIndex && actorsCheckListBox.SelectedIndex > -1)
            {
                _level.Rooms[_currentRoomIndex].Actors.RemoveAt(actorsCheckListBox.SelectedIndex);
                actorsCheckListBox.Items.RemoveAt(actorsCheckListBox.SelectedIndex);
                actorsCheckListBox.SelectedIndex = -1;
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

        private void actorsCheckListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _level.Rooms[_currentRoomIndex].Actors.Count <= actorsCheckListBox.SelectedIndex || actorsCheckListBox.SelectedIndex < 0)
            {
                _level.Rooms[_currentRoomIndex].Actors.RemoveAt(actorsCheckListBox.SelectedIndex);
                BuildLayerActorList(true);
                if (actorsCheckListBox.Items.Count > 0)
                {
                    actorsCheckListBox.SetSelected(0, true);
                }
                UpdateView();
            }
        }

        #endregion

        #region Current Actor

        private void CopyActorToClipboard(object sender, EventArgs e)
        {
            string actor = _level.Rooms[_currentRoomIndex].Actors[actorsCheckListBox.SelectedIndex].ToStringCode();
            System.Windows.Forms.Clipboard.SetText(actor);
        }

        private bool _ignoreActorChanges = false;

        private void ActorChanged(object sender, EventArgs e)
        {
            if (_ignoreActorChanges)
            {
                return;
            }

            var actorName = (KeyValuePair<Identifier32, ActorDefinition>)ActorNameComboBox.SelectedItem;
            Actor actor = new Actor()
            {
                ID = actorName.Key,
                Layer = (byte)ActorLayerInput.Value,
                XCoord = (byte)ActorXCoordInput.Value,
                YCoord = (byte)ActorYCoordInput.Value,
                VariableByte4 = (byte)((int)ActorVariable4AInput.Value << 3 | (int)ActorVariable4BInput.Value),
                VariableByte3 = (byte)ActorVariable3Input.Value,
                VariableByte2 = (byte)ActorVariable2Input.Value,
                VariableByte1 = (byte)ActorVariable1Input.Value
            };

            UpdateActor(actorsCheckListBox.SelectedIndex, actor);
            CreateSelectedActorFields(actor);
            UpdateView();
        }

        private void ActorChangedV6(object sender, EventArgs e)
        {
            if (_ignoreActorChanges)
            {
                return;
            }
            _ignoreActorChanges = true;
            ActorVariable4AInput.Value = ActorV6Variable6Input.Value;
            ActorVariable4BInput.Value = (int)ActorV6Variable5Input.Value >> 2;
            ActorVariable3Input.Value = ((int)ActorV6Variable5Input.Value & 0x3) << 6 | (int)ActorV6Variable4Input.Value << 1 | (int)ActorV6Variable3Input.Value >> 4;
            ActorVariable2Input.Value = ((int)ActorV6Variable3Input.Value & 0xF) << 4 | (int)ActorV6Variable2fInput.Value >> 1;
            ActorVariable1Input.Value = ((int)ActorV6Variable2fInput.Value & 0x1) << 7 | (int)ActorV6Variable1Input.Value;
            _ignoreActorChanges = false;
            ActorChanged(sender, e);
        }

        private void UpdateActor(int index, Actor actor)
        {
            bool isSelected = actorsCheckListBox.SelectedIndex == index;
            if (Assets.Actors.TryGetValue(actor.ID, out ActorDefinition schema) && schema.Category == ActorCategoryType.TileObject)
            {
                actor.XCoord = (byte)(actor.XCoord / 2 * 2);
                actor.YCoord = (byte)(actor.YCoord / 2 * 2);
            }

            _level.Rooms[_currentRoomIndex].Actors[index] = actor;

            if (isSelected)
                UpdateSelectedActorUI(actor);

            int newIndex = _level.Rooms[_currentRoomIndex].Actors.IndexOf(actor);
            if (newIndex != index)
            {
                _ignoreActorChanges = true;
                bool isChecked = actorsCheckListBox.GetItemChecked(index);
                actorsCheckListBox.Items.RemoveAt(index);
                actorsCheckListBox.Items.Insert(newIndex, actor);
                actorsCheckListBox.SetItemChecked(newIndex, isChecked);
                _ignoreActorChanges = false;

                if (isSelected)
                {
                    actorsCheckListBox.SelectedIndex = newIndex;
                    UpdateSelectedActorUI(actor);
                }
            }
        }

        private void actorsCheckListBox_Click(object sender, EventArgs e)
            => SelectedActor(actorsCheckListBox.SelectedIndex);

        private void NewActorSelected()
        {
            if (_ignoreActorChanges)
                return;

            if (actorsCheckListBox.SelectedIndex == -1)
            {
                actorAttributesgroupBox.Enabled = false;
                return;
            }

            var newActor = _level.Rooms[_currentRoomIndex].Actors[actorsCheckListBox.SelectedIndex];
            UpdateSelectedActorUI(newActor);
            CreateSelectedActorFields(newActor);
        }

        private void UpdateSelectedActorUI(in Actor actor)
        {
            _ignoreActorChanges = true;
            ActorNameComboBox.SelectedIndex = Array.IndexOf(_actorIDs, actor.ID);
            ActorLayerInput.Value = actor.Layer;
            ActorXCoordInput.Value = actor.XCoord;
            ActorYCoordInput.Value = actor.YCoord;
            ActorVariableFullInput.Value = actor.Variable;
            // V5
            ActorVariable4AInput.Value = actor.VariableByte4 >> 3;
            ActorVariable4BInput.Value = actor.VariableByte4 & 0x7;
            ActorVariable3Input.Value = actor.VariableByte3;
            ActorVariable2Input.Value = actor.VariableByte2;
            ActorVariable1Input.Value = actor.VariableByte1;
            // V6
            ActorV6Variable6Input.Value = ActorVariable4AInput.Value;
            ActorV6Variable5Input.Value = (int)ActorVariable4BInput.Value << 2 | (int)ActorVariable3Input.Value >> 6;
            ActorV6Variable4Input.Value = ((int)ActorVariable3Input.Value >> 1) & 0x1F;
            ActorV6Variable3Input.Value = ((int)ActorVariable3Input.Value & 0x1) << 4 | (int)ActorVariable2Input.Value >> 4;
            ActorV6Variable2fInput.Value = ((int)ActorVariable2Input.Value & 0xF) << 1 | (int)ActorVariable1Input.Value >> 7;
            ActorV6Variable1Input.Value = (int)ActorVariable1Input.Value & 0x7F;
            _ignoreActorChanges = false;

            string name = actor.Name;
            if (name == "PNPC" || name == "PNP2")
            {
                UpdateBrushTileBitmap((ushort)((actor.VariableByte2 & 0x3) << 8 | actor.VariableByte1));
                ActorInfoPictureBox.Image = brushTileBitmap;
            }
            else
            {
                using (var brushImage = (MemoryImage<BGRA32>)actorBitmap.AsAuroraImage())
                {
                    brushImage.Clear();
                    DrawActorToImage(actor, new Point(brushImage.Width / 2, brushImage.Height / 2 + 16), brushImage);
                }
                ActorInfoPictureBox.Image = actorBitmap;
            }
        }

        private void CreateSelectedActorFields(in Actor newActor)
        {
            panelActorFields.SuspendLayout();
            panelActorFields.Controls.Clear();
            if (Assets.Actors.TryGetValue(newActor.ID, out ActorDefinition schema))
            {
                _actorInfoToolTip.RemoveAll();
                _actorInfoToolTip.SetToolTip(ActorNameComboBox, schema.Description);

                foreach (VariableField field in schema.Fields)
                {
                    Label label = new Label
                    {
                        Text = field.Name,
                        AutoSize = true,
                    };
                    Control inputControl;
                    switch (field.ValueType)
                    {
                        case FSALib.AssetDefinitions.ValueType.Integer:
                            var numericUpDown = new NumericUpDown()
                            {
                                Minimum = 0,
                                Maximum = (1 << field.BitSize) - 1,
                                Value = (int)field.ReadActorField(newActor.Variable)
                            };
                            numericUpDown.ValueChanged += new System.EventHandler(ActorFieldChanged);
                            inputControl = numericUpDown;
                            break;

                        case FSALib.AssetDefinitions.ValueType.Boolean:
                            var checkBox = new CheckBox()
                            {
                                AutoSize = true,
                                Checked = field.ReadActorField(newActor.Variable) == 1
                            };
                            checkBox.CheckedChanged += new System.EventHandler(ActorFieldChanged);
                            inputControl = checkBox;
                            break;

                        case FSALib.AssetDefinitions.ValueType.Enum:
                            ComboBox comboBox = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, };
                            int value = (int)field.ReadActorField(newActor.Variable);
                            foreach (var enumValue in field.EnumValues)
                            {
                                ValueTuple<int, string> data = (enumValue.Key, enumValue.Value.Name);
                                comboBox.Items.Add(data);
                                if (value == enumValue.Key)
                                {
                                    comboBox.SelectedIndex = comboBox.Items.Count - 1;
                                }
                            }
                            if (comboBox.SelectedIndex == -1)
                            {
                                comboBox.Items.Add((value, "Not documented!"));
                                comboBox.SelectedIndex = comboBox.Items.Count - 1;
                            }
                            comboBox.SelectedIndexChanged += new System.EventHandler(ActorFieldChanged);
                            inputControl = comboBox;
                            break;

                        case FSALib.AssetDefinitions.ValueType.Flags:
                            CheckedListBox checkedListBox = new CheckedListBox();

                            int flagsValue = (int)field.ReadActorField(newActor.Variable);
                            int processedFlags = flagsValue;

                            foreach (var flagValue in field.EnumValues)
                            {
                                bool isChecked = (flagsValue & flagValue.Key) != 0;
                                checkedListBox.Items.Add((flagValue.Key, flagValue.Value.Name), isChecked);

                                processedFlags &= ~flagValue.Key;
                            }

                            int bit = 1;
                            while (processedFlags != 0)
                            {
                                if ((processedFlags & bit) != 0)
                                {
                                    checkedListBox.Items.Add((bit, $"Not documented!"), true);
                                    processedFlags &= ~bit;
                                }
                                bit <<= 1;
                            }

                            checkedListBox.SelectedIndexChanged += new System.EventHandler(ActorFieldChanged);
                            inputControl = checkedListBox;
                            break;

                        default:
                            continue;
                    }
                    inputControl.Margin = new Padding(0, 0, 0, 10);
                    inputControl.Width = panelActorFields.Width - 10;
                    inputControl.Tag = field;

                    // set ToolTip
                    if (!string.IsNullOrEmpty(field.Description))
                    {
                        _actorInfoToolTip.SetToolTip(label, field.Description);
                        _actorInfoToolTip.SetToolTip(inputControl, field.Description);
                    }

                    panelActorFields.Controls.Add(label);
                    panelActorFields.Controls.Add(inputControl);
                }
            }
            panelActorFields.ResumeLayout();
        }

        private void ActorFieldChanged(object sender, EventArgs e)
        {
            Control inputControl = (Control)sender;
            VariableField field = (VariableField)inputControl.Tag;
            uint newValue;
            if (inputControl is NumericUpDown numericUpDown)
            {
                newValue = (uint)numericUpDown.Value;
            }
            else if (inputControl is CheckBox checkBox)
            {
                newValue = checkBox.Checked ? 1u : 0u;
            }
            else if (inputControl is ComboBox comboBox)
            {
                ValueTuple<int, string> selectedItem = (ValueTuple<int, string>)comboBox.SelectedItem;
                newValue = (uint)selectedItem.Item1;
            }
            else if (inputControl is CheckedListBox checkedListBox)
            {
                newValue = 0;

                foreach (var item in checkedListBox.CheckedItems)
                {
                    ValueTuple<int, string> flagItem = (ValueTuple<int, string>)item;
                    newValue |= (uint)flagItem.Item1;
                }
            }
            else
            {
                return;
            }
            uint variableFull = field.UpdateActorField((uint)ActorVariableFullInput.Value, newValue);

            var actorName = (KeyValuePair<Identifier32, ActorDefinition>)ActorNameComboBox.SelectedItem;
            Actor actor = new Actor()
            {
                ID = actorName.Key,
                Layer = (byte)ActorLayerInput.Value,
                XCoord = (byte)ActorXCoordInput.Value,
                YCoord = (byte)ActorYCoordInput.Value,
                Variable = variableFull,
            };

            UpdateActor(actorsCheckListBox.SelectedIndex, actor);
            UpdateView();
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
            int index = _level.Rooms[_currentRoomIndex].Actors.IndexOf(actor);
            return actorsCheckListBox.SelectedIndex == index;
        }

        #endregion

        #endregion

        #endregion

        #region View

        private void UpdateView(int? layer = null)
        {
            if (_currentRoomIndex == -1 || _level?.Rooms[_currentRoomIndex] == null)
                return;

            using (var layerImage = (MemoryImage<BGRA32>)roomLayerBitmap.AsAuroraImage())
            {
                layerImage.Clear();
                for (int i = 0; i < 8; i++)
                {
                    // Is TV layer or GBA?
                    var renderer = (i == 0) ? tilesetRendererTV : tilesetRendererGBA;

                    // Draw Layer
                    for (int n = 0; n <= 8; n += 8)
                    {
                        if ((layer == null && layersCheckList.GetItemChecked(i + n)) || (i + n) == layer)
                        {
                            renderer.Draw(layerImage, _level.Rooms[_currentRoomIndex].Layers[i + n]);
                        }
                    }

                    // Draw Overlay
                    if (displayOverlayToolStripMenuItem.Checked && i == 0 && overlayBitmap != null)
                    {
                        using var overlayImage = (MemoryImage<BGRA32>)overlayBitmap.AsAuroraImage();
                        DrawOverlayOnLayer(layerImage, overlayImage);
                    }
                }

                if ((tabControl.SelectedIndex == (int)TabControlIndex.Tile || tabControl.SelectedIndex == (int)TabControlIndex.Stamp) && GetHighestActiveLayerIndex() != null)
                {
                    DrawTileInfosOnLayer(layerImage, _level.Rooms[_currentRoomIndex].Layers[GetHighestActiveLayerIndex().Value % 8].Tiles);
                }
            }

            DrawActors();
        }

        private void DrawTileInfosOnLayer(IImage<BGRA32> target, ReadOnlySpan<ushort> tiles)
        {
            for (int y = 0; y < Layer.DIMENSION; y++)
            {
                for (int x = 0; x < Layer.DIMENSION; x++)
                {
                    Point pos = new Point(x * TILE_DIMENSION_IN_PIXELS + 8, y * TILE_DIMENSION_IN_PIXELS + 8);
                    ushort tile = tiles[y * Layer.DIMENSION + x];

                    switch (tile)
                    {
                        case 12: // Abyss
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 791, 1);
                            break;
                        case 13: // Abyss, does damage
                        case 84: // Abyss
                        case 222: // Abyss
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 791, 1, 13);
                            break;
                        case 33: // Block, movable
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 426, 1);
                            break;
                        case 38: // Block, movable north
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 83, 1);
                            break;
                        case 39: // Block, movable south
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 82, 1);
                            break;
                        case 54: // Block, movable west
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 81, 1);
                            break;
                        case 55: // Block, movable east
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 80, 1);
                            break;
                        case 105: // Bush, reveals hole
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 410, 1);
                            break;
                        case 67: // Pot, reveals star switch
                        case 70: // Pot (Side view), reveals switch
                        case 72: // Pot (Side view), reveals star switch
                        case 104: // Bush, reveals switch
                        case 106: // Bush, reveals star switch
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 426, 1);
                            break;
                        case 66: // Pot, drops a heart
                        case 71: // Pot (Side view), drops a heart
                        case 117: // Bush, drops a heart
                            spriteRendererTV.DrawSprite(target, pos.X, pos.Y, 44, 1);
                            break;
                        case 68: // Pot, drops two heart
                        case 73: // Pot (Side view), drops two heart
                        case 107: // Bush, drops two heart
                            spriteRendererTV.DrawSprite(target, pos.X - 4, pos.Y, 44, 1);
                            spriteRendererTV.DrawSprite(target, pos.X + 4, pos.Y, 44, 1);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void DrawOverlayOnLayer(MemoryImage<BGRA32> layerImage, MemoryImage<BGRA32> overlayImage)
        {
            for (int x = 0; x < layerImage.Width; x += overlayImage.Width)
            {
                for (int y = 0; y < roomLayerBitmap.Height - 128; y += overlayBitmap.Height)
                {
                    layerImage.CopyFrom(overlayImage, new Point(x, y), BlendModes.Overlay, 1);
                }
            }
        }

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
            switch (tabControl.SelectedIndex)
            {
                case (int)TabControlIndex.Tile: // Tile tab
                case (int)TabControlIndex.Stamp:
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            DoTileAction(scaledEvent);
                            break;
                        case MouseButtons.Right:
                            UpdateClipboardSelection();
                            TileStampFlowLayoutPanel.SelectedIndex = -1;
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
                                    using (var layerImage = (MemoryImage<BGRA32>)roomLayerBitmap.AsAuroraImage())
                                    {
                                        TilesetRenderer.Draw(layerImage, _tileBrush.Clipboard, position.Location);
                                    }

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
                case (int)TabControlIndex.Actor:
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
                if (actorMouseDownOnIndex == -1 || e.Button != MouseButtons.Left)
                {
                    return;
                }

                int newActorXCoord = scaledEvent.X / ACTOR_PIXELS_PER_COORDINATE;
                int newActorYCoord = scaledEvent.Y / ACTOR_PIXELS_PER_COORDINATE;

                if (newActorXCoord < 0) newActorXCoord = 0;
                if (newActorYCoord < 0) newActorYCoord = 0;
                if (newActorXCoord > ActorXCoordInput.Maximum) newActorXCoord = (int)ActorXCoordInput.Maximum;
                if (newActorYCoord > ActorYCoordInput.Maximum) newActorYCoord = (int)ActorYCoordInput.Maximum;

                var actor = _level.Rooms[_currentRoomIndex].Actors[actorMouseDownOnIndex];
                actor.XCoord = (byte)newActorXCoord;
                actor.YCoord = (byte)newActorYCoord;

                UpdateActor(actorMouseDownOnIndex, actor);
                actorMouseDownOnIndex = actorsCheckListBox.SelectedIndex;
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

        private void layersPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventArgs scaledEvent = ScaleEventToLayerRealSize(e);

            switch (tabControl.SelectedIndex)
            {
                case (int)TabControlIndex.Tile:
                case (int)TabControlIndex.Stamp:
                    DoTileAction(scaledEvent);
                    break;
                case (int)TabControlIndex.Actor:
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

                actorMouseDownOnIndex = -1;
                for (int i = 0; i < actorsCheckListBox.Items.Count; i++)
                {
                    if (actorsCheckListBox.GetItemChecked(i) == true)
                    {
                        var actor = _level.Rooms[_currentRoomIndex].Actors[i];
                        bool isVisible = GetHighestActiveLayerIndex() % 8 == actor.Layer || IsSelectedActor(actor);
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
            if (tabControl.SelectedIndex == (int)TabControlIndex.Actor)
            {
                actorMouseDownOnIndex = -1;
            }
            else if (tabControl.SelectedIndex == (int)TabControlIndex.Tile || tabControl.SelectedIndex == (int)TabControlIndex.Stamp)
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
                            _tileBrush.Copy(_tileSelection, _level.Rooms[_currentRoomIndex].Layers[layer.Value]);
                            UpdateBrushTileBitmap();
                        }
                        break;
                    default:
                        break;
                }
            }
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

            if (tabControl.SelectedIndex != (int)TabControlIndex.Tile && tabControl.SelectedIndex != (int)TabControlIndex.Stamp)
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
                        int lastLayer = layerPosition.Value;

                        do
                        {
                            layerPosition++;
                            if (layerPosition == 8)
                                return;
                        } while (layersCheckList.GetItemColor(layerPosition.Value) == Color.Gray);

                        if (lastLayer > 0)
                        {
                            layersCheckList.SetItemChecked(lastLayer, false);
                            layersCheckList.SetItemChecked(lastLayer + 8, false);
                        }

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
                        if (layerPosition > 8)
                        {
                            layersCheckList.SetItemChecked(layerPosition.Value, false);
                            layersCheckList.SetItemChecked(layerPosition.Value % 8, false);
                        }
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
                    _tileBrush.AutomaticSetTileActors = automaticSetTileActorsToolStripMenuItem.Checked;
                    if (_tileBrush.Draw(_level.Rooms[_currentRoomIndex], layer.Value, eventX, eventY))
                    {
                        if (actorsCheckListBox.Items.Count != _level.Rooms[_currentRoomIndex].Actors.Count)
                        {
                            BuildLayerActorList();
                            actorLayerComboBox_SelectionChangeCommitted(_tileBrush, null);
                        }
                        UpdateView(layer);
                        _levelIsDirty = true;
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
            Color color = _level.Rooms[_currentRoomIndex].Layers[layer].IsEmpty ? Color.Gray : Color.Black;
            layersCheckList.SetItemColor(layer, color);
            layersCheckList.Refresh();
        }

        private void LayersCheckList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Need to delay redraw because right now the newly checked layer won't have checked=true
            if (IsHandleCreated)
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    UpdateView();
                    UpdateTileSheetPictureBox();
                }));

            layersCheckList.SelectedIndex = -1;
        }

        private void ChangeTileSheet()
        {
            var properties = _level.Map.GetRoomProperties(_currentRoomIndex);
            int tileSheetIndex = properties.TileSheetId;
            tilesetRendererTV.LoadTileset(dataRarc, tileSheetIndex);
            tilesetRendererGBA.LoadTileset(dataRarc, tileSheetIndex, true);
            spriteRendererTV.LoadTilesheet(dataRarc, properties.NPCSheetID, false);
            spriteRendererGBA.LoadTilesheet(dataRarc, properties.NPCSheetID, true);
            {
                using var tileSheet = (MemoryImage<BGRA32>)tileSheetBitmap.AsAuroraImage();
                tileSheet.Clear();
                tilesetRendererTV.Draw(tileSheet);

                using var tileSheetGBA = (MemoryImage<BGRA32>)tileSheetBitmapGBA.AsAuroraImage();
                tileSheetGBA.Clear();
                tilesetRendererGBA.Draw(tileSheetGBA);
            }

            UpdateView();
        }

        private void UpdateTileSheetPictureBox()
            => tileSheetPictureBox.Image = GetHighestActiveLayerIndex() % 8 == 0 ? tileSheetBitmap : tileSheetBitmapGBA;

        private void ChangeOverlay()
        {
            var properties = _level.Map.GetRoomProperties(_currentRoomIndex);
            var tileSheetPath = Path.Combine(dataDirectory, $"Overlays\\filter{properties.OverlayTextureId}.png");

            if (File.Exists(tileSheetPath))
            {
                overlayBitmap?.Dispose();
                overlayBitmap = new Bitmap(tileSheetPath);
            }
            UpdateView();
        }

        private void MirrorBrushbutton_Click(object sender, EventArgs e)
        {
            _tileBrush.Clipboard.Mirror();
            UpdateBrushTileBitmap();
        }

        private void TileStampFlowLayoutPanel_SelectionChanged(object sender, EventArgs e)
        {
            if (TileStampFlowLayoutPanel.SelectedIndex != -1)
            {
                TileStampFlowLayoutPanel.Load(_tileBrush.Clipboard, TileStampFlowLayoutPanel.SelectedIndex);
                UpdateBrushTileBitmap();
                DeleteStampButton.Enabled = true;
            }
            else
            {
                DeleteStampButton.Enabled = false;
            }
        }

        private void DeleteStampButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = TileStampFlowLayoutPanel.SelectedIndex;
            if (selectedIndex != -1)
            {
                string stampName = TileStampFlowLayoutPanel.Controls[selectedIndex].Name;
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the selected stamp \"{stampName}\"?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    TileStampFlowLayoutPanel.Delete(selectedIndex);
                }
            }
        }


        private void mirrorRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _level.Rooms[_currentRoomIndex].Mirror();
            BuildLayerActorList();
            if (autoSelectToolStripMenuItem.Checked)
            {
                SelectAllLayerActors();
            }
            UpdateView();
        }

        private void SaveStampButton_Click(object sender, EventArgs e)
        {
            string? name = InputDialog.Show("Create a new Tile Stamp", "Please enter a name for the stamp:");
            if (name != null)
            {
                string path = Path.Combine(dataDirectory, "Stamps", name + ".bin");
                if (File.Exists(path))
                    throw new Exception();

                using (FileStream stampData = new FileStream(path, FileMode.CreateNew))
                {
                    _tileBrush.Clipboard.WriteToStream(stampData);
                }
                string icon = Path.ChangeExtension(path, ".png");
                using Bitmap iconData = new Bitmap(_tileBrush.Width * 16, _tileBrush.Height * 16);

                using (var iconmage = (MemoryImage<BGRA32>)iconData.AsAuroraImage())
                {
                    tilesetRendererTV.Draw(iconmage, _tileBrush.Clipboard);
                }

                iconData.Save(icon, ImageFormat.Png);
                TileStampFlowLayoutPanel.Add(path);
            }
        }
        #endregion Layers

        #region Actors
        private void DrawActor(Actor actor)
        {
            int? currentLayer = GetHighestActiveLayerIndex() % 8;
            bool isOnCurrentLayer = currentLayer == actor.Layer;
            Point actorPixelPosition = new Point(actor.XCoord * ACTOR_PIXELS_PER_COORDINATE, actor.YCoord * ACTOR_PIXELS_PER_COORDINATE);
            Assets.Actors.TryGetValue(actor.ID, out ActorDefinition actorDefinition);

            // Draw base actor graphics
            if (isOnCurrentLayer)
            {
                using var actorLayer = (MemoryImage<BGRA32>)actorLayerBitmap.AsAuroraImage();
                DrawActorToImage(actor, actorPixelPosition, actorDefinition, actorLayer);
            }

            // When we're in aktor tap, we want to display more information
            if (tabControl.SelectedIndex != (int)TabControlIndex.Actor)
                return;

            bool isSelectedActor = IsSelectedActor(actor);

            // Teleport Information
            switch (actor.Name)
            {
                case "WARP":
                case "MJGR":
                case "CIRC":
                    int layer = actor.VariableByte4 & 0x7;
                    if (actor.Name == "CIRC" && layer != 0)
                        layer = 1;

                    bool isZero = actor.VariableByte3 == 0 && actor.VariableByte2 == 0;
                    int x = isZero ? actorPixelPosition.X : actor.VariableByte3 * ACTOR_PIXELS_PER_COORDINATE;
                    int y = isZero ? actorPixelPosition.Y : actor.VariableByte2 * ACTOR_PIXELS_PER_COORDINATE;
                    if (currentLayer == layer)
                    {
                        Color colorX = isSelectedActor ? Color.Red : Color.White;
                        actorLayerGraphics.DrawRectangleWithDropShadow(colorX,
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
            if (isOnCurrentLayer)
            {
                int width, height;
                // debug display
                switch (actor.Name)
                {
                    case "PNPC":
                        if (isOnCurrentLayer)
                        {
                            var renderer = (actor.Layer == 0 || actor.Layer == 8) ? tilesetRendererTV : tilesetRendererGBA;
                            using var iconmage = (MemoryImage<BGRA32>)roomLayerBitmap.AsAuroraImage();
                            ushort tile = (ushort)((actor.VariableByte2 & 0x3) << 8 | actor.VariableByte1);
                            renderer.DrawTile(iconmage, actor.XCoord / 2 * TILE_DIMENSION_IN_PIXELS, actor.YCoord / 2 * TILE_DIMENSION_IN_PIXELS, tile);
                        }
                        break;
                    case "PNP2":
                        if (isOnCurrentLayer)
                        {
                            var renderer = (actor.Layer == 0 || actor.Layer == 8) ? tilesetRendererTV : tilesetRendererGBA;
                            using var iconmage = (MemoryImage<BGRA32>)roomLayerBitmap.AsAuroraImage();
                            ushort tileTarget = (ushort)(actor.Variable & 0xFFF);
                            ushort tile = (ushort)(actor.Variable >> 12 & 0xFFF);
                            var layerTiles = _level.Rooms[_currentRoomIndex].Layers[actor.Layer].Tiles;
                            for (int y = 0; y < Layer.DIMENSION; y++)
                            {
                                for (int x = 0; x < Layer.DIMENSION; x++)
                                {
                                    if (layerTiles[x + (y * Layer.DIMENSION)] == tileTarget)
                                    {
                                        renderer.DrawTile(iconmage, x * TILE_DIMENSION_IN_PIXELS, y * TILE_DIMENSION_IN_PIXELS, tile);
                                    }
                                }
                            }
                        }
                        break;
                    case "PTMI":
                        actorLayerGraphics.DrawRectangleWithDropShadow(Color.Black,
                            actorPixelPosition.X,
                            actorPixelPosition.Y,
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.VariableByte1),
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.VariableByte2));
                        break;
                    case "TOMI":
                        actorLayerGraphics.DrawRectangleWithDropShadow(Color.LightPink,
                            actorPixelPosition.X,
                            actorPixelPosition.Y,
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.VariableByte1),
                            ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + actor.VariableByte2));
                        break;
                    case "BLDF":
                        width = ACTOR_PIXELS_PER_COORDINATE * (1 + actor.VariableByte2);
                        height = ACTOR_PIXELS_PER_COORDINATE * (1 + actor.VariableByte3);
                        actorLayerGraphics.DrawRectangleWithDropShadow(Color.Black,
                            actorPixelPosition.X - (width / 2),
                            actorPixelPosition.Y - (height / 2),
                            width,
                            height);

                        if ((actor.VariableByte4 & 7) < 4)
                        {
                            actorLayerGraphics.FillRectangle(GraphicsEX.DropShadow,
                            actorPixelPosition.X - (width / 2),
                            actorPixelPosition.Y - (height / 2),
                            width,
                            height);
                        }
                        break;

                    case "SWTH":
                        var tYellow = Color.FromArgb(96, Color.Yellow);
                        if (isOnCurrentLayer)
                        {
                            switch (actor.VariableByte1)
                            {
                                case 7:
                                    width = ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + (actor.VariableByte3 >> 4));
                                    height = ACTOR_PIXELS_PER_COORDINATE * 2 * (1 + (actor.VariableByte3 & 0xF));
                                    actorLayerGraphics.DrawRectangleWithDropShadow(tYellow,
                                        actorPixelPosition.X - (width / 2),
                                        actorPixelPosition.Y - (height / 2),
                                        width,
                                        height);
                                    break;
                                default:
                                    actorLayerGraphics.DrawRectangleWithDropShadow(tYellow, actorPixelPosition.X, actorPixelPosition.Y, 16, 16);
                                    break;
                            }
                        }
                        break;
                    case "OBLF":
                        GraphicsEX.Direction d = (GraphicsEX.Direction)(actor.VariableByte2 & 0x3);
                        actorLayerGraphics.DrawLineWithDropShadow(Color.Aquamarine, actorPixelPosition, actorPixelPosition.MovePointInDirection(d, 64));
                        break;
                    case "LOTS":
                        width = ((actor.VariableByte2 >> 4) + (actor.VariableByte3 & 0xF) * 16) * ACTOR_PIXELS_PER_COORDINATE * 2;
                        if (width != 0)
                        {
                            GraphicsEX.Direction direction = (GraphicsEX.Direction)(actor.VariableByte2 & 0xF);
                            Point targetPosition = actorPixelPosition.MovePointInDirection(direction, width);
                            actorLayerGraphics.DrawLineWithDropShadow(Color.Aquamarine, actorPixelPosition, targetPosition);
                        }
                        break;
                    case "RUSA":
                        width = (actor.VariableByte1 & 0xF) switch
                        {
                            0 => 2 * ACTOR_PIXELS_PER_COORDINATE,
                            1 => 4 * ACTOR_PIXELS_PER_COORDINATE,
                            _ => 8 * ACTOR_PIXELS_PER_COORDINATE,
                        };
                        actorLayerGraphics.DrawCircleWithDropShadow(Color.Honeydew, actorPixelPosition, width);
                        break;
                    case "BMST":
                        if (actor.VariableByte1 != 0)
                        {
                            Color colorX = isSelectedActor ? Color.LightPink : Color.DarkRed;
                            width = Math.Min(actor.VariableByte1, (byte)15) * ACTOR_PIXELS_PER_COORDINATE * 2 - ACTOR_PIXELS_PER_COORDINATE;
                            actorLayerGraphics.DrawCircleWithDropShadow(colorX, actorPixelPosition, width);
                        }
                        break;
                    case "TKRA":
                        int i = actor.VariableByte1;
                        int sob = 1;

                        if (i <= 7)
                            i = 1 + i * 4;
                        else if (i <= 15)
                            break;
                        else if (i <= 31)
                            i = 44 + (i - 15);
                        else if (i <= 47)
                            break;
                        else if (i <= 63)
                            i = 387 + (i - 48);
                        else
                            i = 525 + (i - 64);

                        using (var actorLayer = (MemoryImage<BGRA32>)actorLayerBitmap.AsAuroraImage())
                            spriteRendererTV.DrawSprite(actorLayer, actorPixelPosition.X + 8, actorPixelPosition.Y - 8, (ushort)i, (ushort)sob);
                        break;
                    default:
                        break;
                }

                Bitmap actorSprite = GetActorInfoBitmap(actor);
                if (actorSprite != null)
                {
                    actorLayerGraphics.DrawImage(actorSprite,
                        actorPixelPosition.X - (actorSprite.Width / 2),
                        actorPixelPosition.Y - (actorSprite.Height / 2),
                        actorSprite.Width,
                        actorSprite.Height);
                }

                // display variables
                if (displayVariablesActorsToolStripMenuItem.Checked)
                {
                    int localVariable = actor.VariableByte4 >> 3;
                    if (localVariable != 0)
                    {
                        actorLayerGraphics.DrawString(Color.Yellow, localVariable.ToString(), actorPixelPosition.X + 2, actorPixelPosition.Y + 2);
                        if (V6ACTORS.Contains(actor.Name))
                        {
                            int o = 1;
                            int v2 = (actor.VariableByte4 & 0x7) << 2 | actor.VariableByte3 >> 6;
                            int v3 = (actor.VariableByte3 >> 1) & 0x1F;
                            int v4 = (actor.VariableByte3 & 0x1) << 4 | actor.VariableByte2 >> 4;
                            int v5 = (actor.VariableByte2 & 0xF) << 1 | actor.VariableByte1 >> 7;
                            if (v2 != 0)
                                actorLayerGraphics.DrawString(Color.GreenYellow, v2.ToString(), actorPixelPosition.X + 2, actorPixelPosition.Y + 2 + o++ * 8);
                            if (v3 != 0)
                                actorLayerGraphics.DrawString(Color.GreenYellow, v3.ToString(), actorPixelPosition.X + 2, actorPixelPosition.Y + 2 + o++ * 8);
                            if (v4 != 0)
                                actorLayerGraphics.DrawString(Color.GreenYellow, v4.ToString(), actorPixelPosition.X + 2, actorPixelPosition.Y + 2 + o++ * 8);
                            if (v5 != 0)
                                actorLayerGraphics.DrawString(Color.GreenYellow, v5.ToString(), actorPixelPosition.X + 2, actorPixelPosition.Y + 2 + o++ * 8);
                        }
                    }

                }

                // Highlights Actor
                if (isSelectedActor)
                {
                    actorLayerGraphics.DrawRectangleWithDropShadow(Color.Red, actorPixelPosition.X, actorPixelPosition.Y, 5, 5);
                }
                else
                {
                    Color color = actorDefinition?.Category switch
                    {
                        ActorCategoryType.Object => Color.White,
                        ActorCategoryType.TileObject => Color.LightGray,
                        ActorCategoryType.Item => Color.Cyan,
                        ActorCategoryType.NPC => Color.LimeGreen,
                        ActorCategoryType.Enemy => Color.LightCoral,
                        ActorCategoryType.Boss => Color.DarkGreen,
                        ActorCategoryType.Property => Color.Tan,
                        ActorCategoryType.Cutscene => Color.Gold,
                        ActorCategoryType.Intern => Color.Black,
                        null => Color.Magenta,
                        _ => Color.Magenta,
                    };
                    actorLayerGraphics.DrawRectangleWithDropShadow(color, actorPixelPosition.X, actorPixelPosition.Y, 3, 3);
                }

            }
        }
        private void DrawActorToImage(Actor actor, Point actorPixelPosition, MemoryImage<BGRA32> image)
        {
            if (Assets.Actors.TryGetValue(actor.ID, out ActorDefinition actorDefinition))
                DrawActorToImage(actor, actorPixelPosition, actorDefinition, image);
        }
        private void DrawActorToImage(Actor actor, Point actorPixelPosition, ActorDefinition actorDefinition, MemoryImage<BGRA32> image)
        {
            if (actorDefinition?.Rendering != null)
            {
                var rendering = actorDefinition.Rendering;
                int variantKey = (int)(actor.Variable & rendering.BitMask);
                if (rendering.Variants.TryGetValue(variantKey, out var renderList))
                {
                    var renderer = actor.Layer == 0 ? spriteRendererTV : spriteRendererGBA;
                    foreach (var renderInfo in renderList)
                    {
                        if (renderInfo.SpriteIndex != -1) // render sprite
                        {
                            renderer.DrawSprite(image, actorPixelPosition.X + renderInfo.XOffset, actorPixelPosition.Y + renderInfo.YOffset, (ushort)renderInfo.SpriteIndex, renderInfo.SpriteListIndex, renderInfo.ReplacementPaletteIndex, renderInfo.TargetPaletteIndex);
                        }
                        else // render bti
                        {
                            if (dataRarc.Root.Directorys[Rarc.CommonFolderTypes.Timg].TryGetFile(renderInfo.BtiFile, out var btiFile) || (_level.Resources.Directorys.TryGetValue(Rarc.CommonFolderTypes.Timg, out var mapFolder) && mapFolder.TryGetFile(renderInfo.BtiFile, out btiFile)))
                            {
                                // bti file found
                                // TODO!!
                                _logger.AppendLine($"{actor.ID}: \"{btiFile.Name}\" cannot be displayed at this time!");
                            }
                            else
                            {
                                // bti file not found
                                _logger.AppendLine($"{actor.ID}: \"{renderInfo.BtiFile}\" not found!");
                            }
                        }
                    }
                }
            }
        }

        private SpriteConverterForm? _spriteConverter;
        private void openSpriteConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_spriteConverter == null || _spriteConverter.IsDisposed)
            {
                _spriteConverter = new SpriteConverterForm(dataRarc);
                _spriteConverter.Show(this);
            }
            else
            {
                _spriteConverter.BringToFront();
                _spriteConverter.Focus();
            }
        }

        private Bitmap? GetActorInfoBitmap(Actor actor)
        {
            string type;
            switch (actor.Name)
            {
                case "JIJI":
                    type = $"{actor.VariableByte3 & 0x7F}";
                    break;
                case "DOOR":
                    int doorType = actor.VariableByte1 & 0x7F;
                    if (doorType == 5 || doorType == 7)
                        goto default;
                    else
                        type = $"{(actor.VariableByte2 & 0xf) << 1 | actor.VariableByte1 >> 7}_{actor.VariableByte2 >> 4}";
                    break;
                case "RBPN":
                    type = $"{(actor.VariableByte2 & 0xf) << 1 | actor.VariableByte1 >> 7}";
                    break;
                case "STBL":
                    type = $"{actor.VariableByte2 & 0x7F}";
                    break;
                default:
                    type = $"{actor.VariableByte1 & 0x7F}";
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
                return null;
            }
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
        #endregion

        #endregion

        private enum TabControlIndex : int
        {
            Map = 0,
            Tile = 1,
            Stamp = 2,
            Actor
        }
    }
}