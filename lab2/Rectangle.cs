using System.Drawing;

namespace lab1
{
    public class Rectangle : Shape
    {
        public int Width;
        public int Height;
        public bool IsFilled;

        public Rectangle(int x, int y, int width, int height, Color color, 
            bool isfilled = false, int linewidth = 2) : base(x, y, color, linewidth)
        {
            Width = width;
            Height = height;
            IsFilled = isfilled;
        }

        public override void Accept(IDrawVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
