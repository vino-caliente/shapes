using System.Drawing;

namespace lab1
{
    public class Triangle : Shape
    {
        public Point Point2;
        public Point Point3;
        public bool IsFilled;

        public Triangle(Point p1, Point p2, Point p3, Color color, bool isfilled = false, 
            int lineWidth = 2) : base(p1.X, p1.Y, color, lineWidth)
        {
            Point2 = p2;
            Point3 = p3;
            IsFilled = isfilled;
        }

        public override void Draw(Graphics g)
        {
            Point[] points = { new Point(X, Y), Point2, Point3 };

            if (IsFilled)
            {
                Brush brush = new SolidBrush(Color);
                g.FillPolygon(brush, points);
            }
            else
            {
                Pen pen = new Pen(Color, LineWidth);
                g.DrawPolygon(pen, points);
            }
        }
    }
}
