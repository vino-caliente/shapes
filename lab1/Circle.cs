using System.Drawing;

namespace lab1
{
    public class Circle : Ellipse
    {
        public int Radius;

        public Circle(int centerX, int centerY, int radius, Color color, 
            bool isfilled = false, int lineWidth = 2) : base(centerX - radius, 
            centerY - radius, radius * 2, radius * 2, color, isfilled, lineWidth)
        {
            Radius = radius;
        }

        // Draw() без изменений наследуется от Ellipse
    }
}
