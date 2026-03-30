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

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(Color, LineWidth);
            g.DrawLine(pen, X, Y, EndX, EndY);
        }
    }
}
