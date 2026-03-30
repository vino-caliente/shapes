using System;
using System.Drawing;

namespace lab1
{
    public class DrawVisitor : IDrawVisitor
    {
        private Graphics g;

        public DrawVisitor(Graphics graphics)
        {
            g = graphics;
        }

        public void UpdateGraphics(Graphics graphics)
        {
            g = graphics;
        }

        public void Visit(Triangle tr)
        {
            Point[] points = { new Point(tr.X, tr.Y), tr.Point2, tr.Point3 };

            if (tr.IsFilled)
            {
                Brush brush = new SolidBrush(tr.Color);
                g.FillPolygon(brush, points);
            }
            else
            {
                Pen pen = new Pen(tr.Color, tr.LineWidth);
                g.DrawPolygon(pen, points);
            }
        }

        public void Visit(Rectangle rect)
        {
            if (rect.IsFilled)
            {
                Brush brush = new SolidBrush(rect.Color);
                g.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
            }
            else
            {
                Pen pen = new Pen(rect.Color, rect.LineWidth);
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        public void Visit(Square sq)
        {
            Visit(sq as Rectangle);
        }

        public void Visit(Line l)
        {
            Pen pen = new Pen(l.Color, l.LineWidth);
            g.DrawLine(pen, l.X, l.Y, l.EndX, l.EndY);
        }

        public void Visit(Ellipse ell)
        {
            if (ell.IsFilled)
            {
                Brush brush = new SolidBrush(ell.Color);
                g.FillEllipse(brush, ell.X, ell.Y, ell.Width, ell.Height);
            }
            else
            {
                Pen pen = new Pen(ell.Color, ell.LineWidth);
                g.DrawEllipse(pen, ell.X, ell.Y, ell.Width, ell.Height);
            }
        }

        public void Visit(Circle circ)
        {
            Visit(circ as Ellipse);
        }
    }
}
