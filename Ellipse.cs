using System.Drawing;

namespace lab1
{
    public class Ellipse : Shape
    {
        public int Width;
        public int Height;
        public bool IsFilled;

        public Ellipse(int x, int y, int width, int height, Color color, 
            bool isfilled = false, int lineWidth = 2) : base(x, y, color, lineWidth)
        {
            Width = width;
            Height = height;
            IsFilled = isfilled;
        }

        public override void Draw(Graphics g)
        {
            if (IsFilled)
            {
                Brush brush = new SolidBrush(Color);
                g.FillEllipse(brush, X, Y, Width, Height);
            }
            else
            {
                Pen pen = new Pen(Color, LineWidth);
                g.DrawEllipse(pen, X, Y, Width, Height);
            }
        }
    }
}
