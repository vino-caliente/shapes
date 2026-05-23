using System;

namespace CommonInterfaces
{
    /// <summary>
    /// Plugin interface for data processing (compress, encrypt, transform)
    /// </summary>
    public interface IProcessingPlugin
    {
        string PluginName { get; }

        /// <summary>
        /// Process data before saving (compress)
        /// </summary>
        byte[] ProcessBeforeSave(byte[] data);

        /// <summary>
        /// Process data after loading (decompress)
        /// </summary>
        byte[] ProcessAfterLoad(byte[] data);
    }
}