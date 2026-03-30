using System;
using System.Drawing;
using System.Windows.Forms;

namespace lab1
{
    public partial class MainForm : Form
    {
        private ShapeList list;
        private Random rnd;

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            list = new ShapeList();
            rnd = new Random();
        }
        
        private void Form1_MouseDown(object sender, MouseEventArgs e)
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
                int shape = rnd.Next(1, 7);
                switch (shape)
                {
                    case 1:
                        list.Add(new Circle(x, y, int1, rndcolor, isfilled, linewidth));
                        break;
                    case 2:
                        list.Add(new Ellipse(x - int1 / 2, y - int2 / 2, int1, int2, rndcolor, isfilled, linewidth));
                        break;
                    case 3:
                        int1 -= 25;
                        int2 -= 25;
                        list.Add(new Line(x-int1/2, y-int2/2, x + int1/2, y + int2/2, rndcolor, linewidth));
                        break;
                    case 4:
                        list.Add(new Rectangle(x - int1 / 2, y - int2 / 2, int1, int2, rndcolor, isfilled, linewidth));
                        break;
                    case 5:
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
                this.Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.Clear(Color.White);
            list.DrawAll(g);
        }
    }
}
