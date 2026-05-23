using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommonInterfaces;

namespace lab1
{
    public class ProcessingPluginManager
    {
        private List<IProcessingPlugin> loadedPlugins = new List<IProcessingPlugin>();
        private string pluginsDirectory;

        public event Action<IProcessingPlugin> PluginLoaded;

        public ProcessingPluginManager()
        {
            pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);
        }

        public void LoadAllPlugins()
        {
            foreach (string dllPath in Directory.GetFiles(pluginsDirectory, "*.dll"))
            {
                LoadPlugin(dllPath);
            }
        }

        private void LoadPlugin(string dllPath)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);

                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IProcessingPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        IProcessingPlugin plugin = (IProcessingPlugin)Activator.CreateInstance(type);
                        loadedPlugins.Add(plugin);
                        PluginLoaded?.Invoke(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading plugin: {ex.Message}");
            }
        }

        public List<IProcessingPlugin> GetPlugins() => loadedPlugins;
    }
}