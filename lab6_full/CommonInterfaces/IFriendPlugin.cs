namespace CommonInterfaces
{
    /// <summary>
    /// Friend's plugin interface (different from our IProcessingPlugin)
    /// </summary>
    public interface IFriendPlugin
    {
        string PluginName { get; }
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);
    }
}