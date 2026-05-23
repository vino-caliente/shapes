using CommonInterfaces;

namespace FriendPluginAdapter
{
    /// <summary>
    /// Adapter that converts IFriendPlugin to IProcessingPlugin
    /// </summary>
    public class FriendToProcessingAdapter : IProcessingPlugin
    {
        private IFriendPlugin friendPlugin;

        public FriendToProcessingAdapter(IFriendPlugin plugin)
        {
            friendPlugin = plugin;
        }

        public string PluginName => friendPlugin.PluginName;

        public byte[] ProcessBeforeSave(byte[] data)
        {
            // Encrypt before saving
            return friendPlugin.Encrypt(data);
        }

        public byte[] ProcessAfterLoad(byte[] data)
        {
            // Decrypt after loading
            return friendPlugin.Decrypt(data);
        }
    }
}