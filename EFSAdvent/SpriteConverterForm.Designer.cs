namespace EFSAdvent
{
    partial class SpriteConverterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpriteConverterForm));
            this.spritePictureBox = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ReplacePaletteUpDown = new System.Windows.Forms.NumericUpDown();
            this.TargetPalletUpDown = new System.Windows.Forms.NumericUpDown();
            this.SaveSpriteButton = new System.Windows.Forms.Button();
            this.SpritIndexUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SpritListUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SpriteSheetUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.UseEndingSheetCheckBox = new System.Windows.Forms.CheckBox();
            this.UseGBAPaletteCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.spritePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReplacePaletteUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetPalletUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpritIndexUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpritListUpDown)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SpriteSheetUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // spritePictureBox
            // 
            this.spritePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spritePictureBox.Location = new System.Drawing.Point(228, 12);
            this.spritePictureBox.Name = "spritePictureBox";
            this.spritePictureBox.Size = new System.Drawing.Size(128, 128);
            this.spritePictureBox.TabIndex = 18;
            this.spritePictureBox.TabStop = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(148, 26);
            this.label5.TabIndex = 27;
            this.label5.Text = "Sprite List:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ReplacePaletteUpDown
            // 
            this.ReplacePaletteUpDown.Location = new System.Drawing.Point(158, 82);
            this.ReplacePaletteUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.ReplacePaletteUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.ReplacePaletteUpDown.Name = "ReplacePaletteUpDown";
            this.ReplacePaletteUpDown.Size = new System.Drawing.Size(48, 20);
            this.ReplacePaletteUpDown.TabIndex = 34;
            this.ReplacePaletteUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.ReplacePaletteUpDown.ValueChanged += new System.EventHandler(this.DrawSprite);
            // 
            // TargetPalletUpDown
            // 
            this.TargetPalletUpDown.Location = new System.Drawing.Point(158, 108);
            this.TargetPalletUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.TargetPalletUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.TargetPalletUpDown.Name = "TargetPalletUpDown";
            this.TargetPalletUpDown.Size = new System.Drawing.Size(48, 20);
            this.TargetPalletUpDown.TabIndex = 32;
            this.TargetPalletUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.TargetPalletUpDown.ValueChanged += new System.EventHandler(this.DrawSprite);
            // 
            // SaveSpriteButton
            // 
            this.SaveSpriteButton.Location = new System.Drawing.Point(228, 146);
            this.SaveSpriteButton.Name = "SaveSpriteButton";
            this.SaveSpriteButton.Size = new System.Drawing.Size(128, 43);
            this.SaveSpriteButton.TabIndex = 35;
            this.SaveSpriteButton.Text = "Save Sprite";
            this.SaveSpriteButton.UseVisualStyleBackColor = true;
            this.SaveSpriteButton.Click += new System.EventHandler(this.SaveSpriteButton_Click);
            // 
            // SpritIndexUpDown
            // 
            this.SpritIndexUpDown.Location = new System.Drawing.Point(158, 30);
            this.SpritIndexUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.SpritIndexUpDown.Name = "SpritIndexUpDown";
            this.SpritIndexUpDown.Size = new System.Drawing.Size(48, 20);
            this.SpritIndexUpDown.TabIndex = 26;
            this.SpritIndexUpDown.ValueChanged += new System.EventHandler(this.DrawSprite);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 26);
            this.label1.TabIndex = 36;
            this.label1.Text = "Sprite Index:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 26);
            this.label2.TabIndex = 37;
            this.label2.Text = "Replacement Palette:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 26);
            this.label3.TabIndex = 38;
            this.label3.Text = "Target Palette:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SpritListUpDown
            // 
            this.SpritListUpDown.Location = new System.Drawing.Point(158, 4);
            this.SpritListUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.SpritListUpDown.Name = "SpritListUpDown";
            this.SpritListUpDown.Size = new System.Drawing.Size(48, 20);
            this.SpritListUpDown.TabIndex = 39;
            this.SpritListUpDown.ValueChanged += new System.EventHandler(this.SpritListUpDown_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.SpriteSheetUpDown, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.UseEndingSheetCheckBox, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SpritListUpDown, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SpritIndexUpDown, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.TargetPalletUpDown, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ReplacePaletteUpDown, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.UseGBAPaletteCheckBox, 0, 6);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(210, 177);
            this.tableLayoutPanel1.TabIndex = 40;
            // 
            // SpriteSheetUpDown
            // 
            this.SpriteSheetUpDown.Location = new System.Drawing.Point(158, 56);
            this.SpriteSheetUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.SpriteSheetUpDown.Name = "SpriteSheetUpDown";
            this.SpriteSheetUpDown.Size = new System.Drawing.Size(48, 20);
            this.SpriteSheetUpDown.TabIndex = 41;
            this.SpriteSheetUpDown.ValueChanged += new System.EventHandler(this.LoadTilesheet);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 26);
            this.label4.TabIndex = 43;
            this.label4.Text = "Sprite Sheet Index:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UseEndingSheetCheckBox
            // 
            this.UseEndingSheetCheckBox.AutoSize = true;
            this.UseEndingSheetCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UseEndingSheetCheckBox.Location = new System.Drawing.Point(4, 134);
            this.UseEndingSheetCheckBox.Name = "UseEndingSheetCheckBox";
            this.UseEndingSheetCheckBox.Size = new System.Drawing.Size(148, 17);
            this.UseEndingSheetCheckBox.TabIndex = 42;
            this.UseEndingSheetCheckBox.Text = "Use Ending Sprite Sheet.";
            this.UseEndingSheetCheckBox.UseVisualStyleBackColor = true;
            this.UseEndingSheetCheckBox.CheckedChanged += new System.EventHandler(this.LoadTilesheet);
            // 
            // UseGBAPaletteCheckBox
            // 
            this.UseGBAPaletteCheckBox.AutoSize = true;
            this.UseGBAPaletteCheckBox.Location = new System.Drawing.Point(4, 157);
            this.UseGBAPaletteCheckBox.Name = "UseGBAPaletteCheckBox";
            this.UseGBAPaletteCheckBox.Size = new System.Drawing.Size(109, 17);
            this.UseGBAPaletteCheckBox.TabIndex = 41;
            this.UseGBAPaletteCheckBox.Text = "Use GBA Palette.";
            this.UseGBAPaletteCheckBox.UseVisualStyleBackColor = true;
            this.UseGBAPaletteCheckBox.CheckedChanged += new System.EventHandler(this.LoadTilesheet);
            // 
            // SpriteConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 194);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.SaveSpriteButton);
            this.Controls.Add(this.spritePictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SpriteConverterForm";
            this.Text = "Sprite Converter";
            ((System.ComponentModel.ISupportInitialize)(this.spritePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReplacePaletteUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetPalletUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpritIndexUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpritListUpDown)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SpriteSheetUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox spritePictureBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown ReplacePaletteUpDown;
        private System.Windows.Forms.NumericUpDown TargetPalletUpDown;
        private System.Windows.Forms.Button SaveSpriteButton;
        private System.Windows.Forms.NumericUpDown SpritIndexUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown SpritListUpDown;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox UseGBAPaletteCheckBox;
        private System.Windows.Forms.CheckBox UseEndingSheetCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown SpriteSheetUpDown;
    }
}