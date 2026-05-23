using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace lab1
{
    public class PluginManager
    {
        private List<IShapePlugin> loadedPlugins = new List<IShapePlugin>();
        private string pluginsDirectory;

        // Event for UI updates
        public event Action<IShapePlugin> PluginLoaded;

        public PluginManager()
        {
            pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            // Create plugins directory if not exists
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }
        }

        // Load all plugins from directory
        public void LoadAllPlugins(DrawVisitor visitor, MainForm mainForm)
        {
            string[] dllFiles = Directory.GetFiles(pluginsDirectory, "*.dll");

            foreach (string dllPath in dllFiles)
            {
                LoadPlugin(dllPath, visitor, mainForm);
            }
        }

        // Load a single plugin from DLL
        private void LoadPlugin(string dllPath, DrawVisitor visitor, MainForm mainForm)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading plugin: {dllPath}");

                Assembly assembly = Assembly.LoadFrom(dllPath);

                foreach (Type type in assembly.GetTypes())
                {
                    System.Diagnostics.Debug.WriteLine($"  Found type: {type.Name}");

                    if (typeof(IShapePlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Creating instance of {type.Name}");
                        IShapePlugin plugin = (IShapePlugin)Activator.CreateInstance(type);

                        System.Diagnostics.Debug.WriteLine($"  Calling RegisterDrawMethod for {plugin.ShapeName}");
                        plugin.RegisterDrawMethod(visitor);

                        plugin.OnLoad(mainForm);
                        loadedPlugins.Add(plugin);
                        PluginLoaded?.Invoke(plugin);

                        System.Diagnostics.Debug.WriteLine($"Loaded plugin: {plugin.PluginName} v{plugin.PluginVersion}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                MessageBox.Show($"Error loading plugin {Path.GetFileName(dllPath)}: {ex.Message}",
                                "Plugin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Get all loaded plugins
        public List<IShapePlugin> GetPlugins() => loadedPlugins;

        // Find plugin by shape name
        public IShapePlugin FindPluginByShapeName(string shapeName)
        {
            return loadedPlugins.FirstOrDefault(p => p.ShapeName == shapeName);
        }

        // Reload plugins (clear and reload)
        public void ReloadPlugins(DrawVisitor visitor, MainForm mainForm)
        {
            loadedPlugins.Clear();
            LoadAllPlugins(visitor, mainForm);
        }
    }
}