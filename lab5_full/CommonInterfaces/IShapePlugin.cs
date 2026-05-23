using System;
using System.Drawing;

namespace CommonInterfaces
{
    public interface IShapePlugin
    {
        string PluginName { get; }
        string PluginVersion { get; }
        string ShapeName { get; }
        Type GetShapeType();
        void RegisterDrawMethod(object visitor);  // object для избежания зависимостей
        object CreateRandomShape(int centerX, int centerY, Random random);
    }
}