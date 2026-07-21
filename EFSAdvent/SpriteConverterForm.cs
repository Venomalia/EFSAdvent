using AuroraLib.Pixel.BitmapExtension;
using AuroraLib.Pixel.Image;
using FSALib;
using FSALib.Renderer;
using System;
using System.Drawing;
using System.Windows.Forms;
using BGRA32 = AuroraLib.Pixel.PixelFormats.BGRA<byte>;

namespace EFSAdvent
{
    public partial class SpriteConverterForm : Form
    {
        readonly SpriteRenderer<BGRA32> SpriteRenderer;
        readonly Bitmap SpriteObjectBitmap;
        readonly Rarc DataArc;

        public SpriteConverterForm(Rarc dataArc)
        {
            DataArc = dataArc;
            InitializeComponent();

            // Initialize SpriteRenderer Renderer
            SpriteRenderer = new SpriteRenderer<BGRA32>(DataArc);
            SpriteObjectBitmap = new Bitmap(128, 128);
            SpritListUpDown.Maximum = SpriteRenderer.SpriteAttributeLists.Count - 1;
            SpritIndexUpDown.Maximum = SpriteRenderer.SpriteAttributeLists[(byte)SpritListUpDown.Value].Count - 1;
            spritePictureBox.Image = SpriteObjectBitmap;
            SpriteSheetUpDown.Value = 10; // <-- LoadTilesheet + DrawSprite
        }

        private void SaveSpriteButton_Click(object sender, EventArgs e)
        {
            var savePng = new SaveFileDialog
            {
                DefaultExt = "png",
                AddExtension = true,
                Filter = "Portable Network Graphics (*.png)|*.png",
                FileName = $"FSA{(int)SpritListUpDown.Value}_{(int)SpritIndexUpDown.Value:D4}.png"
            };

            if (savePng.ShowDialog() == DialogResult.OK)
            {
                SpriteObjectBitmap.Save(savePng.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void SpritListUpDown_ValueChanged(object sender, EventArgs e)
        {
            SpritIndexUpDown.Maximum = SpriteRenderer.SpriteAttributeLists[(byte)SpritListUpDown.Value].Count - 1;
            SpritIndexUpDown.Value = 0;
            DrawSprite(sender, e);
        }

        private void LoadTilesheet(object sender, EventArgs e)
        {
            SpriteRenderer.LoadTilesheet(DataArc, (int)SpriteSheetUpDown.Value, UseGBAPaletteCheckBox.Checked, UseEndingSheetCheckBox.Checked);
            DrawSprite(sender, e);
        }

        private void DrawSprite(object sender, EventArgs e)
        {
            using (var spriteImage = (MemoryImage<BGRA32>)SpriteObjectBitmap.AsAuroraImage())
            {
                spriteImage.Pixel.Clear();
                SpriteRenderer.DrawSprite(spriteImage, 64, 64, (ushort)SpritIndexUpDown.Value, (ushort)SpritListUpDown.Value, (sbyte)ReplacePaletteUpDown.Value, (sbyte)TargetPalletUpDown.Value);
            }
            spritePictureBox.Refresh();
        }
    }
}
