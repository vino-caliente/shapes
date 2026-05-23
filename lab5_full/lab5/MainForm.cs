using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using CommonInterfaces;

namespace lab1
{
    public partial class MainForm : Form
    {
        private ShapeList list;
        private Random rnd;
        private DrawVisitor visitor;
        private ProcessingPipeline pipeline;
        private ShapeSerializer serializer;
        private ProcessingPluginManager processingPluginManager;
        private PluginManager shapePluginManager;

        public enum Shapes { Triangle, Square, Ellipse, Rectangle, Circle, Line }

        public MainForm()
        {
            InitializeComponent();
            SetupMenu();

            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic, null, splitContainer1.Panel1, new object[] { true });

            list = new ShapeList();
            rnd = new Random();
            visitor = new DrawVisitor(null);
            serializer = new ShapeSerializer();
            pipeline = new ProcessingPipeline();

            // Load shape plugins (StarPlugin, etc.)
            shapePluginManager = new PluginManager();
            shapePluginManager.PluginLoaded += OnShapePluginLoaded;

            // Load processing plugins (GZipPlugin, etc.)
            processingPluginManager = new ProcessingPluginManager();
            processingPluginManager.PluginLoaded += (plugin) => {
                pipeline.AddPlugin(plugin);
                var item = new ToolStripMenuItem(plugin.PluginName);
                item.Tag = plugin;
                item.CheckOnClick = true;
                item.Checked = true;
                item.Click += (s, e) => {
                    if (item.Checked) pipeline.AddPlugin((IProcessingPlugin)item.Tag);
                    else pipeline.GetPlugins().Remove((IProcessingPlugin)item.Tag);
                };
                pluginsToolStripMenuItem.DropDownItems.Add(item);
            };

            // Load all plugins when form loads
            this.Load += (s, e) => {
                shapePluginManager.LoadAllPlugins(visitor, this);
                processingPluginManager.LoadAllPlugins();
            };
        }

        private void OnShapePluginLoaded(IShapePlugin plugin)
        {
            comboBoxShape.Items.Add(plugin.ShapeName);
            System.Diagnostics.Debug.WriteLine($"Loaded shape plugin: {plugin.ShapeName}");
        }

        private void SetupMenu()
        {
            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("File");
            var saveItem = new ToolStripMenuItem("Save");
            var loadItem = new ToolStripMenuItem("Load");
            var pluginsMenu = new ToolStripMenuItem("Plugins");
            pluginsToolStripMenuItem = new ToolStripMenuItem("Processing");

            saveItem.Click += (s, e) => {
                var dlg = new SaveFileDialog() { Filter = "Shape files (*.shp)|*.shp", DefaultExt = "shp" };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var json = serializer.Serialize(list);

                        // ← ВЫВОДИМ JSON В ОКНО DEBUG
                        System.Diagnostics.Debug.WriteLine("=== SAVED JSON ===");
                        System.Diagnostics.Debug.WriteLine(json);
                        System.Diagnostics.Debug.WriteLine("=== END JSON ===");

                        var data = pipeline.ProcessBeforeSave(System.Text.Encoding.UTF8.GetBytes(json));
                        File.WriteAllBytes(dlg.FileName, data);
                        MessageBox.Show($"Saved {list.GetAll().Count} shapes", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            loadItem.Click += (s, e) => {
                var dlg = new OpenFileDialog() { Filter = "Shape files (*.shp)|*.shp" };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var data = pipeline.ProcessAfterLoad(File.ReadAllBytes(dlg.FileName));
                        var json = System.Text.Encoding.UTF8.GetString(data);
                        var newList = serializer.Deserialize(json, shapePluginManager);  // ← передаём

                        list = newList;
                        splitContainer1.Panel1.Invalidate();
                        splitContainer1.Panel1.Refresh();

                        MessageBox.Show($"Loaded {list.GetAll().Count} shapes", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            fileMenu.DropDownItems.Add(saveItem);
            fileMenu.DropDownItems.Add(loadItem);
            pluginsMenu.DropDownItems.Add(pluginsToolStripMenuItem);
            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(pluginsMenu);
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private ToolStripMenuItem pluginsToolStripMenuItem;

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            visitor.UpdateGraphics(e.Graphics);
            list.DrawAll(visitor);
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var selected = comboBoxShape.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selected)) return;

            // Check if selected shape is from a plugin
            IShapePlugin shapePlugin = shapePluginManager?.FindPluginByShapeName(selected);

            if (shapePlugin != null)
            {
                // Create shape using plugin
                Shape shape = shapePlugin.CreateRandomShape(e.X, e.Y, rnd);
                if (shape != null)
                {
                    list.Add(shape);
                    System.Diagnostics.Debug.WriteLine($"Added {shape.GetType().Name} at ({e.X}, {e.Y})");
                }
            }
            else
            {
                // Built-in shape
                Color rndColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                bool isFilled = rnd.Next(2) == 0;
                int lineWidth = rnd.Next(1, 8);
                int int1 = rnd.Next(1, 50);
                int int2 = rnd.Next(1, 50);

                Enum.TryParse(selected, out Shapes shape);
                switch (shape)
                {
                    case Shapes.Circle:
                        list.Add(new Circle(e.X, e.Y, int1, rndColor, isFilled, lineWidth));
                        break;
                    case Shapes.Ellipse:
                        list.Add(new Ellipse(e.X - int1 / 2, e.Y - int2 / 2, int1, int2, rndColor, isFilled, lineWidth));
                        break;
                    case Shapes.Line:
                        list.Add(new Line(e.X - int1 / 2, e.Y - int2 / 2, e.X + int1 / 2, e.Y + int2 / 2, rndColor, lineWidth));
                        break;
                    case Shapes.Rectangle:
                        list.Add(new Rectangle(e.X - int1 / 2, e.Y - int2 / 2, int1, int2, rndColor, isFilled, lineWidth));
                        break;
                    case Shapes.Square:
                        list.Add(new Square(e.X - int1 / 2, e.Y - int1 / 2, int1, rndColor, isFilled, lineWidth));
                        break;
                    default:
                        list.Add(new Triangle(
                            new Point(e.X - rnd.Next(1, 15), e.Y - rnd.Next(1, 15)),
                            new Point(e.X + rnd.Next(1, 15), e.Y - rnd.Next(1, 15)),
                            new Point(e.X, e.Y + rnd.Next(1, 15)),
                            rndColor, isFilled, lineWidth));
                        break;
                }
            }
            (sender as Panel)?.Invalidate();
        }

        // Public method for plugins to access serializer
        public ShapeSerializer GetShapeSerializer()
        {
            return serializer;
        }
    }
}