using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using lab1;

namespace StarPlugin
{
    public class StarPlugin : IShapePlugin
    {
        public string PluginName => "Star Plugin";
        public string PluginVersion => "1.0.0";
        public string ShapeName => "Star";

        public Type GetShapeType() => typeof(Star);

        public void RegisterDrawMethod(DrawVisitor visitor)
        {
            System.Diagnostics.Debug.WriteLine("StarPlugin.RegisterDrawMethod called!");

            visitor.RegisterDrawMethod<Star>((star, g) =>
            {
                System.Diagnostics.Debug.WriteLine($"Drawing Star at ({star.X}, {star.Y})");
                Point[] points = CalculateStarPoints(star);

                if (star.IsFilled)
                {
                    using (Brush brush = new SolidBrush(star.Color))
                        g.FillPolygon(brush, points);
                }
                else
                {
                    using (Pen pen = new Pen(star.Color, star.LineWidth))
                        g.DrawPolygon(pen, points);
                }
            });

            System.Diagnostics.Debug.WriteLine("Star draw method registered!");
        }

        public Shape CreateRandomShape(int centerX, int centerY, Random random)
        {
            return new Star(
                centerX, centerY,
                random.Next(15, 45),  // outer radius
                random.Next(5, 25),   // inner radius
                random.Next(5, 9),    // points (5 to 8)
                Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)),
                random.Next(2) == 0,  // isFilled
                random.Next(1, 8)     // lineWidth
            );
        }

        public bool ShowParameterDialog(out Shape shape, Point location)
        {
            // Optional: custom dialog
            shape = null;
            return false;
        }

        public void OnLoad(MainForm mainForm)
        {
            // Optional: add menu items, toolbars, etc.
        }

        private Point[] CalculateStarPoints(Star star)
        {
            List<Point> points = new List<Point>();
            double angleStep = Math.PI / star.Points;

            for (int i = 0; i < star.Points * 2; i++)
            {
                double radius = (i % 2 == 0) ? star.OuterRadius : star.InnerRadius;
                double angle = i * angleStep - Math.PI / 2;
                // Перевод из полярных координат (радиус, угол) в декартовы (x, y)
                int x = star.X + (int)(radius * Math.Cos(angle));
                int y = star.Y + (int)(radius * Math.Sin(angle));
                points.Add(new Point(x, y));
            }

            return points.ToArray();
        }
    }
}