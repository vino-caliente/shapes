using System.Drawing;

namespace lab1
{
    public class Line : Shape
    {
        public int EndX;
        public int EndY;

        public Line (int startX, int startY, int endX, int endY, Color color, 
            int lineWidth = 2) : base (startX, startY, color, lineWidth)
        {
            EndX = endX;
            EndY = endY;
        }

        public override void Accept(IDrawVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
