using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommonInterfaces;
using FriendPluginAdapter; 

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
                    // Check for our IProcessingPlugin interface
                    if (typeof(IProcessingPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        IProcessingPlugin plugin = (IProcessingPlugin)Activator.CreateInstance(type);
                        loadedPlugins.Add(plugin);
                        PluginLoaded?.Invoke(plugin);
                        System.Diagnostics.Debug.WriteLine($"Loaded IProcessingPlugin: {plugin.PluginName}");
                    }

                    // ← НОВОЕ: Check for friend's IFriendPlugin and wrap with adapter
                    else if (typeof(IFriendPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        IFriendPlugin friendPlugin = (IFriendPlugin)Activator.CreateInstance(type);
                        IProcessingPlugin adapter = new FriendToProcessingAdapter(friendPlugin);
                        loadedPlugins.Add(adapter);
                        PluginLoaded?.Invoke(adapter);
                        System.Diagnostics.Debug.WriteLine($"Loaded friend plugin via adapter: {adapter.PluginName}");
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