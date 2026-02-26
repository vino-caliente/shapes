using System.Collections.Generic;
using System.Drawing;

namespace lab1
{
    public class ShapeList
    {
        private List<Shape> shapes;

        public ShapeList()
        {
            shapes = new List<Shape>();
        }

        public void Add(Shape shape)
        {
            shapes.Add(shape);
        }

        public void Remove(Shape shape)
        {
            shapes.Remove(shape);
        }

        public void Clear()
        {
            shapes.Clear();
        }

        public void DrawAll(Graphics g)
        {
            foreach (var shape in shapes)
            {
                shape.Draw(g);
            }
        }
    }
}
