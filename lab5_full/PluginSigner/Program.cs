using System;
using System.IO;
using System.Security.Cryptography;

namespace PluginSigner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Plugin Signing Tool ===");

            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            if (args[0] == "--generate-keys" || args[0] == "-g")
            {
                GenerateKeys();
            }
            else if (args[0] == "--help" || args[0] == "-h")
            {
                ShowHelp();
            }
            else if (File.Exists(args[0]) && args[0].EndsWith(".dll"))
            {
                if (args.Length >= 2)
                    SignPlugin(args[0], args[1]);
                else
                    Console.WriteLine("Error: Please provide expiry date (YYYY-MM-DD)");
            }
            else
            {
                Console.WriteLine($"Unknown command or file not found: {args[0]}");
                ShowHelp();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void ShowHelp()
        {
            Console.WriteLine(@"
Usage:
  PluginSigner.exe --generate-keys  (-g)    Generate RSA key pair
  PluginSigner.exe --help          (-h)    Show this help
  PluginSigner.exe <plugin.dll> <expiry>   Sign a plugin

Examples:
  PluginSigner.exe --generate-keys
  PluginSigner.exe StarPlugin.dll 2026-12-31
  PluginSigner.exe MyPlugin.dll 2027-06-01

Date format: YYYY-MM-DD
");
        }

        static void GenerateKeys()
        {
            try
            {
                Console.WriteLine("\n🔐 Generating RSA key pair (2048-bit)...");

                using (RSA rsa = RSA.Create())
                {
                    rsa.KeySize = 2048;

                    string privateKey = rsa.ToXmlString(true);
                    string publicKey = rsa.ToXmlString(false);

                    File.WriteAllText("private_key.xml", privateKey);
                    File.WriteAllText("public_key.xml", publicKey);

                    Console.WriteLine("\n✅ Keys generated successfully!");
                    Console.WriteLine($"   📁 private_key.xml - {new FileInfo("private_key.xml").Length} bytes (KEEP SECRET!)");
                    Console.WriteLine($"   📁 public_key.xml  - {new FileInfo("public_key.xml").Length} bytes");
                    Console.WriteLine("\n📌 Next steps:");
                    Console.WriteLine("   1. Copy public_key.xml to your main application folder");
                    Console.WriteLine("   2. Use private_key.xml to sign your plugins");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error generating keys: {ex.Message}");
            }
        }

        static void SignPlugin(string pluginPath, string expiryDate)
        {
            try
            {
                Console.WriteLine($"\n📦 Signing plugin: {pluginPath}");
                Console.WriteLine($"📅 Expiry date: {expiryDate}");

                if (!File.Exists(pluginPath))
                {
                    Console.WriteLine($"❌ Error: File not found - {pluginPath}");
                    return;
                }

                if (!File.Exists("private_key.xml"))
                {
                    Console.WriteLine("❌ Error: private_key.xml not found!");
                    Console.WriteLine("   Run 'PluginSigner.exe --generate-keys' first.");
                    return;
                }

                // Validate expiry date
                if (!DateTime.TryParse(expiryDate, out DateTime expiry))
                {
                    Console.WriteLine($"❌ Error: Invalid expiry date format. Use YYYY-MM-DD");
                    return;
                }

                byte[] pluginBytes = File.ReadAllBytes(pluginPath);

                using (RSA rsa = RSA.Create())
                {
                    string privateKeyXml = File.ReadAllText("private_key.xml");
                    rsa.FromXmlString(privateKeyXml);

                    byte[] signature = rsa.SignData(
                        pluginBytes,
                        HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pkcs1
                    );

                    string sigPath = pluginPath + ".sig";
                    string expiryPath = pluginPath + ".expiry";

                    File.WriteAllText(sigPath, Convert.ToBase64String(signature));
                    File.WriteAllText(expiryPath, expiryDate);

                    Console.WriteLine("\n✅ Plugin signed successfully!");
                    Console.WriteLine($"   📄 {Path.GetFileName(sigPath)}");
                    Console.WriteLine($"   📄 {Path.GetFileName(expiryPath)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error signing plugin: {ex.Message}");
                Console.WriteLine($"   {ex.StackTrace}");
            }
        }
    }
}