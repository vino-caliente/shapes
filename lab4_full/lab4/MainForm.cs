using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace lab1
{
    public partial class MainForm : Form
    {
        private ShapeList list;
        private Random rnd;
        private DrawVisitor visitor;
        private PluginManager pluginManager;

        public enum Shapes
        {
            Triangle, Square, Ellipse, Rectangle, Circle, Line
        }

        public MainForm()
        {
            InitializeComponent();

            // Enable double buffering
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, this.splitContainer1.Panel1, new object[] { true });

            list = new ShapeList();
            rnd = new Random();
            visitor = new DrawVisitor(null);

            // Initialize plugin system
            pluginManager = new PluginManager();
            pluginManager.PluginLoaded += OnPluginLoaded;

            // Load plugins after form is fully initialized
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load plugins
            pluginManager.LoadAllPlugins(visitor, this);
        }

        private void OnPluginLoaded(IShapePlugin plugin)
        {
            // Add plugin shape to combobox
            comboBoxShape.Items.Add(plugin.ShapeName);

            // Optionally: add to a separate list or show notification
            System.Diagnostics.Debug.WriteLine($"Plugin shape available: {plugin.ShapeName}");
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
                string selectedItem = comboBoxShape.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedItem)) return;

                // Check if this is a plugin shape
                IShapePlugin plugin = pluginManager.FindPluginByShapeName(selectedItem);

                if (plugin != null)
                {
                    // Create shape using plugin
                    Shape shape = plugin.CreateRandomShape(e.X, e.Y, rnd);
                    if (shape != null)
                    {
                        list.Add(shape);
                    }
                }
                else
                {
                    // Built-in shape
                    CreateBuiltInShape(e.X, e.Y, selectedItem);
                }

                (sender as Panel)?.Invalidate();
            }
        }

        private void CreateBuiltInShape(int x, int y, string shapeName)
        {
            Color rndColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            bool isFilled = (rnd.Next(2) == 0);
            int lineWidth = rnd.Next(1, 8);
            int int1 = rnd.Next(1, 50);
            int int2 = rnd.Next(1, 50);

            Enum.TryParse(shapeName, out Shapes shape);

            switch (shape)
            {
                case Shapes.Circle:
                    list.Add(new Circle(x, y, int1, rndColor, isFilled, lineWidth));
                    break;
                case Shapes.Ellipse:
                    list.Add(new Ellipse(x - int1 / 2, y - int2 / 2, int1, int2, rndColor, isFilled, lineWidth));
                    break;
                case Shapes.Line:
                    list.Add(new Line(x - int1 / 2, y - int2 / 2, x + int1 / 2, y + int2 / 2, rndColor, lineWidth));
                    break;
                case Shapes.Rectangle:
                    list.Add(new Rectangle(x - int1 / 2, y - int2 / 2, int1, int2, rndColor, isFilled, lineWidth));
                    break;
                case Shapes.Square:
                    list.Add(new Square(x - int1 / 2, y - int1 / 2, int1, rndColor, isFilled, lineWidth));
                    break;
                default: // Triangle
                    list.Add(new Triangle(
                        new Point(x - rnd.Next(1, 15), y - rnd.Next(1, 15)),
                        new Point(x + rnd.Next(1, 15), y - rnd.Next(1, 15)),
                        new Point(x, y + rnd.Next(1, 15)),
                        rndColor, isFilled, lineWidth));
                    break;
            }
        }

        // Optional: reload plugins menu item
        private void reloadPluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear existing plugin items from combobox (keep built-in)
            var builtInShapes = Enum.GetNames(typeof(Shapes));
            var currentItems = comboBoxShape.Items.Cast<string>().ToList();

            foreach (var item in currentItems)
            {
                if (!builtInShapes.Contains(item))
                {
                    comboBoxShape.Items.Remove(item);
                }
            }

            // Reload plugins
            pluginManager.ReloadPlugins(visitor, this);

            // Reset selection to first item
            if (comboBoxShape.Items.Count > 0)
                comboBoxShape.SelectedIndex = 0;
        }
    }
}