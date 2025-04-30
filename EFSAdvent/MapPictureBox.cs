using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace EFSAdvent
{
    public class MapPictureBox : PictureBox
    {
        private readonly Bitmap mapBitmap;
        private readonly Graphics mapGraphics;

        private (int X, int Y) selectedRoomCoordinates;

        private FSALib.Map Map;

        public Brush NumberBrush;
        public Brush StartRoomBrush;
        public Brush SelectedRoomBrush;
        public Font NumberFront;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public (int X, int Y) SelectedRoomCoordinates
        {
            get => selectedRoomCoordinates;
            set
            {
                selectedRoomCoordinates = value;
                DrawMap();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedRoomID
        {
            get
            {
                if (Map == null)
                    throw new ArgumentNullException(nameof(Map));

                return selectedRoomCoordinates.X == -1 ? -1 : Map[selectedRoomCoordinates.X, selectedRoomCoordinates.Y];
            }

            set
            {
                if (selectedRoomCoordinates.X == -1 || selectedRoomCoordinates.Y == -1)
                    throw new InvalidOperationException("No room is currently selected.");

                if (Map == null)
                    throw new ArgumentNullException(nameof(Map));

                Map[selectedRoomCoordinates.X, selectedRoomCoordinates.Y] = value;
            }
        }

        public MapPictureBox()
        {
            selectedRoomCoordinates = (-1, -1);
            SelectedRoomBrush = Brushes.DarkGreen;
            StartRoomBrush = Brushes.Crimson;
            NumberBrush = Brushes.White;
            NumberFront = new Font("Microsoft Sans Serif", 11);

            SizeMode = PictureBoxSizeMode.StretchImage;
            mapBitmap = new Bitmap(32 * 10, 24 * 10, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            mapGraphics = Graphics.FromImage(mapBitmap); 
            mapGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Image = mapBitmap;
        }

        public void SetMap(FSALib.Map map)
        {
            if (Map != null)
                Map.PropertyChanged -= Map_PropertyChanged;

            Map = map;

            Map.PropertyChanged += Map_PropertyChanged;
            DrawMap();
        }

        private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e) => DrawMap();

        private unsafe void DrawMap()
        {
            if (Map == null)
                return;
            DrawMap_Layout();
            DrawMap_Numbers();
            Refresh();
        }

        private unsafe void DrawMap_Layout()
        {
            byte roomColour;
            int roomValue;
            int roomWidthInPixels = mapBitmap.Width / Map.XDimension;
            int roomHeightInPixels = mapBitmap.Height / Map.YDimension;

            var bitmapLock = mapBitmap.LockBits(new Rectangle(0, 0, mapBitmap.Width, mapBitmap.Height), ImageLockMode.WriteOnly, mapBitmap.PixelFormat);
            for (int y = 0; y < Map.YDimension * roomHeightInPixels; y += roomHeightInPixels)
            {
                for (int x = 0; x < Map.XDimension * roomWidthInPixels; x += roomWidthInPixels)
                {
                    roomValue = Map[x / roomWidthInPixels, y / roomHeightInPixels];
                    roomColour = (byte)(roomValue == FSALib.Map.EMPTY_ROOM_VALUE ? 0xFF : 0x00);
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
        }

        private void DrawMap_Numbers()
        {
            int roomValue;
            int roomWidthInPixels = mapBitmap.Width / Map.XDimension;
            int roomHeightInPixels = mapBitmap.Height / Map.YDimension;

            // Selected Room
            if (selectedRoomCoordinates.X != -1 && selectedRoomCoordinates.Y != -1)
            {
                mapGraphics.FillRectangle(SelectedRoomBrush,
                    SelectedRoomCoordinates.X * roomWidthInPixels,
                    SelectedRoomCoordinates.Y * roomHeightInPixels,
                    roomWidthInPixels,
                    roomHeightInPixels);
            }

            // Numbers
            for (int y = 0; y < Map.YDimension * roomHeightInPixels; y += roomHeightInPixels)
            {
                for (int x = 0; x < Map.XDimension * roomWidthInPixels; x += roomWidthInPixels)
                {
                    roomValue = Map[x / roomWidthInPixels, y / roomHeightInPixels];
                    if (roomValue != FSALib.Map.EMPTY_ROOM_VALUE)
                    {
                        //Draw the room number over the top of the room for clarity
                        mapGraphics.DrawString(Convert.ToString(roomValue), NumberFront, NumberBrush, x, y);
                    }
                }
            }

            // Start Room Number
            roomValue = Map[Map.StartX, Map.StartY];
            mapGraphics.DrawString(Convert.ToString(roomValue), NumberFront, StartRoomBrush, Map.StartX * roomWidthInPixels, Map.StartY * roomHeightInPixels);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NumberBrush?.Dispose();
                SelectedRoomBrush?.Dispose();
                StartRoomBrush?.Dispose();
                mapGraphics?.Dispose();
                mapBitmap?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
