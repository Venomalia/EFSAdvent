using EFSAdvent.Controls;
using System.Windows.Forms;

namespace EFSAdvent
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tileSheetPanel = new System.Windows.Forms.Panel();
            this.tileSheetPictureBox = new System.Windows.Forms.PictureBox();
            this.layersPanel = new System.Windows.Forms.Panel();
            this.layerPictureBox = new EFSAdvent.Controls.PictureBoxWithInterpolationMode();
            this.actorContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggerTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelAsArcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.roomAstsxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.viewAspngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapAspngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapAndAAspngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allRoomsAspngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allRoomsAndActorsAspngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportRoomFromTmx = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.roomImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actorsImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.automaticSetTileActorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xSizeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.displayOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureFilterModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bilinearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bicubicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nearestNeighborToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysShowActorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightSideGroupBox = new System.Windows.Forms.GroupBox();
            this.CoridinatesTextBox = new System.Windows.Forms.TextBox();
            this.updateLayersButton = new System.Windows.Forms.Button();
            this.buttonSaveLayers = new System.Windows.Forms.Button();
            this.layersCheckList = new EFSAdvent.Controls.CheckedListBoxColorable();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.MapTabControl = new System.Windows.Forms.TabControl();
            this.MapMultiplayerTabPage = new System.Windows.Forms.TabPage();
            this.MapEditor = new EFSAdvent.Controls.MapEditor();
            this.MapSinglelplayerTabPage = new System.Windows.Forms.TabPage();
            this.MapEditorSinglelplayer = new EFSAdvent.Controls.MapEditor();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.MirrorBrushbutton = new System.Windows.Forms.Button();
            this.BrushSizeLabel = new System.Windows.Forms.Label();
            this.BrushSizeComboBox = new System.Windows.Forms.ComboBox();
            this.BrushTileLabel = new System.Windows.Forms.Label();
            this.BrushTilePictureBox = new System.Windows.Forms.PictureBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.SaveStampButton = new System.Windows.Forms.Button();
            this.MirrorStampButton = new System.Windows.Forms.Button();
            this.DeleteStampButton = new System.Windows.Forms.Button();
            this.TileStampFlowLayoutPanel = new EFSAdvent.Controls.TileStampFlowLayoutPanel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonActorsSelectNone = new System.Windows.Forms.Button();
            this.actorLayerComboBox = new System.Windows.Forms.ComboBox();
            this.actorAttributesgroupBox = new System.Windows.Forms.GroupBox();
            this.ActorInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.tabPageRawVariable = new System.Windows.Forms.TabControl();
            this.Variables5TabPage = new System.Windows.Forms.TabPage();
            this.tabControlRawVarType = new System.Windows.Forms.TabControl();
            this.tabPageV5 = new System.Windows.Forms.TabPage();
            this.label_V5_3 = new System.Windows.Forms.Label();
            this.label_V5_3a = new System.Windows.Forms.Label();
            this.ActorVariable4AInput = new System.Windows.Forms.NumericUpDown();
            this.ActorVariable1Input = new System.Windows.Forms.NumericUpDown();
            this.label_V5_4 = new System.Windows.Forms.Label();
            this.ActorVariable2Input = new System.Windows.Forms.NumericUpDown();
            this.label_V5_1 = new System.Windows.Forms.Label();
            this.ActorVariable4BInput = new System.Windows.Forms.NumericUpDown();
            this.ActorVariable3Input = new System.Windows.Forms.NumericUpDown();
            this.label_V5_2 = new System.Windows.Forms.Label();
            this.tabPageV6 = new System.Windows.Forms.TabPage();
            this.label21 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.ActorV6Variable2fInput = new System.Windows.Forms.NumericUpDown();
            this.ActorV6Variable1Input = new System.Windows.Forms.NumericUpDown();
            this.ActorV6Variable4Input = new System.Windows.Forms.NumericUpDown();
            this.ActorV6Variable3Input = new System.Windows.Forms.NumericUpDown();
            this.ActorV6Variable6Input = new System.Windows.Forms.NumericUpDown();
            this.ActorV6Variable5Input = new System.Windows.Forms.NumericUpDown();
            this.groupBoxFullVariable = new System.Windows.Forms.GroupBox();
            this.ActorVariableFullInput = new System.Windows.Forms.NumericUpDown();
            this.tabPageFieldsVariable = new System.Windows.Forms.TabPage();
            this.panelActorFields = new System.Windows.Forms.FlowLayoutPanel();
            this.cloneButton = new System.Windows.Forms.Button();
            this.actorDeleteButton = new System.Windows.Forms.Button();
            this.groupBoxCoridinate = new System.Windows.Forms.GroupBox();
            this.ActorXCoordInput = new System.Windows.Forms.NumericUpDown();
            this.ActorYCoordInput = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ActorLayerInput = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.ActorNameComboBox = new System.Windows.Forms.ComboBox();
            this.actorsCheckListBox = new System.Windows.Forms.CheckedListBox();
            this.ActorAttributesTip = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.BottomGroupBox = new System.Windows.Forms.GroupBox();
            this.RootFolderPathTextBox = new System.Windows.Forms.TextBox();
            this.tileSheetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileSheetPictureBox)).BeginInit();
            this.layersPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layerPictureBox)).BeginInit();
            this.actorContextMenuStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.rightSideGroupBox.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.MapTabControl.SuspendLayout();
            this.MapMultiplayerTabPage.SuspendLayout();
            this.MapSinglelplayerTabPage.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrushTilePictureBox)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.actorAttributesgroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorInfoPictureBox)).BeginInit();
            this.tabPageRawVariable.SuspendLayout();
            this.Variables5TabPage.SuspendLayout();
            this.tabControlRawVarType.SuspendLayout();
            this.tabPageV5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable4AInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable1Input)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable2Input)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable4BInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable3Input)).BeginInit();
            this.tabPageV6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable2fInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable1Input)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable4Input)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable3Input)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable6Input)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable5Input)).BeginInit();
            this.groupBoxFullVariable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariableFullInput)).BeginInit();
            this.tabPageFieldsVariable.SuspendLayout();
            this.groupBoxCoridinate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorXCoordInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorYCoordInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorLayerInput)).BeginInit();
            this.BottomGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tileSheetPanel
            // 
            this.tileSheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.tileSheetPanel.AutoScroll = true;
            this.tileSheetPanel.BackColor = System.Drawing.Color.Transparent;
            this.tileSheetPanel.Controls.Add(this.tileSheetPictureBox);
            this.tileSheetPanel.Location = new System.Drawing.Point(0, 40);
            this.tileSheetPanel.Name = "tileSheetPanel";
            this.tileSheetPanel.Size = new System.Drawing.Size(273, 483);
            this.tileSheetPanel.TabIndex = 0;
            // 
            // tileSheetPictureBox
            // 
            this.tileSheetPictureBox.Location = new System.Drawing.Point(0, 0);
            this.tileSheetPictureBox.Name = "tileSheetPictureBox";
            this.tileSheetPictureBox.Size = new System.Drawing.Size(100, 50);
            this.tileSheetPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tileSheetPictureBox.TabIndex = 0;
            this.tileSheetPictureBox.TabStop = false;
            this.tileSheetPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tilePictureBox_MouseClick);
            // 
            // layersPanel
            // 
            this.layersPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.layersPanel.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.layersPanel.Controls.Add(this.layerPictureBox);
            this.layersPanel.Location = new System.Drawing.Point(283, 27);
            this.layersPanel.MinimumSize = new System.Drawing.Size(512, 512);
            this.layersPanel.Name = "layersPanel";
            this.layersPanel.Size = new System.Drawing.Size(512, 516);
            this.layersPanel.TabIndex = 1;
            // 
            // layerPictureBox
            // 
            this.layerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.layerPictureBox.Enabled = false;
            this.layerPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.layerPictureBox.Location = new System.Drawing.Point(0, -1);
            this.layerPictureBox.MinimumSize = new System.Drawing.Size(512, 512);
            this.layerPictureBox.Name = "layerPictureBox";
            this.layerPictureBox.Size = new System.Drawing.Size(512, 512);
            this.layerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.layerPictureBox.TabIndex = 0;
            this.layerPictureBox.TabStop = false;
            this.layerPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.layersPictureBox_MouseDown);
            this.layerPictureBox.MouseLeave += new System.EventHandler(this.layerPictureBox_MouseLeave);
            this.layerPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.layersPictureBox_MouseMove);
            this.layerPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.layersPictureBox_MouseUp);
            this.layerPictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.layerPictureBox_MouseWheel);
            // 
            // actorContextMenuStrip
            // 
            this.actorContextMenuStrip.DropShadowEnabled = false;
            this.actorContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pastToolStripMenuItem});
            this.actorContextMenuStrip.Name = "actorContextMenuStrip";
            this.actorContextMenuStrip.Size = new System.Drawing.Size(131, 26);
            this.actorContextMenuStrip.Paint += new System.Windows.Forms.PaintEventHandler(this.actorContextMenuStrip_Paint);
            // 
            // pastToolStripMenuItem
            // 
            this.pastToolStripMenuItem.Name = "pastToolStripMenuItem";
            this.pastToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.pastToolStripMenuItem.Text = "Paste Here";
            this.pastToolStripMenuItem.Click += new System.EventHandler(this.pastToolStripMenuItem_Click);
            // 
            // loggerTextBox
            // 
            this.loggerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.loggerTextBox.Location = new System.Drawing.Point(7, 207);
            this.loggerTextBox.Multiline = true;
            this.loggerTextBox.Name = "loggerTextBox";
            this.loggerTextBox.ReadOnly = true;
            this.loggerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.loggerTextBox.Size = new System.Drawing.Size(150, 312);
            this.loggerTextBox.TabIndex = 22;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(958, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.SaveMenuItem,
            this.SaveAsMenuItem,
            this.toolStripSeparator2,
            this.ExportMenuItem,
            this.importToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.newToolStripMenuItem.Text = "New File";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.openToolStripMenuItem.Text = "Open..";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenLevel);
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Enabled = false;
            this.SaveMenuItem.Name = "SaveMenuItem";
            this.SaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveMenuItem.Size = new System.Drawing.Size(249, 22);
            this.SaveMenuItem.Text = "Save changes";
            this.SaveMenuItem.Click += new System.EventHandler(this.saveChangesToolStripMenuItem_Click);
            // 
            // SaveAsMenuItem
            // 
            this.SaveAsMenuItem.Enabled = false;
            this.SaveAsMenuItem.Name = "SaveAsMenuItem";
            this.SaveAsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.SaveAsMenuItem.Size = new System.Drawing.Size(249, 22);
            this.SaveAsMenuItem.Text = "Save as ...";
            this.SaveAsMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(246, 6);
            // 
            // ExportMenuItem
            // 
            this.ExportMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.levelAsArcToolStripMenuItem,
            this.toolStripSeparator4,
            this.roomAstsxToolStripMenuItem,
            this.toolStripSeparator3,
            this.viewAspngToolStripMenuItem,
            this.mapAspngToolStripMenuItem,
            this.mapAndAAspngToolStripMenuItem,
            this.allRoomsAspngToolStripMenuItem,
            this.allRoomsAndActorsAspngToolStripMenuItem});
            this.ExportMenuItem.Enabled = false;
            this.ExportMenuItem.Name = "ExportMenuItem";
            this.ExportMenuItem.Size = new System.Drawing.Size(249, 22);
            this.ExportMenuItem.Text = "Export";
            // 
            // levelAsArcToolStripMenuItem
            // 
            this.levelAsArcToolStripMenuItem.Name = "levelAsArcToolStripMenuItem";
            this.levelAsArcToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.levelAsArcToolStripMenuItem.Text = "Level as .arc";
            this.levelAsArcToolStripMenuItem.ToolTipText = "Export as FSA Level Archive.";
            this.levelAsArcToolStripMenuItem.Click += new System.EventHandler(this.ExportLevel);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(226, 6);
            // 
            // roomAstsxToolStripMenuItem
            // 
            this.roomAstsxToolStripMenuItem.Name = "roomAstsxToolStripMenuItem";
            this.roomAstsxToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.roomAstsxToolStripMenuItem.Text = "Room as .tmx";
            this.roomAstsxToolStripMenuItem.ToolTipText = "Export as Tiled map files.";
            this.roomAstsxToolStripMenuItem.Click += new System.EventHandler(this.ExportRoomAsTmx_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(226, 6);
            // 
            // viewAspngToolStripMenuItem
            // 
            this.viewAspngToolStripMenuItem.Name = "viewAspngToolStripMenuItem";
            this.viewAspngToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.viewAspngToolStripMenuItem.Text = "View as .png";
            this.viewAspngToolStripMenuItem.Click += new System.EventHandler(this.ExportViewAsPng);
            // 
            // mapAspngToolStripMenuItem
            // 
            this.mapAspngToolStripMenuItem.Name = "mapAspngToolStripMenuItem";
            this.mapAspngToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mapAspngToolStripMenuItem.Text = "Map as .png";
            this.mapAspngToolStripMenuItem.Click += new System.EventHandler(this.ExportLevelAsPng);
            // 
            // mapAndAAspngToolStripMenuItem
            // 
            this.mapAndAAspngToolStripMenuItem.Name = "mapAndAAspngToolStripMenuItem";
            this.mapAndAAspngToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mapAndAAspngToolStripMenuItem.Text = "Map and Actors as .png ";
            this.mapAndAAspngToolStripMenuItem.Click += new System.EventHandler(this.ExportLevelAsPng);
            // 
            // allRoomsAspngToolStripMenuItem
            // 
            this.allRoomsAspngToolStripMenuItem.Name = "allRoomsAspngToolStripMenuItem";
            this.allRoomsAspngToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.allRoomsAspngToolStripMenuItem.Text = "All Rooms as .png";
            this.allRoomsAspngToolStripMenuItem.Click += new System.EventHandler(this.ExportRoomsAsPng);
            // 
            // allRoomsAndActorsAspngToolStripMenuItem
            // 
            this.allRoomsAndActorsAspngToolStripMenuItem.Name = "allRoomsAndActorsAspngToolStripMenuItem";
            this.allRoomsAndActorsAspngToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.allRoomsAndActorsAspngToolStripMenuItem.Text = "All Rooms and Actors as .png";
            this.allRoomsAndActorsAspngToolStripMenuItem.Click += new System.EventHandler(this.ExportRoomsAsPng);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImportRoomFromTmx,
            this.toolStripSeparator5,
            this.roomImportToolStripMenuItem,
            this.actorsImportToolStripMenuItem});
            this.importToolStripMenuItem.Enabled = false;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // ImportRoomFromTmx
            // 
            this.ImportRoomFromTmx.Name = "ImportRoomFromTmx";
            this.ImportRoomFromTmx.Size = new System.Drawing.Size(162, 22);
            this.ImportRoomFromTmx.Text = "Room from .tmx";
            this.ImportRoomFromTmx.ToolTipText = "Import from Tiled map files";
            this.ImportRoomFromTmx.Click += new System.EventHandler(this.ImportRoomFromTmx_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(159, 6);
            // 
            // roomImportToolStripMenuItem
            // 
            this.roomImportToolStripMenuItem.Name = "roomImportToolStripMenuItem";
            this.roomImportToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.roomImportToolStripMenuItem.Text = "Room";
            this.roomImportToolStripMenuItem.ToolTipText = "Imports a room from another map file.";
            this.roomImportToolStripMenuItem.Click += new System.EventHandler(this.roomImportToolStripMenuItem_Click);
            // 
            // actorsImportToolStripMenuItem
            // 
            this.actorsImportToolStripMenuItem.Name = "actorsImportToolStripMenuItem";
            this.actorsImportToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.actorsImportToolStripMenuItem.Text = "Actors";
            this.actorsImportToolStripMenuItem.ToolTipText = "Imports actors from another room file to the current room.";
            this.actorsImportToolStripMenuItem.Click += new System.EventHandler(this.actorsImportToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(246, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator7,
            this.automaticSetTileActorsToolStripMenuItem,
            this.mirrorRoomToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.undoToolStripMenuItem.Text = "Undo Tile Change";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.redoToolStripMenuItem.Text = "Redo Tile Change";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(208, 6);
            // 
            // automaticSetTileActorsToolStripMenuItem
            // 
            this.automaticSetTileActorsToolStripMenuItem.Checked = true;
            this.automaticSetTileActorsToolStripMenuItem.CheckOnClick = true;
            this.automaticSetTileActorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.automaticSetTileActorsToolStripMenuItem.Name = "automaticSetTileActorsToolStripMenuItem";
            this.automaticSetTileActorsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.automaticSetTileActorsToolStripMenuItem.Text = "Automatic set tile actors";
            // 
            // mirrorRoomToolStripMenuItem
            // 
            this.mirrorRoomToolStripMenuItem.Name = "mirrorRoomToolStripMenuItem";
            this.mirrorRoomToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.mirrorRoomToolStripMenuItem.Text = "Mirror room";
            this.mirrorRoomToolStripMenuItem.Click += new System.EventHandler(this.mirrorRoomToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xSizeToolStripMenuItem,
            this.xSizeToolStripMenuItem1,
            this.toolStripSeparator6,
            this.displayOverlayToolStripMenuItem,
            this.textureFilterModeToolStripMenuItem,
            this.actorsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // xSizeToolStripMenuItem
            // 
            this.xSizeToolStripMenuItem.Name = "xSizeToolStripMenuItem";
            this.xSizeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.xSizeToolStripMenuItem.Text = "1x size";
            this.xSizeToolStripMenuItem.Click += new System.EventHandler(this.OneXSizeToolStripMenuItem_Click);
            // 
            // xSizeToolStripMenuItem1
            // 
            this.xSizeToolStripMenuItem1.Name = "xSizeToolStripMenuItem1";
            this.xSizeToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.xSizeToolStripMenuItem1.Text = "2x size";
            this.xSizeToolStripMenuItem1.Click += new System.EventHandler(this.TwoXSizeToolStripMenuItem1_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(150, 6);
            // 
            // displayOverlayToolStripMenuItem
            // 
            this.displayOverlayToolStripMenuItem.Checked = true;
            this.displayOverlayToolStripMenuItem.CheckOnClick = true;
            this.displayOverlayToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayOverlayToolStripMenuItem.Name = "displayOverlayToolStripMenuItem";
            this.displayOverlayToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.displayOverlayToolStripMenuItem.Text = "Display overlay";
            this.displayOverlayToolStripMenuItem.Click += new System.EventHandler(this.UpdateView_Click);
            // 
            // textureFilterModeToolStripMenuItem
            // 
            this.textureFilterModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bilinearToolStripMenuItem,
            this.bicubicToolStripMenuItem,
            this.nearestNeighborToolStripMenuItem});
            this.textureFilterModeToolStripMenuItem.Name = "textureFilterModeToolStripMenuItem";
            this.textureFilterModeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.textureFilterModeToolStripMenuItem.Text = "Filter mode";
            // 
            // bilinearToolStripMenuItem
            // 
            this.bilinearToolStripMenuItem.Name = "bilinearToolStripMenuItem";
            this.bilinearToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.bilinearToolStripMenuItem.Text = "Bilinear";
            this.bilinearToolStripMenuItem.Click += new System.EventHandler(this.bilinearToolStripMenuItem_Click);
            // 
            // bicubicToolStripMenuItem
            // 
            this.bicubicToolStripMenuItem.Name = "bicubicToolStripMenuItem";
            this.bicubicToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.bicubicToolStripMenuItem.Text = "Bicubic";
            this.bicubicToolStripMenuItem.Click += new System.EventHandler(this.bicubicToolStripMenuItem_Click);
            // 
            // nearestNeighborToolStripMenuItem
            // 
            this.nearestNeighborToolStripMenuItem.Checked = true;
            this.nearestNeighborToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nearestNeighborToolStripMenuItem.Name = "nearestNeighborToolStripMenuItem";
            this.nearestNeighborToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.nearestNeighborToolStripMenuItem.Text = "Nearest Neighbor";
            this.nearestNeighborToolStripMenuItem.Click += new System.EventHandler(this.nearestNeighborToolStripMenuItem_Click);
            // 
            // actorsToolStripMenuItem
            // 
            this.actorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoSelectToolStripMenuItem,
            this.alwaysShowActorsToolStripMenuItem});
            this.actorsToolStripMenuItem.Name = "actorsToolStripMenuItem";
            this.actorsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.actorsToolStripMenuItem.Text = "Actors";
            // 
            // autoSelectToolStripMenuItem
            // 
            this.autoSelectToolStripMenuItem.Checked = true;
            this.autoSelectToolStripMenuItem.CheckOnClick = true;
            this.autoSelectToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSelectToolStripMenuItem.Name = "autoSelectToolStripMenuItem";
            this.autoSelectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.autoSelectToolStripMenuItem.Text = "Auto select";
            this.autoSelectToolStripMenuItem.ToolTipText = "Auto load actor on room change.";
            // 
            // alwaysShowActorsToolStripMenuItem
            // 
            this.alwaysShowActorsToolStripMenuItem.CheckOnClick = true;
            this.alwaysShowActorsToolStripMenuItem.Name = "alwaysShowActorsToolStripMenuItem";
            this.alwaysShowActorsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.alwaysShowActorsToolStripMenuItem.Text = "Show Always";
            this.alwaysShowActorsToolStripMenuItem.ToolTipText = "Displays actors regardless of the current layer.";
            this.alwaysShowActorsToolStripMenuItem.Click += new System.EventHandler(this.UpdateView_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.wikiToolStripMenuItem,
            this.sourceCodeToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // wikiToolStripMenuItem
            // 
            this.wikiToolStripMenuItem.Name = "wikiToolStripMenuItem";
            this.wikiToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.wikiToolStripMenuItem.Text = "Wiki";
            this.wikiToolStripMenuItem.Click += new System.EventHandler(this.WikiToolStripMenuItem_Click);
            // 
            // sourceCodeToolStripMenuItem
            // 
            this.sourceCodeToolStripMenuItem.Name = "sourceCodeToolStripMenuItem";
            this.sourceCodeToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.sourceCodeToolStripMenuItem.Text = "Source code";
            this.sourceCodeToolStripMenuItem.Click += new System.EventHandler(this.sourceCodeToolStripMenuItem_Click);
            // 
            // rightSideGroupBox
            // 
            this.rightSideGroupBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rightSideGroupBox.Controls.Add(this.CoridinatesTextBox);
            this.rightSideGroupBox.Controls.Add(this.updateLayersButton);
            this.rightSideGroupBox.Controls.Add(this.buttonSaveLayers);
            this.rightSideGroupBox.Controls.Add(this.layersCheckList);
            this.rightSideGroupBox.Controls.Add(this.loggerTextBox);
            this.rightSideGroupBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightSideGroupBox.Enabled = false;
            this.rightSideGroupBox.Location = new System.Drawing.Point(795, 24);
            this.rightSideGroupBox.Name = "rightSideGroupBox";
            this.rightSideGroupBox.Size = new System.Drawing.Size(163, 552);
            this.rightSideGroupBox.TabIndex = 7;
            this.rightSideGroupBox.TabStop = false;
            this.rightSideGroupBox.Text = "Layers";
            // 
            // CoridinatesTextBox
            // 
            this.CoridinatesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CoridinatesTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.CoridinatesTextBox.Location = new System.Drawing.Point(7, 525);
            this.CoridinatesTextBox.Name = "CoridinatesTextBox";
            this.CoridinatesTextBox.ReadOnly = true;
            this.CoridinatesTextBox.Size = new System.Drawing.Size(150, 20);
            this.CoridinatesTextBox.TabIndex = 23;
            // 
            // updateLayersButton
            // 
            this.updateLayersButton.Location = new System.Drawing.Point(6, 178);
            this.updateLayersButton.Name = "updateLayersButton";
            this.updateLayersButton.Size = new System.Drawing.Size(75, 23);
            this.updateLayersButton.TabIndex = 20;
            this.updateLayersButton.Text = "Update view";
            this.updateLayersButton.UseVisualStyleBackColor = true;
            this.updateLayersButton.Click += new System.EventHandler(this.UpdateView_Click);
            // 
            // buttonSaveLayers
            // 
            this.buttonSaveLayers.Enabled = false;
            this.buttonSaveLayers.Location = new System.Drawing.Point(82, 178);
            this.buttonSaveLayers.Name = "buttonSaveLayers";
            this.buttonSaveLayers.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveLayers.TabIndex = 21;
            this.buttonSaveLayers.Text = "Save Layers";
            this.buttonSaveLayers.UseVisualStyleBackColor = true;
            this.buttonSaveLayers.Click += new System.EventHandler(this.buttonSaveLayers_Click);
            // 
            // layersCheckList
            // 
            this.layersCheckList.BackColor = System.Drawing.SystemColors.ControlLight;
            this.layersCheckList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.layersCheckList.CheckOnClick = true;
            this.layersCheckList.ColumnWidth = 70;
            this.layersCheckList.FormattingEnabled = true;
            this.layersCheckList.Items.AddRange(new object[] {
            "Layer 1-0",
            "Layer 1-1",
            "Layer 1-2",
            "Layer 1-3",
            "Layer 1-4",
            "Layer 1-5",
            "Layer 1-6",
            "Layer 1-7",
            "Layer 2-0",
            "Layer 2-1",
            "Layer 2-2",
            "Layer 2-3",
            "Layer 2-4",
            "Layer 2-5",
            "Layer 2-6",
            "Layer 2-7"});
            this.layersCheckList.Location = new System.Drawing.Point(6, 16);
            this.layersCheckList.MultiColumn = true;
            this.layersCheckList.Name = "layersCheckList";
            this.layersCheckList.Size = new System.Drawing.Size(145, 120);
            this.layersCheckList.TabIndex = 19;
            this.layersCheckList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LayersCheckList_ItemCheck);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl.Enabled = false;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(281, 552);
            this.tabControl.TabIndex = 8;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_TabIndexChanged);
            this.tabControl.TabIndexChanged += new System.EventHandler(this.tabControl_TabIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.MapTabControl);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(273, 526);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Map Info";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // MapTabControl
            // 
            this.MapTabControl.Controls.Add(this.MapMultiplayerTabPage);
            this.MapTabControl.Controls.Add(this.MapSinglelplayerTabPage);
            this.MapTabControl.Location = new System.Drawing.Point(0, 0);
            this.MapTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.MapTabControl.Name = "MapTabControl";
            this.MapTabControl.SelectedIndex = 0;
            this.MapTabControl.Size = new System.Drawing.Size(273, 526);
            this.MapTabControl.TabIndex = 1;
            // 
            // MapMultiplayerTabPage
            // 
            this.MapMultiplayerTabPage.Controls.Add(this.MapEditor);
            this.MapMultiplayerTabPage.Location = new System.Drawing.Point(4, 22);
            this.MapMultiplayerTabPage.Name = "MapMultiplayerTabPage";
            this.MapMultiplayerTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MapMultiplayerTabPage.Size = new System.Drawing.Size(265, 500);
            this.MapMultiplayerTabPage.TabIndex = 0;
            this.MapMultiplayerTabPage.Text = "Multiplayer";
            this.MapMultiplayerTabPage.UseVisualStyleBackColor = true;
            // 
            // MapEditor
            // 
            this.MapEditor.Location = new System.Drawing.Point(0, 0);
            this.MapEditor.Margin = new System.Windows.Forms.Padding(0);
            this.MapEditor.Name = "MapEditor";
            this.MapEditor.Size = new System.Drawing.Size(266, 500);
            this.MapEditor.TabIndex = 0;
            this.MapEditor.LoadRoom += new System.EventHandler<EFSAdvent.Controls.MapEditor>(this.mapEditor_LoadRoom);
            this.MapEditor.NewRoom += new System.EventHandler<EFSAdvent.Controls.MapEditor>(this.mapEditor_NewRoom);
            this.MapEditor.SelectedRoomCoordinatesChanged += new System.EventHandler<EFSAdvent.Controls.MapEditor>(this.mapEditor_SelectedRoomCoordinatesChanged);
            // 
            // MapSinglelplayerTabPage
            // 
            this.MapSinglelplayerTabPage.Controls.Add(this.MapEditorSinglelplayer);
            this.MapSinglelplayerTabPage.Location = new System.Drawing.Point(4, 22);
            this.MapSinglelplayerTabPage.Name = "MapSinglelplayerTabPage";
            this.MapSinglelplayerTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MapSinglelplayerTabPage.Size = new System.Drawing.Size(265, 500);
            this.MapSinglelplayerTabPage.TabIndex = 1;
            this.MapSinglelplayerTabPage.Text = "Singlelplayer";
            this.MapSinglelplayerTabPage.UseVisualStyleBackColor = true;
            // 
            // MapEditorSinglelplayer
            // 
            this.MapEditorSinglelplayer.Location = new System.Drawing.Point(0, 0);
            this.MapEditorSinglelplayer.Margin = new System.Windows.Forms.Padding(0);
            this.MapEditorSinglelplayer.Name = "MapEditorSinglelplayer";
            this.MapEditorSinglelplayer.Size = new System.Drawing.Size(266, 500);
            this.MapEditorSinglelplayer.TabIndex = 1;
            this.MapEditorSinglelplayer.LoadRoom += new System.EventHandler<EFSAdvent.Controls.MapEditor>(this.mapEditor_LoadRoom);
            this.MapEditorSinglelplayer.NewRoom += new System.EventHandler<EFSAdvent.Controls.MapEditor>(this.mapEditor_NewRoom);
            this.MapEditorSinglelplayer.SelectedRoomCoordinatesChanged += new System.EventHandler<EFSAdvent.Controls.MapEditor>(this.mapEditor_SelectedRoomCoordinatesChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.MirrorBrushbutton);
            this.tabPage2.Controls.Add(this.BrushSizeLabel);
            this.tabPage2.Controls.Add(this.BrushSizeComboBox);
            this.tabPage2.Controls.Add(this.BrushTileLabel);
            this.tabPage2.Controls.Add(this.BrushTilePictureBox);
            this.tabPage2.Controls.Add(this.tileSheetPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(273, 526);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tile Sheet";
            // 
            // MirrorBrushbutton
            // 
            this.MirrorBrushbutton.Location = new System.Drawing.Point(148, 7);
            this.MirrorBrushbutton.Name = "MirrorBrushbutton";
            this.MirrorBrushbutton.Size = new System.Drawing.Size(52, 21);
            this.MirrorBrushbutton.TabIndex = 8;
            this.MirrorBrushbutton.Text = "Mirror";
            this.MirrorBrushbutton.UseVisualStyleBackColor = true;
            this.MirrorBrushbutton.Click += new System.EventHandler(this.MirrorBrushbutton_Click);
            // 
            // BrushSizeLabel
            // 
            this.BrushSizeLabel.AutoSize = true;
            this.BrushSizeLabel.Location = new System.Drawing.Point(6, 10);
            this.BrushSizeLabel.Name = "BrushSizeLabel";
            this.BrushSizeLabel.Size = new System.Drawing.Size(77, 13);
            this.BrushSizeLabel.TabIndex = 7;
            this.BrushSizeLabel.Text = "Tile brush size:";
            // 
            // BrushSizeComboBox
            // 
            this.BrushSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BrushSizeComboBox.FormattingEnabled = true;
            this.BrushSizeComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.BrushSizeComboBox.Location = new System.Drawing.Point(93, 7);
            this.BrushSizeComboBox.Name = "BrushSizeComboBox";
            this.BrushSizeComboBox.Size = new System.Drawing.Size(49, 21);
            this.BrushSizeComboBox.TabIndex = 6;
            this.BrushSizeComboBox.SelectionChangeCommitted += new System.EventHandler(this.BrushSizeComboBox_SelectionChangeCommitted);
            // 
            // BrushTileLabel
            // 
            this.BrushTileLabel.AutoSize = true;
            this.BrushTileLabel.BackColor = System.Drawing.Color.Transparent;
            this.BrushTileLabel.Location = new System.Drawing.Point(206, 10);
            this.BrushTileLabel.Name = "BrushTileLabel";
            this.BrushTileLabel.Size = new System.Drawing.Size(13, 13);
            this.BrushTileLabel.TabIndex = 5;
            this.BrushTileLabel.Text = "0";
            // 
            // BrushTilePictureBox
            // 
            this.BrushTilePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BrushTilePictureBox.Location = new System.Drawing.Point(239, 2);
            this.BrushTilePictureBox.Name = "BrushTilePictureBox";
            this.BrushTilePictureBox.Size = new System.Drawing.Size(32, 32);
            this.BrushTilePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BrushTilePictureBox.TabIndex = 4;
            this.BrushTilePictureBox.TabStop = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.SaveStampButton);
            this.tabPage4.Controls.Add(this.MirrorStampButton);
            this.tabPage4.Controls.Add(this.DeleteStampButton);
            this.tabPage4.Controls.Add(this.TileStampFlowLayoutPanel);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(273, 526);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Text = "Stamps";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // SaveStampButton
            // 
            this.SaveStampButton.Location = new System.Drawing.Point(6, 6);
            this.SaveStampButton.Name = "SaveStampButton";
            this.SaveStampButton.Size = new System.Drawing.Size(80, 22);
            this.SaveStampButton.TabIndex = 10;
            this.SaveStampButton.Text = "Save";
            this.SaveStampButton.Click += new System.EventHandler(this.SaveStampButton_Click);
            // 
            // MirrorStampButton
            // 
            this.MirrorStampButton.Location = new System.Drawing.Point(187, 6);
            this.MirrorStampButton.Name = "MirrorStampButton";
            this.MirrorStampButton.Size = new System.Drawing.Size(80, 22);
            this.MirrorStampButton.TabIndex = 9;
            this.MirrorStampButton.Text = "Mirror";
            this.MirrorStampButton.UseVisualStyleBackColor = true;
            this.MirrorStampButton.Click += new System.EventHandler(this.MirrorBrushbutton_Click);
            // 
            // DeleteStampButton
            // 
            this.DeleteStampButton.Enabled = false;
            this.DeleteStampButton.Location = new System.Drawing.Point(92, 6);
            this.DeleteStampButton.Name = "DeleteStampButton";
            this.DeleteStampButton.Size = new System.Drawing.Size(80, 22);
            this.DeleteStampButton.TabIndex = 5;
            this.DeleteStampButton.Text = "Delete";
            this.DeleteStampButton.UseVisualStyleBackColor = true;
            this.DeleteStampButton.Click += new System.EventHandler(this.DeleteStampButton_Click);
            // 
            // TileStampFlowLayoutPanel
            // 
            this.TileStampFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TileStampFlowLayoutPanel.AutoScroll = true;
            this.TileStampFlowLayoutPanel.Location = new System.Drawing.Point(0, 34);
            this.TileStampFlowLayoutPanel.Name = "TileStampFlowLayoutPanel";
            this.TileStampFlowLayoutPanel.SelectedIndex = -1;
            this.TileStampFlowLayoutPanel.Size = new System.Drawing.Size(273, 489);
            this.TileStampFlowLayoutPanel.TabIndex = 0;
            this.TileStampFlowLayoutPanel.SelectionChanged += new System.EventHandler(this.TileStampFlowLayoutPanel_SelectionChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonActorsSelectNone);
            this.tabPage1.Controls.Add(this.actorLayerComboBox);
            this.tabPage1.Controls.Add(this.actorAttributesgroupBox);
            this.tabPage1.Controls.Add(this.actorsCheckListBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(273, 526);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Actors";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonActorsSelectNone
            // 
            this.buttonActorsSelectNone.Location = new System.Drawing.Point(7, 33);
            this.buttonActorsSelectNone.Name = "buttonActorsSelectNone";
            this.buttonActorsSelectNone.Size = new System.Drawing.Size(74, 23);
            this.buttonActorsSelectNone.TabIndex = 3;
            this.buttonActorsSelectNone.Text = "Select none";
            this.buttonActorsSelectNone.UseVisualStyleBackColor = true;
            this.buttonActorsSelectNone.Click += new System.EventHandler(this.buttonActorsSelectNone_Click);
            // 
            // actorLayerComboBox
            // 
            this.actorLayerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.actorLayerComboBox.Enabled = false;
            this.actorLayerComboBox.FormattingEnabled = true;
            this.actorLayerComboBox.Items.AddRange(new object[] {
            "All Actors",
            "Layer 0",
            "Layer 1",
            "Layer 2",
            "Layer 3",
            "Layer 4",
            "Layer 5",
            "Layer 6",
            "Layer 7",
            "Variable 1",
            "Variable 2",
            "Variable 3",
            "Variable 4",
            "Variable 5",
            "Variable 6",
            "Variable 7",
            "Variable 8",
            "Variable 9",
            "Variable 10",
            "Variable 11",
            "Variable 12",
            "Variable 13",
            "Variable 14",
            "Variable 15",
            "Variable 16",
            "Variable 17",
            "Variable 18",
            "Variable 19",
            "Variable 20",
            "Variable 21",
            "Variable 22",
            "Variable 23",
            "Variable 24",
            "Variable 25",
            "Variable 26",
            "Variable 27",
            "Variable 28",
            "Variable 29",
            "Variable 30",
            "Variable 31"});
            this.actorLayerComboBox.Location = new System.Drawing.Point(7, 6);
            this.actorLayerComboBox.Name = "actorLayerComboBox";
            this.actorLayerComboBox.Size = new System.Drawing.Size(74, 21);
            this.actorLayerComboBox.TabIndex = 0;
            this.actorLayerComboBox.SelectionChangeCommitted += new System.EventHandler(this.actorLayerComboBox_SelectionChangeCommitted);
            // 
            // actorAttributesgroupBox
            // 
            this.actorAttributesgroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.actorAttributesgroupBox.Controls.Add(this.ActorInfoPictureBox);
            this.actorAttributesgroupBox.Controls.Add(this.tabPageRawVariable);
            this.actorAttributesgroupBox.Controls.Add(this.cloneButton);
            this.actorAttributesgroupBox.Controls.Add(this.actorDeleteButton);
            this.actorAttributesgroupBox.Controls.Add(this.groupBoxCoridinate);
            this.actorAttributesgroupBox.Controls.Add(this.ActorNameComboBox);
            this.actorAttributesgroupBox.Enabled = false;
            this.actorAttributesgroupBox.Location = new System.Drawing.Point(87, 6);
            this.actorAttributesgroupBox.Name = "actorAttributesgroupBox";
            this.actorAttributesgroupBox.Size = new System.Drawing.Size(180, 512);
            this.actorAttributesgroupBox.TabIndex = 5;
            this.actorAttributesgroupBox.TabStop = false;
            this.actorAttributesgroupBox.Text = "Actor Attributes";
            // 
            // ActorInfoPictureBox
            // 
            this.ActorInfoPictureBox.BackColor = System.Drawing.Color.DarkGray;
            this.ActorInfoPictureBox.Location = new System.Drawing.Point(113, 56);
            this.ActorInfoPictureBox.MaximumSize = new System.Drawing.Size(64, 64);
            this.ActorInfoPictureBox.MinimumSize = new System.Drawing.Size(32, 32);
            this.ActorInfoPictureBox.Name = "ActorInfoPictureBox";
            this.ActorInfoPictureBox.Size = new System.Drawing.Size(61, 62);
            this.ActorInfoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ActorInfoPictureBox.TabIndex = 1;
            this.ActorInfoPictureBox.TabStop = false;
            // 
            // tabPageRawVariable
            // 
            this.tabPageRawVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabPageRawVariable.Controls.Add(this.Variables5TabPage);
            this.tabPageRawVariable.Controls.Add(this.tabPageFieldsVariable);
            this.tabPageRawVariable.ItemSize = new System.Drawing.Size(20, 18);
            this.tabPageRawVariable.Location = new System.Drawing.Point(6, 137);
            this.tabPageRawVariable.Name = "tabPageRawVariable";
            this.tabPageRawVariable.Padding = new System.Drawing.Point(3, 3);
            this.tabPageRawVariable.SelectedIndex = 1;
            this.tabPageRawVariable.Size = new System.Drawing.Size(170, 341);
            this.tabPageRawVariable.TabIndex = 33;
            this.ActorAttributesTip.SetToolTip(this.tabPageRawVariable, "Specify the view of the actor variables");
            // 
            // Variables5TabPage
            // 
            this.Variables5TabPage.Controls.Add(this.tabControlRawVarType);
            this.Variables5TabPage.Controls.Add(this.groupBoxFullVariable);
            this.Variables5TabPage.Location = new System.Drawing.Point(4, 22);
            this.Variables5TabPage.Name = "Variables5TabPage";
            this.Variables5TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.Variables5TabPage.Size = new System.Drawing.Size(162, 315);
            this.Variables5TabPage.TabIndex = 1;
            this.Variables5TabPage.Text = "Raw";
            this.Variables5TabPage.UseVisualStyleBackColor = true;
            // 
            // tabControlRawVarType
            // 
            this.tabControlRawVarType.Controls.Add(this.tabPageV5);
            this.tabControlRawVarType.Controls.Add(this.tabPageV6);
            this.tabControlRawVarType.Location = new System.Drawing.Point(6, 63);
            this.tabControlRawVarType.Name = "tabControlRawVarType";
            this.tabControlRawVarType.SelectedIndex = 0;
            this.tabControlRawVarType.Size = new System.Drawing.Size(151, 246);
            this.tabControlRawVarType.TabIndex = 48;
            this.ActorAttributesTip.SetToolTip(this.tabControlRawVarType, "Divides the full variable into sections.");
            // 
            // tabPageV5
            // 
            this.tabPageV5.Controls.Add(this.label_V5_3);
            this.tabPageV5.Controls.Add(this.label_V5_3a);
            this.tabPageV5.Controls.Add(this.ActorVariable4AInput);
            this.tabPageV5.Controls.Add(this.ActorVariable1Input);
            this.tabPageV5.Controls.Add(this.label_V5_4);
            this.tabPageV5.Controls.Add(this.ActorVariable2Input);
            this.tabPageV5.Controls.Add(this.label_V5_1);
            this.tabPageV5.Controls.Add(this.ActorVariable4BInput);
            this.tabPageV5.Controls.Add(this.ActorVariable3Input);
            this.tabPageV5.Controls.Add(this.label_V5_2);
            this.tabPageV5.Location = new System.Drawing.Point(4, 22);
            this.tabPageV5.Name = "tabPageV5";
            this.tabPageV5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageV5.Size = new System.Drawing.Size(143, 220);
            this.tabPageV5.TabIndex = 0;
            this.tabPageV5.Text = "V5";
            this.tabPageV5.UseVisualStyleBackColor = true;
            // 
            // label_V5_3
            // 
            this.label_V5_3.AutoSize = true;
            this.label_V5_3.Location = new System.Drawing.Point(6, 86);
            this.label_V5_3.Name = "label_V5_3";
            this.label_V5_3.Size = new System.Drawing.Size(57, 13);
            this.label_V5_3.TabIndex = 40;
            this.label_V5_3.Text = "Variable 3:";
            // 
            // label_V5_3a
            // 
            this.label_V5_3a.AutoSize = true;
            this.label_V5_3a.Location = new System.Drawing.Point(6, 112);
            this.label_V5_3a.Name = "label_V5_3a";
            this.label_V5_3a.Size = new System.Drawing.Size(43, 13);
            this.label_V5_3a.TabIndex = 41;
            this.label_V5_3a.Text = "Trigger:";
            // 
            // ActorVariable4AInput
            // 
            this.ActorVariable4AInput.Location = new System.Drawing.Point(86, 110);
            this.ActorVariable4AInput.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.ActorVariable4AInput.Name = "ActorVariable4AInput";
            this.ActorVariable4AInput.Size = new System.Drawing.Size(51, 20);
            this.ActorVariable4AInput.TabIndex = 40;
            this.ActorAttributesTip.SetToolTip(this.ActorVariable4AInput, "The last 5 bits of the fifth byte.");
            this.ActorVariable4AInput.ValueChanged += new System.EventHandler(this.ActorChanged);
            // 
            // ActorVariable1Input
            // 
            this.ActorVariable1Input.Location = new System.Drawing.Point(86, 6);
            this.ActorVariable1Input.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.ActorVariable1Input.Name = "ActorVariable1Input";
            this.ActorVariable1Input.Size = new System.Drawing.Size(51, 20);
            this.ActorVariable1Input.TabIndex = 36;
            this.ActorAttributesTip.SetToolTip(this.ActorVariable1Input, "The 8 bits of the first byte.");
            this.ActorVariable1Input.ValueChanged += new System.EventHandler(this.ActorChanged);
            // 
            // label_V5_4
            // 
            this.label_V5_4.AutoSize = true;
            this.label_V5_4.Location = new System.Drawing.Point(6, 8);
            this.label_V5_4.Name = "label_V5_4";
            this.label_V5_4.Size = new System.Drawing.Size(34, 13);
            this.label_V5_4.TabIndex = 38;
            this.label_V5_4.Text = "Type:";
            // 
            // ActorVariable2Input
            // 
            this.ActorVariable2Input.Location = new System.Drawing.Point(86, 32);
            this.ActorVariable2Input.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.ActorVariable2Input.Name = "ActorVariable2Input";
            this.ActorVariable2Input.Size = new System.Drawing.Size(51, 20);
            this.ActorVariable2Input.TabIndex = 35;
            this.ActorAttributesTip.SetToolTip(this.ActorVariable2Input, "The second byte.");
            this.ActorVariable2Input.ValueChanged += new System.EventHandler(this.ActorChanged);
            // 
            // label_V5_1
            // 
            this.label_V5_1.AutoSize = true;
            this.label_V5_1.Location = new System.Drawing.Point(6, 34);
            this.label_V5_1.Name = "label_V5_1";
            this.label_V5_1.Size = new System.Drawing.Size(57, 13);
            this.label_V5_1.TabIndex = 37;
            this.label_V5_1.Text = "Variable 1:";
            // 
            // ActorVariable4BInput
            // 
            this.ActorVariable4BInput.Location = new System.Drawing.Point(86, 84);
            this.ActorVariable4BInput.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.ActorVariable4BInput.Name = "ActorVariable4BInput";
            this.ActorVariable4BInput.Size = new System.Drawing.Size(51, 20);
            this.ActorVariable4BInput.TabIndex = 39;
            this.ActorAttributesTip.SetToolTip(this.ActorVariable4BInput, "The first 3 bits of the fifth byte.");
            this.ActorVariable4BInput.ValueChanged += new System.EventHandler(this.ActorChanged);
            // 
            // ActorVariable3Input
            // 
            this.ActorVariable3Input.Location = new System.Drawing.Point(86, 58);
            this.ActorVariable3Input.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.ActorVariable3Input.Name = "ActorVariable3Input";
            this.ActorVariable3Input.Size = new System.Drawing.Size(51, 20);
            this.ActorVariable3Input.TabIndex = 34;
            this.ActorAttributesTip.SetToolTip(this.ActorVariable3Input, "The third byte.");
            this.ActorVariable3Input.ValueChanged += new System.EventHandler(this.ActorChanged);
            // 
            // label_V5_2
            // 
            this.label_V5_2.AutoSize = true;
            this.label_V5_2.Location = new System.Drawing.Point(6, 60);
            this.label_V5_2.Name = "label_V5_2";
            this.label_V5_2.Size = new System.Drawing.Size(57, 13);
            this.label_V5_2.TabIndex = 33;
            this.label_V5_2.Text = "Variable 2:";
            // 
            // tabPageV6
            // 
            this.tabPageV6.Controls.Add(this.label21);
            this.tabPageV6.Controls.Add(this.label15);
            this.tabPageV6.Controls.Add(this.label14);
            this.tabPageV6.Controls.Add(this.label13);
            this.tabPageV6.Controls.Add(this.label12);
            this.tabPageV6.Controls.Add(this.label9);
            this.tabPageV6.Controls.Add(this.ActorV6Variable2fInput);
            this.tabPageV6.Controls.Add(this.ActorV6Variable1Input);
            this.tabPageV6.Controls.Add(this.ActorV6Variable4Input);
            this.tabPageV6.Controls.Add(this.ActorV6Variable3Input);
            this.tabPageV6.Controls.Add(this.ActorV6Variable6Input);
            this.tabPageV6.Controls.Add(this.ActorV6Variable5Input);
            this.tabPageV6.Location = new System.Drawing.Point(4, 22);
            this.tabPageV6.Name = "tabPageV6";
            this.tabPageV6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageV6.Size = new System.Drawing.Size(143, 220);
            this.tabPageV6.TabIndex = 1;
            this.tabPageV6.Text = "V6";
            this.tabPageV6.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 34);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(57, 13);
            this.label21.TabIndex = 70;
            this.label21.Text = "Variable 1:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 60);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 69;
            this.label15.Text = "Variable 2:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 86);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(57, 13);
            this.label14.TabIndex = 68;
            this.label14.Text = "Variable 3:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 112);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 13);
            this.label13.TabIndex = 67;
            this.label13.Text = "Variable 4:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 138);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 13);
            this.label12.TabIndex = 66;
            this.label12.Text = "Trigger:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 65;
            this.label9.Text = "Type:";
            // 
            // ActorV6Variable2fInput
            // 
            this.ActorV6Variable2fInput.Location = new System.Drawing.Point(86, 32);
            this.ActorV6Variable2fInput.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.ActorV6Variable2fInput.Name = "ActorV6Variable2fInput";
            this.ActorV6Variable2fInput.Size = new System.Drawing.Size(51, 20);
            this.ActorV6Variable2fInput.TabIndex = 64;
            this.ActorAttributesTip.SetToolTip(this.ActorV6Variable2fInput, "The next 5 bits.");
            this.ActorV6Variable2fInput.ValueChanged += new System.EventHandler(this.ActorChangedV6);
            // 
            // ActorV6Variable1Input
            // 
            this.ActorV6Variable1Input.Location = new System.Drawing.Point(86, 6);
            this.ActorV6Variable1Input.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.ActorV6Variable1Input.Name = "ActorV6Variable1Input";
            this.ActorV6Variable1Input.Size = new System.Drawing.Size(51, 20);
            this.ActorV6Variable1Input.TabIndex = 63;
            this.ActorAttributesTip.SetToolTip(this.ActorV6Variable1Input, "The first 7 bits of the first byte.");
            this.ActorV6Variable1Input.ValueChanged += new System.EventHandler(this.ActorChangedV6);
            // 
            // ActorV6Variable4Input
            // 
            this.ActorV6Variable4Input.Location = new System.Drawing.Point(86, 84);
            this.ActorV6Variable4Input.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.ActorV6Variable4Input.Name = "ActorV6Variable4Input";
            this.ActorV6Variable4Input.Size = new System.Drawing.Size(51, 20);
            this.ActorV6Variable4Input.TabIndex = 60;
            this.ActorAttributesTip.SetToolTip(this.ActorV6Variable4Input, "The next 5 bits.");
            this.ActorV6Variable4Input.ValueChanged += new System.EventHandler(this.ActorChangedV6);
            // 
            // ActorV6Variable3Input
            // 
            this.ActorV6Variable3Input.Location = new System.Drawing.Point(86, 58);
            this.ActorV6Variable3Input.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.ActorV6Variable3Input.Name = "ActorV6Variable3Input";
            this.ActorV6Variable3Input.Size = new System.Drawing.Size(51, 20);
            this.ActorV6Variable3Input.TabIndex = 59;
            this.ActorAttributesTip.SetToolTip(this.ActorV6Variable3Input, "The next 5 bits.");
            this.ActorV6Variable3Input.ValueChanged += new System.EventHandler(this.ActorChangedV6);
            // 
            // ActorV6Variable6Input
            // 
            this.ActorV6Variable6Input.Location = new System.Drawing.Point(86, 136);
            this.ActorV6Variable6Input.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.ActorV6Variable6Input.Name = "ActorV6Variable6Input";
            this.ActorV6Variable6Input.Size = new System.Drawing.Size(51, 20);
            this.ActorV6Variable6Input.TabIndex = 56;
            this.ActorAttributesTip.SetToolTip(this.ActorV6Variable6Input, "The last 5 bits of the fifth byte.");
            this.ActorV6Variable6Input.ValueChanged += new System.EventHandler(this.ActorChangedV6);
            // 
            // ActorV6Variable5Input
            // 
            this.ActorV6Variable5Input.Location = new System.Drawing.Point(86, 110);
            this.ActorV6Variable5Input.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.ActorV6Variable5Input.Name = "ActorV6Variable5Input";
            this.ActorV6Variable5Input.Size = new System.Drawing.Size(51, 20);
            this.ActorV6Variable5Input.TabIndex = 55;
            this.ActorAttributesTip.SetToolTip(this.ActorV6Variable5Input, "The next 5 bits.");
            this.ActorV6Variable5Input.ValueChanged += new System.EventHandler(this.ActorChangedV6);
            // 
            // groupBoxFullVariable
            // 
            this.groupBoxFullVariable.Controls.Add(this.ActorVariableFullInput);
            this.groupBoxFullVariable.Location = new System.Drawing.Point(6, 6);
            this.groupBoxFullVariable.Name = "groupBoxFullVariable";
            this.groupBoxFullVariable.Size = new System.Drawing.Size(151, 51);
            this.groupBoxFullVariable.TabIndex = 47;
            this.groupBoxFullVariable.TabStop = false;
            this.groupBoxFullVariable.Text = "Full Variable";
            // 
            // ActorVariableFullInput
            // 
            this.ActorVariableFullInput.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ActorVariableFullInput.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.ActorVariableFullInput.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ActorVariableFullInput.InterceptArrowKeys = false;
            this.ActorVariableFullInput.Location = new System.Drawing.Point(6, 19);
            this.ActorVariableFullInput.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.ActorVariableFullInput.Name = "ActorVariableFullInput";
            this.ActorVariableFullInput.ReadOnly = true;
            this.ActorVariableFullInput.Size = new System.Drawing.Size(139, 20);
            this.ActorVariableFullInput.TabIndex = 46;
            this.ActorAttributesTip.SetToolTip(this.ActorVariableFullInput, "The 32bit value of the actor variable.");
            // 
            // tabPageFieldsVariable
            // 
            this.tabPageFieldsVariable.Controls.Add(this.panelActorFields);
            this.tabPageFieldsVariable.Location = new System.Drawing.Point(4, 22);
            this.tabPageFieldsVariable.Name = "tabPageFieldsVariable";
            this.tabPageFieldsVariable.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFieldsVariable.Size = new System.Drawing.Size(162, 315);
            this.tabPageFieldsVariable.TabIndex = 3;
            this.tabPageFieldsVariable.Text = "Fields";
            this.tabPageFieldsVariable.UseVisualStyleBackColor = true;
            // 
            // panelActorFields
            // 
            this.panelActorFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelActorFields.AutoScroll = true;
            this.panelActorFields.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelActorFields.Location = new System.Drawing.Point(0, 0);
            this.panelActorFields.Name = "panelActorFields";
            this.panelActorFields.Size = new System.Drawing.Size(162, 315);
            this.panelActorFields.TabIndex = 0;
            this.panelActorFields.WrapContents = false;
            // 
            // cloneButton
            // 
            this.cloneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cloneButton.Location = new System.Drawing.Point(94, 484);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(80, 22);
            this.cloneButton.TabIndex = 31;
            this.cloneButton.Text = "Copy";
            this.ActorAttributesTip.SetToolTip(this.cloneButton, "Copies the currently selected actor to the clipboard.");
            this.cloneButton.UseVisualStyleBackColor = true;
            this.cloneButton.Click += new System.EventHandler(this.CopyActorToClipboard);
            // 
            // actorDeleteButton
            // 
            this.actorDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actorDeleteButton.Enabled = false;
            this.actorDeleteButton.Location = new System.Drawing.Point(6, 484);
            this.actorDeleteButton.Name = "actorDeleteButton";
            this.actorDeleteButton.Size = new System.Drawing.Size(80, 22);
            this.actorDeleteButton.TabIndex = 4;
            this.actorDeleteButton.Text = "Delete";
            this.actorDeleteButton.UseVisualStyleBackColor = true;
            this.actorDeleteButton.Click += new System.EventHandler(this.actorDeleteButton_Click);
            // 
            // groupBoxCoridinate
            // 
            this.groupBoxCoridinate.Controls.Add(this.ActorXCoordInput);
            this.groupBoxCoridinate.Controls.Add(this.ActorYCoordInput);
            this.groupBoxCoridinate.Controls.Add(this.label8);
            this.groupBoxCoridinate.Controls.Add(this.label7);
            this.groupBoxCoridinate.Controls.Add(this.ActorLayerInput);
            this.groupBoxCoridinate.Controls.Add(this.label6);
            this.groupBoxCoridinate.Location = new System.Drawing.Point(6, 46);
            this.groupBoxCoridinate.Name = "groupBoxCoridinate";
            this.groupBoxCoridinate.Size = new System.Drawing.Size(101, 90);
            this.groupBoxCoridinate.TabIndex = 32;
            this.groupBoxCoridinate.TabStop = false;
            this.groupBoxCoridinate.Text = "Coridinate";
            // 
            // ActorXCoordInput
            // 
            this.ActorXCoordInput.Location = new System.Drawing.Point(33, 43);
            this.ActorXCoordInput.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.ActorXCoordInput.Name = "ActorXCoordInput";
            this.ActorXCoordInput.Size = new System.Drawing.Size(51, 20);
            this.ActorXCoordInput.TabIndex = 25;
            this.ActorXCoordInput.ValueChanged += new System.EventHandler(this.ActorChanged);
            this.ActorXCoordInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ActorChanged);
            // 
            // ActorYCoordInput
            // 
            this.ActorYCoordInput.Location = new System.Drawing.Point(33, 69);
            this.ActorYCoordInput.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.ActorYCoordInput.Name = "ActorYCoordInput";
            this.ActorYCoordInput.Size = new System.Drawing.Size(51, 20);
            this.ActorYCoordInput.TabIndex = 26;
            this.ActorYCoordInput.ValueChanged += new System.EventHandler(this.ActorChanged);
            this.ActorYCoordInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ActorChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "X:";
            // 
            // ActorLayerInput
            // 
            this.ActorLayerInput.Location = new System.Drawing.Point(33, 17);
            this.ActorLayerInput.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.ActorLayerInput.Name = "ActorLayerInput";
            this.ActorLayerInput.Size = new System.Drawing.Size(51, 20);
            this.ActorLayerInput.TabIndex = 24;
            this.ActorLayerInput.ValueChanged += new System.EventHandler(this.ActorChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Layer:";
            this.ActorAttributesTip.SetToolTip(this.label6, "A number between 0 and 7 which selects\r\nthe layer this actor appears on.");
            // 
            // ActorNameComboBox
            // 
            this.ActorNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ActorNameComboBox.FormattingEnabled = true;
            this.ActorNameComboBox.Location = new System.Drawing.Point(6, 19);
            this.ActorNameComboBox.MaxDropDownItems = 14;
            this.ActorNameComboBox.Name = "ActorNameComboBox";
            this.ActorNameComboBox.Size = new System.Drawing.Size(168, 21);
            this.ActorNameComboBox.TabIndex = 5;
            this.ActorNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.ActorChanged);
            // 
            // actorsCheckListBox
            // 
            this.actorsCheckListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.actorsCheckListBox.FormattingEnabled = true;
            this.actorsCheckListBox.Location = new System.Drawing.Point(7, 62);
            this.actorsCheckListBox.Name = "actorsCheckListBox";
            this.actorsCheckListBox.Size = new System.Drawing.Size(74, 454);
            this.actorsCheckListBox.TabIndex = 2;
            this.actorsCheckListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.actorsCheckListBox_ItemCheck);
            this.actorsCheckListBox.Click += new System.EventHandler(this.actorsCheckListBox_Click);
            this.actorsCheckListBox.SelectedIndexChanged += new System.EventHandler(this.ActorsCheckListBox_SelectedIndexChanged);
            this.actorsCheckListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.actorsCheckListBox_KeyUp);
            // 
            // ActorAttributesTip
            // 
            this.ActorAttributesTip.AutoPopDelay = 10000;
            this.ActorAttributesTip.InitialDelay = 500;
            this.ActorAttributesTip.ReshowDelay = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // BottomGroupBox
            // 
            this.BottomGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BottomGroupBox.Controls.Add(this.RootFolderPathTextBox);
            this.BottomGroupBox.Location = new System.Drawing.Point(283, 540);
            this.BottomGroupBox.Name = "BottomGroupBox";
            this.BottomGroupBox.Size = new System.Drawing.Size(506, 32);
            this.BottomGroupBox.TabIndex = 24;
            this.BottomGroupBox.TabStop = false;
            // 
            // RootFolderPathTextBox
            // 
            this.RootFolderPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.RootFolderPathTextBox.Location = new System.Drawing.Point(6, 9);
            this.RootFolderPathTextBox.Name = "RootFolderPathTextBox";
            this.RootFolderPathTextBox.ReadOnly = true;
            this.RootFolderPathTextBox.Size = new System.Drawing.Size(494, 20);
            this.RootFolderPathTextBox.TabIndex = 23;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 576);
            this.Controls.Add(this.BottomGroupBox);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.rightSideGroupBox);
            this.Controls.Add(this.layersPanel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(974, 614);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "EFSAdvent";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tileSheetPanel.ResumeLayout(false);
            this.tileSheetPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileSheetPictureBox)).EndInit();
            this.layersPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layerPictureBox)).EndInit();
            this.actorContextMenuStrip.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.rightSideGroupBox.ResumeLayout(false);
            this.rightSideGroupBox.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.MapTabControl.ResumeLayout(false);
            this.MapMultiplayerTabPage.ResumeLayout(false);
            this.MapSinglelplayerTabPage.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrushTilePictureBox)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.actorAttributesgroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ActorInfoPictureBox)).EndInit();
            this.tabPageRawVariable.ResumeLayout(false);
            this.Variables5TabPage.ResumeLayout(false);
            this.tabControlRawVarType.ResumeLayout(false);
            this.tabPageV5.ResumeLayout(false);
            this.tabPageV5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable4AInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable1Input)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable2Input)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable4BInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariable3Input)).EndInit();
            this.tabPageV6.ResumeLayout(false);
            this.tabPageV6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable2fInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable1Input)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable4Input)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable3Input)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable6Input)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorV6Variable5Input)).EndInit();
            this.groupBoxFullVariable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ActorVariableFullInput)).EndInit();
            this.tabPageFieldsVariable.ResumeLayout(false);
            this.groupBoxCoridinate.ResumeLayout(false);
            this.groupBoxCoridinate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActorXCoordInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorYCoordInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActorLayerInput)).EndInit();
            this.BottomGroupBox.ResumeLayout(false);
            this.BottomGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel tileSheetPanel;
        private System.Windows.Forms.Panel layersPanel;
        private System.Windows.Forms.PictureBox tileSheetPictureBox;
        private EFSAdvent.Controls.PictureBoxWithInterpolationMode layerPictureBox;
        private System.Windows.Forms.TextBox loggerTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.GroupBox rightSideGroupBox;
        private EFSAdvent.Controls.CheckedListBoxColorable layersCheckList;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PictureBox BrushTilePictureBox;
        private System.Windows.Forms.Label BrushTileLabel;
        private System.Windows.Forms.Button updateLayersButton;
        private System.Windows.Forms.ToolTip ActorAttributesTip;
        private System.Windows.Forms.CheckedListBox actorsCheckListBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox actorAttributesgroupBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox actorLayerComboBox;
        private System.Windows.Forms.ComboBox ActorNameComboBox;
        private System.Windows.Forms.PictureBox ActorInfoPictureBox;
        private System.Windows.Forms.Button actorDeleteButton;
        private System.Windows.Forms.Button buttonActorsSelectNone;
        private System.Windows.Forms.Button buttonSaveLayers;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ComboBox BrushSizeComboBox;
		private System.Windows.Forms.NumericUpDown ActorYCoordInput;
		private System.Windows.Forms.NumericUpDown ActorXCoordInput;
		private System.Windows.Forms.NumericUpDown ActorLayerInput;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xSizeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xSizeToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
		private System.Windows.Forms.Button cloneButton;
		private System.Windows.Forms.ToolStripMenuItem SaveAsMenuItem;
        private System.Windows.Forms.GroupBox groupBoxCoridinate;
        private System.Windows.Forms.ContextMenuStrip actorContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem pastToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureFilterModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bilinearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bicubicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nearestNeighborToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem roomImportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actorsImportToolStripMenuItem;
        private ToolStripMenuItem ExportMenuItem;
        private ToolStripMenuItem viewAspngToolStripMenuItem;
        private ToolStripMenuItem mapAspngToolStripMenuItem;
        private ToolStripMenuItem levelAsArcToolStripMenuItem;
        private ToolStripMenuItem wikiToolStripMenuItem;
        private ToolStripMenuItem sourceCodeToolStripMenuItem;
        private TextBox CoridinatesTextBox;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem roomAstsxToolStripMenuItem;
        private ToolStripMenuItem displayOverlayToolStripMenuItem;
        private ToolStripMenuItem ImportRoomFromTmx;
        private Label BrushSizeLabel;
        private ToolStripMenuItem actorsToolStripMenuItem;
        private ToolStripMenuItem autoSelectToolStripMenuItem;
        private ToolStripMenuItem alwaysShowActorsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem mapAndAAspngToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem allRoomsAspngToolStripMenuItem;
        private ToolStripMenuItem allRoomsAndActorsAspngToolStripMenuItem;
        private TabControl tabPageRawVariable;
        private TabPage Variables5TabPage;
        private NumericUpDown ActorVariable4AInput;
        private NumericUpDown ActorVariable4BInput;
        private Label label_V5_4;
        private Label label_V5_1;
        private Label label_V5_2;
        private NumericUpDown ActorVariable3Input;
        private NumericUpDown ActorVariable1Input;
        private NumericUpDown ActorVariable2Input;
        private Label label_V5_3a;
        private TabPage tabPageFieldsVariable;
        private GroupBox groupBoxFullVariable;
        private NumericUpDown ActorVariableFullInput;
        private TabControl tabControlRawVarType;
        private TabPage tabPageV5;
        private TabPage tabPageV6;
        private Label label_V5_3;
        private NumericUpDown ActorV6Variable2fInput;
        private NumericUpDown ActorV6Variable1Input;
        private NumericUpDown ActorV6Variable4Input;
        private NumericUpDown ActorV6Variable3Input;
        private NumericUpDown ActorV6Variable6Input;
        private NumericUpDown ActorV6Variable5Input;
        private Label label9;
        private Label label21;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label12;
        private FlowLayoutPanel panelActorFields;
        private Button MirrorBrushbutton;
        private ToolStripMenuItem automaticSetTileActorsToolStripMenuItem;
        private TabPage tabPage4;
        private TileStampFlowLayoutPanel TileStampFlowLayoutPanel;
        private Button DeleteStampButton;
        private Button SaveStampButton;
        private Button MirrorStampButton;
        private GroupBox BottomGroupBox;
        private TextBox RootFolderPathTextBox;
        private ToolStripMenuItem mirrorRoomToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private Controls.MapEditor MapEditor;
        private TabControl MapTabControl;
        private TabPage MapMultiplayerTabPage;
        private TabPage MapSinglelplayerTabPage;
        private MapEditor MapEditorSinglelplayer;
    }
}

