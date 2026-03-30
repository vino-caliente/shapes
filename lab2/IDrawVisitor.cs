using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public interface IDrawVisitor
    {
        void Visit(Triangle tr);
        void Visit(Square sq);
        void Visit(Rectangle rect);
        void Visit(Line l);
        void Visit(Circle circ);
        void Visit(Ellipse ell);
    }
}
