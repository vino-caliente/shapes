using System.Drawing;

namespace lab1
{
    public class Square : Rectangle
    {
        public int Side;

        public Square(int x, int y, int side, Color color, bool isfilled = false, int lineWidth = 2)
            : base(x, y, side, side, color, isfilled, lineWidth)
        {
            Side = side;
        }

        // Draw() без изменений наследуется от Rectangle
    }
}
