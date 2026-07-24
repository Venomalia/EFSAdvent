using System.Drawing;
using System.Drawing.Drawing2D;

namespace EFSAdvent
{
    internal static class GraphicsEX
    {
        public enum Direction
        {
            East = 0,
            West = 1,
            North = 2,
            South = 3
        }

        public static readonly Color DropShadow = Color.FromArgb(128, 0, 0, 0);

        private static readonly Pen DefaultPen = new Pen(DropShadow);
        private static readonly SolidBrush DefaultBrush = new SolidBrush(DropShadow);
        private static readonly Font DefaultFront = new Font("Microsoft Sans Serif", 7);

        public static void DrawString(this Graphics graphics, Color color, string text, int x, int y, DashStyle style = DashStyle.Solid)
        {
            DefaultPen.DashStyle = style;
            DefaultBrush.Color = DropShadow;
            graphics.DrawString(text, DefaultFront, DefaultBrush, x + 1, y + 1);
            DefaultBrush.Color = color;
            graphics.DrawString(text, DefaultFront, DefaultBrush, x, y);
        }

        public static void FillRectangle(this Graphics graphics, Color color, int x, int y, int width, int height, DashStyle style = DashStyle.Solid)
        {
            DefaultPen.DashStyle = style;
            DefaultBrush.Color = color;
            graphics.FillRectangle(DefaultBrush, x, y, width, height);
        }

        public static void DrawRectangleWithDropShadow(this Graphics graphics, Color color, int x, int y, int width, int height, DashStyle style = DashStyle.Solid)
        {
            DefaultPen.DashStyle = style;
            DefaultPen.Color = DropShadow;
            graphics.DrawRectangle(DefaultPen, x + 1, y + 1, width, height);
            DefaultPen.Color = color;
            graphics.DrawRectangle(DefaultPen, x, y, width, height);
        }

        public static void DrawLineWithDropShadow(this Graphics graphics, Color color, Point p1, Point p2, DashStyle style = DashStyle.Solid)
        {
            DefaultPen.DashStyle = style;
            DefaultPen.Color = DropShadow;
            graphics.DrawLine(DefaultPen, p1, p2);
            DefaultPen.Color = color;
            graphics.DrawLine(DefaultPen, p1, p2);
        }

        public static void DrawCircleWithDropShadow(this Graphics graphics, Color color, Point center, int size, DashStyle style = DashStyle.Solid)
        {
            DefaultPen.DashStyle = style;
            DefaultPen.Color = DropShadow;
            graphics.DrawEllipse(DefaultPen, center.X + 1 - size, center.Y + 1 - size, size * 2, size * 2);
            DefaultPen.Color = color;
            graphics.DrawEllipse(DefaultPen, center.X - size, center.Y - size, size * 2, size * 2);
        }

        public static void Clear(this Bitmap bitmap, Color color)
        {
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(color);
        }

        public static Point MovePointInDirection(this Point source, Direction direction, int width)
        {
            return direction switch
            {
                Direction.East => new Point(source.X + width, source.Y),
                Direction.West => new Point(source.X - width, source.Y),
                Direction.North => new Point(source.X, source.Y - width),
                Direction.South => new Point(source.X, source.Y + width),
                _ => source,
            };
        }
    }
}
