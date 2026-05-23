using System.Drawing;

namespace StarPlugin
{
    public class Star : lab1.Shape
    {
        public int OuterRadius { get; set; }
        public int InnerRadius { get; set; }
        public int Points { get; set; }
        public bool IsFilled { get; set; }

        public Star(int centerX, int centerY, int outerRadius, int innerRadius,
                    int points, Color color, bool isFilled, int lineWidth)
            : base(centerX, centerY, color, lineWidth)
        {
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            Points = points;
            IsFilled = isFilled;
        }

        public override void Accept(lab1.IDrawVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}