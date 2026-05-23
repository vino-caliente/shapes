using CommonInterfaces;

namespace FriendPlugin
{
    /// <summary>
    /// Simple XOR encryption plugin (friend's implementation)
    /// </summary>
    public class XorEncryptPlugin : IFriendPlugin
    {
        private const byte KEY = 0xAA;  // Simple XOR key

        public string PluginName => "XOR Encryptor";

        public byte[] Encrypt(byte[] data)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                result[i] = (byte)(data[i] ^ KEY);
            return result;
        }

        public byte[] Decrypt(byte[] data)
        {
            // XOR is symmetric: same operation for decrypt
            return Encrypt(data);
        }
    }
}