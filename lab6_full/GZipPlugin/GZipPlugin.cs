using System.IO;
using System.IO.Compression;
using CommonInterfaces;

namespace GZipPlugin
{
    public class GZipArchiver : IProcessingPlugin
    {
        public string PluginName => "GZip Archiver";

        public byte[] ProcessBeforeSave(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
                    gzip.Write(data, 0, data.Length);
                return output.ToArray();
            }
        }

        public byte[] ProcessAfterLoad(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return output.ToArray();
            }
        }
    }
}