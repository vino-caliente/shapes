using System;
using System.Drawing;
using System.Windows.Forms;

namespace lab1
{
    public interface IShapePlugin
    {
        // Plugin metadata
        string PluginName { get; }
        string PluginVersion { get; }

        // Shape information
        string ShapeName { get; }
        Type GetShapeType();

        // Register drawing method with the visitor
        void RegisterDrawMethod(DrawVisitor visitor);

        // Factory method to create a random shape
        Shape CreateRandomShape(int centerX, int centerY, Random random);

        // Optional: custom parameter dialog
        bool ShowParameterDialog(out Shape shape, Point location);

        // Called after plugin is loaded
        void OnLoad(MainForm mainForm);
    }
}