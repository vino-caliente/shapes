using System;
using System.Collections.Generic;
using System.Drawing;

namespace lab1
{
    public class DrawVisitor : IDrawVisitor
    {
        private Graphics graphics;

        // Dictionary: Type -> drawing delegate
        private Dictionary<Type, Action<Shape, Graphics>> drawMethods;

        public DrawVisitor(Graphics graphics)
        {
            this.graphics = graphics;
            drawMethods = new Dictionary<Type, Action<Shape, Graphics>>();

            // Register built-in shapes
            RegisterBuiltInDrawMethods();
        }

        // Update graphics reference (for repaint)
        public void UpdateGraphics(Graphics graphics)
        {
            this.graphics = graphics;
        }

        // Public method for plugins to register their draw methods
        public void RegisterDrawMethod<T>(Action<T, Graphics> drawMethod) where T : Shape
        {
            System.Diagnostics.Debug.WriteLine($"RegisterDrawMethod for type: {typeof(T).Name}");
            drawMethods[typeof(T)] = (shape, g) => drawMethod((T)shape, g);
            System.Diagnostics.Debug.WriteLine($"  Registered. Total methods: {drawMethods.Count}");
        }

        // Get current graphics (for plugins that need it)
        public Graphics GetGraphics() => graphics;

        // Main visit method
        public void Visit(Shape shape)
        {
            if (shape == null) return;

            Type shapeType = shape.GetType();

            // Try exact match first
            if (drawMethods.TryGetValue(shapeType, out var drawMethod))
            {
                drawMethod(shape, graphics);
                return;
            }

            // Try base classes (for inheritance like Circle -> Ellipse)
            Type baseType = shapeType.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                if (drawMethods.TryGetValue(baseType, out drawMethod))
                {
                    drawMethod(shape, graphics);
                    return;
                }
                baseType = baseType.BaseType;
            }

            throw new NotSupportedException($"No drawing method registered for type: {shapeType.Name}");
        }

        // Register all built-in shapes
        private void RegisterBuiltInDrawMethods()
        {
            // Triangle
            RegisterDrawMethod<Triangle>((tr, g) =>
            {
                Point[] points = { new Point(tr.X, tr.Y), tr.Point2, tr.Point3 };
                if (tr.IsFilled)
                {
                    using (Brush brush = new SolidBrush(tr.Color))
                        g.FillPolygon(brush, points);
                }
                else
                {
                    using (Pen pen = new Pen(tr.Color, tr.LineWidth))
                        g.DrawPolygon(pen, points);
                }
            });

            // Rectangle
            RegisterDrawMethod<Rectangle>((rect, g) =>
            {
                if (rect.IsFilled)
                {
                    using (Brush brush = new SolidBrush(rect.Color))
                        g.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
                }
                else
                {
                    using (Pen pen = new Pen(rect.Color, rect.LineWidth))
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                }
            });

            // Square (reuses Rectangle logic)
            RegisterDrawMethod<Square>((sq, g) =>
            {
                // Square is a Rectangle, so just draw as rectangle
                if (sq.IsFilled)
                {
                    using (Brush brush = new SolidBrush(sq.Color))
                        g.FillRectangle(brush, sq.X, sq.Y, sq.Side, sq.Side);
                }
                else
                {
                    using (Pen pen = new Pen(sq.Color, sq.LineWidth))
                        g.DrawRectangle(pen, sq.X, sq.Y, sq.Side, sq.Side);
                }
            });

            // Ellipse
            RegisterDrawMethod<Ellipse>((ell, g) =>
            {
                if (ell.IsFilled)
                {
                    using (Brush brush = new SolidBrush(ell.Color))
                        g.FillEllipse(brush, ell.X, ell.Y, ell.Width, ell.Height);
                }
                else
                {
                    using (Pen pen = new Pen(ell.Color, ell.LineWidth))
                        g.DrawEllipse(pen, ell.X, ell.Y, ell.Width, ell.Height);
                }
            });

            // Circle (reuses Ellipse logic)
            RegisterDrawMethod<Circle>((circ, g) =>
            {
                // Circle is an Ellipse with Width = Height = Radius*2
                if (circ.IsFilled)
                {
                    using (Brush brush = new SolidBrush(circ.Color))
                        g.FillEllipse(brush, circ.X - circ.Radius, circ.Y - circ.Radius,
                                     circ.Radius * 2, circ.Radius * 2);
                }
                else
                {
                    using (Pen pen = new Pen(circ.Color, circ.LineWidth))
                        g.DrawEllipse(pen, circ.X - circ.Radius, circ.Y - circ.Radius,
                                     circ.Radius * 2, circ.Radius * 2);
                }
            });

            // Line
            RegisterDrawMethod<Line>((l, g) =>
            {
                using (Pen pen = new Pen(l.Color, l.LineWidth))
                    g.DrawLine(pen, l.X, l.Y, l.EndX, l.EndY);
            });
        }
    }
}