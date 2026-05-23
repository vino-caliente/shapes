using System.Collections.Generic;
using CommonInterfaces;

namespace lab1
{
    public class ProcessingPipeline
    {
        private List<IProcessingPlugin> plugins = new List<IProcessingPlugin>();

        public void AddPlugin(IProcessingPlugin plugin)
        {
            plugins.Add(plugin);
        }

        public List<IProcessingPlugin> GetPlugins() => plugins;

        public byte[] ProcessBeforeSave(byte[] data)
        {
            byte[] result = data;
            foreach (var plugin in plugins)
                result = plugin.ProcessBeforeSave(result);
            return result;
        }

        public byte[] ProcessAfterLoad(byte[] data)
        {
            byte[] result = data;
            for (int i = plugins.Count - 1; i >= 0; i--)
                result = plugins[i].ProcessAfterLoad(result);
            return result;
        }
    }
}