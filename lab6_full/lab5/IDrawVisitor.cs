using System;

namespace lab1
{
    public interface IDrawVisitor
    {
        // Single visit method for all shapes
        void Visit(Shape shape);
    }
}