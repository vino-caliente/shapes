using System.Drawing;

namespace lab1
{
    public abstract class Shape
    {
        public int X;
        public int Y;
        public Color Color;
        public int LineWidth;

        protected Shape(int x, int y, Color color, int lineWidth = 2)
        {
            X = x;
            Y = y;
            Color = color;
            LineWidth = lineWidth;
        }

        public virtual void Draw (Graphics g) { }
    }
}
