using System.Drawing;

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

        public static void DrawRectangleWithDropShadow(this Graphics graphics, Color color, int x, int y, int width, int height)
        {
            DefaultPen.Color = DropShadow;
            graphics.DrawRectangle(DefaultPen, x + 1, y + 1, width, height);
            DefaultPen.Color = color;
            graphics.DrawRectangle(DefaultPen, x, y, width, height);
        }

        public static void DrawLineWithDropShadow(this Graphics graphics, Color color, Point p1, Point p2)
        {
            DefaultPen.Color = DropShadow;
            graphics.DrawLine(DefaultPen, p1, p2);
            DefaultPen.Color = color;
            graphics.DrawLine(DefaultPen, p1, p2);
        }

        public static void DrawCircleWithDropShadow(this Graphics graphics, Color color, Point center, int size)
        {
            DefaultPen.Color = DropShadow;
            graphics.DrawEllipse(DefaultPen, center.X + 1 - size, center.Y + 1 - size, size * 2, size * 2);
            DefaultPen.Color = color;
            graphics.DrawEllipse(DefaultPen, center.X - size, center.Y - size, size * 2, size * 2);
        }

        public static Point MovePointInDirection(this Point source, Direction direction, int width)
        {
            switch (direction)
            {
                case Direction.East:
                    return new Point(source.X + width, source.Y);
                case Direction.West:
                    return new Point(source.X - width, source.Y);
                case Direction.North:
                    return new Point(source.X, source.Y - width);
                case Direction.South:
                    return new Point(source.X, source.Y + width);
                default:
                    return source;
            }
        }
    }
}
