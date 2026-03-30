using System;
using System.Drawing;
using System.Windows.Forms;

namespace lab1
{
    public partial class MainForm : Form
    {
        private ShapeList list;
        private Random rnd;
        private DrawVisitor visitor;

        public enum Shapes
        {
            Triangle, Square, Ellipse, Rectangle, Circle, Line
        }

        public MainForm()
        {
            InitializeComponent();
            // включение двойной буферизации для панели отрисовки
            typeof(Panel).InvokeMember("DoubleBuffered",
            System.Reflection.BindingFlags.SetProperty |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic,
            null, this.splitContainer1.Panel1, new object[] { true });

            list = new ShapeList();
            rnd = new Random();
            visitor = new DrawVisitor(null);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            visitor.UpdateGraphics(g);

            list.DrawAll(visitor);
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Color rndcolor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                int x = e.X;
                int y = e.Y;
                bool isfilled = (rnd.Next(2) == 0);
                int linewidth = rnd.Next(1, 8);
                int int1 = rnd.Next(1, 50);
                int int2 = rnd.Next(1, 50);
                Enum.TryParse(comboBoxShape.SelectedItem.ToString(), out Shapes shape);
                switch (shape)
                {
                    case Shapes.Circle:
                        list.Add(new Circle(x, y, int1, rndcolor, isfilled, linewidth));
                        break;
                    case Shapes.Ellipse:
                        list.Add(new Ellipse(x - int1 / 2, y - int2 / 2, int1, int2, rndcolor, isfilled, linewidth));
                        break;
                    case Shapes.Line:
                        int1 -= 25;
                        int2 -= 25;
                        list.Add(new Line(x - int1 / 2, y - int2 / 2, x + int1 / 2, y + int2 / 2, rndcolor, linewidth));
                        break;
                    case Shapes.Rectangle:
                        list.Add(new Rectangle(x - int1 / 2, y - int2 / 2, int1, int2, rndcolor, isfilled, linewidth));
                        break;
                    case Shapes.Square:
                        list.Add(new Square(x - int1 / 2, y - int1 / 2, int1, rndcolor, isfilled, linewidth));
                        break;
                    default:
                        list.Add(new Triangle(
                            new Point(x - rnd.Next(1, 15), y - rnd.Next(1, 15)),
                            new Point(x + rnd.Next(1, 15), y - rnd.Next(1, 15)),
                            new Point(x, y + rnd.Next(1, 15)),
                            rndcolor, isfilled, linewidth));
                        break;
                }
                (sender as Panel).Invalidate();
            }
        }
    }
}
