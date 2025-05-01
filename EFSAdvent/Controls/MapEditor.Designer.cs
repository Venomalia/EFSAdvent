using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFSAdvent.Controls
{
    public sealed partial class MapEditor : UserControl
    {
        private GroupBox MapVariablesGroupBox;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox RoomLayoutgroupBox1;
        private Panel panel1;
        private ComboBox MapVariableMusicComboBox;
        private Label label11;
        private NumericUpDown MapVariableNPCSheetID;
        private NumericUpDown MapVariableOverlay;
        private NumericUpDown MapVariableUnknown2;
        private NumericUpDown MapVariableDisallowTingle;
        private NumericUpDown MapVariableTileSheet;
        private NumericUpDown MapVariableE3Banner;
        private NumericUpDown MapVariableStartY;
        private NumericUpDown MapVariableStartX;
        private Label label20;
        private Label label19;
        private Label label18;
        private Label label17;
        private Label csvLabel4;
        private Label label10;
        private Label label16;
        private Label label3;
        private MapPictureBox mapPictureBox;
        private Button MapRoomLoadButton;
        private Label label5;
        private Button MapRoomNewButton;
        private Button MapRoomSetButton;
        private NumericUpDown MapRoomNumberInput;
        private Button MapRoomRemoveButton;
        private Button MapSaveButton;


        private void InitializeComponent()
        {
            this.MapVariablesGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.RoomLayoutgroupBox1 = new System.Windows.Forms.GroupBox();
            this.mapPictureBox = new EFSAdvent.MapPictureBox();
            this.MapRoomLoadButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.MapRoomNewButton = new System.Windows.Forms.Button();
            this.MapRoomSetButton = new System.Windows.Forms.Button();
            this.MapRoomNumberInput = new System.Windows.Forms.NumericUpDown();
            this.MapRoomRemoveButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.MapVariableMusicComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.MapVariableNPCSheetID = new System.Windows.Forms.NumericUpDown();
            this.MapVariableOverlay = new System.Windows.Forms.NumericUpDown();
            this.MapVariableUnknown2 = new System.Windows.Forms.NumericUpDown();
            this.MapVariableDisallowTingle = new System.Windows.Forms.NumericUpDown();
            this.MapVariableTileSheet = new System.Windows.Forms.NumericUpDown();
            this.MapVariableE3Banner = new System.Windows.Forms.NumericUpDown();
            this.MapVariableStartY = new System.Windows.Forms.NumericUpDown();
            this.MapVariableStartX = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.csvLabel4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.MapSaveButton = new System.Windows.Forms.Button();
            this.MapVariablesGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.RoomLayoutgroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapRoomNumberInput)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableNPCSheetID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableOverlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableUnknown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableDisallowTingle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableTileSheet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableE3Banner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableStartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableStartX)).BeginInit();
            this.SuspendLayout();
            // 
            // MapVariablesGroupBox
            // 
            this.MapVariablesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapVariablesGroupBox.Controls.Add(this.tableLayoutPanel1);
            this.MapVariablesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.MapVariablesGroupBox.Name = "MapVariablesGroupBox";
            this.MapVariablesGroupBox.Size = new System.Drawing.Size(267, 445);
            this.MapVariablesGroupBox.TabIndex = 14;
            this.MapVariablesGroupBox.TabStop = false;
            this.MapVariablesGroupBox.Text = "Map";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.RoomLayoutgroupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MapSaveButton, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.50914F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64.49086F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(267, 426);
            this.tableLayoutPanel1.TabIndex = 33;
            // 
            // RoomLayoutgroupBox1
            // 
            this.RoomLayoutgroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RoomLayoutgroupBox1.Controls.Add(this.mapPictureBox);
            this.RoomLayoutgroupBox1.Controls.Add(this.MapRoomLoadButton);
            this.RoomLayoutgroupBox1.Controls.Add(this.label5);
            this.RoomLayoutgroupBox1.Controls.Add(this.MapRoomNewButton);
            this.RoomLayoutgroupBox1.Controls.Add(this.MapRoomSetButton);
            this.RoomLayoutgroupBox1.Controls.Add(this.MapRoomNumberInput);
            this.RoomLayoutgroupBox1.Controls.Add(this.MapRoomRemoveButton);
            this.RoomLayoutgroupBox1.Location = new System.Drawing.Point(3, 139);
            this.RoomLayoutgroupBox1.Name = "RoomLayoutgroupBox1";
            this.RoomLayoutgroupBox1.Size = new System.Drawing.Size(261, 241);
            this.RoomLayoutgroupBox1.TabIndex = 33;
            this.RoomLayoutgroupBox1.TabStop = false;
            this.RoomLayoutgroupBox1.Text = "Layout of Rooms in Map";
            // 
            // mapPictureBox
            // 
            this.mapPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapPictureBox.Location = new System.Drawing.Point(9, 70);
            this.mapPictureBox.Name = "mapPictureBox";
            this.mapPictureBox.Size = new System.Drawing.Size(240, 170);
            this.mapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.mapPictureBox.TabIndex = 0;
            this.mapPictureBox.TabStop = false;
            this.mapPictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MapPictureBox_MouseDoubleClick);
            this.mapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapPictureBox_MouseDown);
            this.mapPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapPictureBox_MouseUp);
            // 
            // MapRoomLoadButton
            // 
            this.MapRoomLoadButton.Enabled = false;
            this.MapRoomLoadButton.Location = new System.Drawing.Point(183, 44);
            this.MapRoomLoadButton.Name = "MapRoomLoadButton";
            this.MapRoomLoadButton.Size = new System.Drawing.Size(65, 20);
            this.MapRoomLoadButton.TabIndex = 11;
            this.MapRoomLoadButton.Text = "Load";
            this.MapRoomLoadButton.UseVisualStyleBackColor = true;
            this.MapRoomLoadButton.Click += new System.EventHandler(this.MapRoomLoadButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Selected Room";
            // 
            // MapRoomNewButton
            // 
            this.MapRoomNewButton.Enabled = false;
            this.MapRoomNewButton.Location = new System.Drawing.Point(112, 18);
            this.MapRoomNewButton.Name = "MapRoomNewButton";
            this.MapRoomNewButton.Size = new System.Drawing.Size(65, 20);
            this.MapRoomNewButton.TabIndex = 29;
            this.MapRoomNewButton.Text = "New";
            this.MapRoomNewButton.UseVisualStyleBackColor = true;
            this.MapRoomNewButton.Click += new System.EventHandler(this.MapRoomNewButton_Click);
            // 
            // MapRoomSetButton
            // 
            this.MapRoomSetButton.Enabled = false;
            this.MapRoomSetButton.Location = new System.Drawing.Point(112, 44);
            this.MapRoomSetButton.Name = "MapRoomSetButton";
            this.MapRoomSetButton.Size = new System.Drawing.Size(65, 20);
            this.MapRoomSetButton.TabIndex = 10;
            this.MapRoomSetButton.Text = "Set";
            this.MapRoomSetButton.UseVisualStyleBackColor = true;
            this.MapRoomSetButton.Click += new System.EventHandler(this.MapRoomSetButton_Click);
            // 
            // MapRoomNumberInput
            // 
            this.MapRoomNumberInput.Location = new System.Drawing.Point(8, 44);
            this.MapRoomNumberInput.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.MapRoomNumberInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.MapRoomNumberInput.Name = "MapRoomNumberInput";
            this.MapRoomNumberInput.Size = new System.Drawing.Size(99, 20);
            this.MapRoomNumberInput.TabIndex = 19;
            this.MapRoomNumberInput.ValueChanged += new System.EventHandler(this.MapRoomNumberInput_ValueChanged);
            // 
            // MapRoomRemoveButton
            // 
            this.MapRoomRemoveButton.Enabled = false;
            this.MapRoomRemoveButton.Location = new System.Drawing.Point(183, 18);
            this.MapRoomRemoveButton.Name = "MapRoomRemoveButton";
            this.MapRoomRemoveButton.Size = new System.Drawing.Size(65, 20);
            this.MapRoomRemoveButton.TabIndex = 30;
            this.MapRoomRemoveButton.Text = "Remove";
            this.MapRoomRemoveButton.UseVisualStyleBackColor = false;
            this.MapRoomRemoveButton.Click += new System.EventHandler(this.MapRoomRemoveButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.MapVariableMusicComboBox);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.MapVariableNPCSheetID);
            this.panel1.Controls.Add(this.MapVariableOverlay);
            this.panel1.Controls.Add(this.MapVariableUnknown2);
            this.panel1.Controls.Add(this.MapVariableDisallowTingle);
            this.panel1.Controls.Add(this.MapVariableTileSheet);
            this.panel1.Controls.Add(this.MapVariableE3Banner);
            this.panel1.Controls.Add(this.MapVariableStartY);
            this.panel1.Controls.Add(this.MapVariableStartX);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.csvLabel4);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(267, 136);
            this.panel1.TabIndex = 32;
            // 
            // MapVariableMusicComboBox
            // 
            this.MapVariableMusicComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MapVariableMusicComboBox.FormattingEnabled = true;
            this.MapVariableMusicComboBox.Location = new System.Drawing.Point(84, 107);
            this.MapVariableMusicComboBox.Name = "MapVariableMusicComboBox";
            this.MapVariableMusicComboBox.Size = new System.Drawing.Size(177, 21);
            this.MapVariableMusicComboBox.TabIndex = 35;
            this.MapVariableMusicComboBox.SelectedIndexChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 110);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 13);
            this.label11.TabIndex = 34;
            this.label11.Text = "BG Music:";
            // 
            // MapVariableNPCSheetID
            // 
            this.MapVariableNPCSheetID.Location = new System.Drawing.Point(215, 55);
            this.MapVariableNPCSheetID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MapVariableNPCSheetID.Name = "MapVariableNPCSheetID";
            this.MapVariableNPCSheetID.Size = new System.Drawing.Size(46, 20);
            this.MapVariableNPCSheetID.TabIndex = 44;
            this.MapVariableNPCSheetID.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // MapVariableOverlay
            // 
            this.MapVariableOverlay.Location = new System.Drawing.Point(84, 55);
            this.MapVariableOverlay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MapVariableOverlay.Name = "MapVariableOverlay";
            this.MapVariableOverlay.Size = new System.Drawing.Size(46, 20);
            this.MapVariableOverlay.TabIndex = 43;
            this.MapVariableOverlay.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // MapVariableUnknown2
            // 
            this.MapVariableUnknown2.Location = new System.Drawing.Point(215, 81);
            this.MapVariableUnknown2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MapVariableUnknown2.Name = "MapVariableUnknown2";
            this.MapVariableUnknown2.Size = new System.Drawing.Size(46, 20);
            this.MapVariableUnknown2.TabIndex = 42;
            this.MapVariableUnknown2.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // MapVariableDisallowTingle
            // 
            this.MapVariableDisallowTingle.Location = new System.Drawing.Point(84, 81);
            this.MapVariableDisallowTingle.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MapVariableDisallowTingle.Name = "MapVariableDisallowTingle";
            this.MapVariableDisallowTingle.Size = new System.Drawing.Size(46, 20);
            this.MapVariableDisallowTingle.TabIndex = 41;
            this.MapVariableDisallowTingle.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // MapVariableTileSheet
            // 
            this.MapVariableTileSheet.Location = new System.Drawing.Point(84, 29);
            this.MapVariableTileSheet.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MapVariableTileSheet.Name = "MapVariableTileSheet";
            this.MapVariableTileSheet.Size = new System.Drawing.Size(46, 20);
            this.MapVariableTileSheet.TabIndex = 40;
            this.MapVariableTileSheet.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // MapVariableE3Banner
            // 
            this.MapVariableE3Banner.Location = new System.Drawing.Point(215, 29);
            this.MapVariableE3Banner.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MapVariableE3Banner.Name = "MapVariableE3Banner";
            this.MapVariableE3Banner.Size = new System.Drawing.Size(46, 20);
            this.MapVariableE3Banner.TabIndex = 39;
            this.MapVariableE3Banner.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // MapVariableStartY
            // 
            this.MapVariableStartY.Location = new System.Drawing.Point(215, 3);
            this.MapVariableStartY.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.MapVariableStartY.Name = "MapVariableStartY";
            this.MapVariableStartY.Size = new System.Drawing.Size(46, 20);
            this.MapVariableStartY.TabIndex = 38;
            this.MapVariableStartY.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // MapVariableStartX
            // 
            this.MapVariableStartX.Location = new System.Drawing.Point(84, 3);
            this.MapVariableStartX.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.MapVariableStartX.Name = "MapVariableStartX";
            this.MapVariableStartX.Size = new System.Drawing.Size(46, 20);
            this.MapVariableStartX.TabIndex = 37;
            this.MapVariableStartX.ValueChanged += new System.EventHandler(this.MapVariable_Changed);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 83);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(81, 13);
            this.label20.TabIndex = 36;
            this.label20.Text = "Disallow Tingle:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(136, 86);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(56, 13);
            this.label19.TabIndex = 35;
            this.label19.Text = "Unknown:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 58);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(46, 13);
            this.label18.TabIndex = 34;
            this.label18.Text = "Overlay:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(136, 57);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 13);
            this.label17.TabIndex = 33;
            this.label17.Text = "NPC Sheet:";
            // 
            // csvLabel4
            // 
            this.csvLabel4.AutoSize = true;
            this.csvLabel4.Location = new System.Drawing.Point(139, 31);
            this.csvLabel4.Name = "csvLabel4";
            this.csvLabel4.Size = new System.Drawing.Size(60, 13);
            this.csvLabel4.TabIndex = 32;
            this.csvLabel4.Text = "E3 Banner:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 31);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Tile Sheet:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(136, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(73, 13);
            this.label16.TabIndex = 30;
            this.label16.Text = "Start Room Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Start Room X:";
            // 
            // MapSaveButton
            // 
            this.MapSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapSaveButton.Enabled = false;
            this.MapSaveButton.Location = new System.Drawing.Point(3, 386);
            this.MapSaveButton.Name = "MapSaveButton";
            this.MapSaveButton.Size = new System.Drawing.Size(261, 37);
            this.MapSaveButton.TabIndex = 13;
            this.MapSaveButton.Text = "Save changes to Map";
            this.MapSaveButton.UseVisualStyleBackColor = true;
            this.MapSaveButton.Click += new System.EventHandler(this.MapSaveButton_Click);
            // 
            // MapEditor
            // 
            this.Controls.Add(this.MapVariablesGroupBox);
            this.Name = "MapEditor";
            this.Size = new System.Drawing.Size(267, 445);
            this.MapVariablesGroupBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.RoomLayoutgroupBox1.ResumeLayout(false);
            this.RoomLayoutgroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapRoomNumberInput)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableNPCSheetID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableOverlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableUnknown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableDisallowTingle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableTileSheet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableE3Banner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableStartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapVariableStartX)).EndInit();
            this.ResumeLayout(false);

        }

    }
}
