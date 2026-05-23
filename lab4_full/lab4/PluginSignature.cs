using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace lab1
{
    public class PluginSignature
    {
        public class SignatureInfo
        {
            public bool IsValid { get; set; }
            public bool IsExpired { get; set; }
            public string ExpiryDate { get; set; }
            public string ErrorMessage { get; set; }
        }

        // Verify plugin signature and integrity
        public static SignatureInfo VerifyPlugin(string pluginPath)
        {
            var result = new SignatureInfo { IsValid = false };

            try
            {
                // Check if signature file exists
                string sigPath = pluginPath + ".sig";
                if (!File.Exists(sigPath))
                {
                    result.ErrorMessage = "Signature file not found";
                    return result;
                }

                // Check if expiry file exists
                string expiryPath = pluginPath + ".expiry";
                if (!File.Exists(expiryPath))
                {
                    result.ErrorMessage = "Expiry file not found";
                    return result;
                }

                // Load public key from file
                string publicKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public_key.xml");
                if (!File.Exists(publicKeyPath))
                {
                    result.ErrorMessage = "Public key file not found. Please copy public_key.xml to application directory.";
                    return result;
                }

                string publicKeyXml = File.ReadAllText(publicKeyPath);

                // Read plugin bytes
                byte[] pluginBytes = File.ReadAllBytes(pluginPath);

                // Read signature
                string signatureBase64 = File.ReadAllText(sigPath);
                byte[] signature = Convert.FromBase64String(signatureBase64);

                // Verify using RSA
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(publicKeyXml);

                    bool integrityOk = rsa.VerifyData(
                        pluginBytes,
                        signature,
                        HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pkcs1
                    );

                    if (!integrityOk)
                    {
                        result.ErrorMessage = "Plugin integrity check failed (file may be corrupted or tampered)";
                        return result;
                    }
                }

                // Read expiry date
                string expiryDate = File.ReadAllText(expiryPath).Trim();
                result.ExpiryDate = expiryDate;

                // Check expiration
                if (DateTime.TryParse(expiryDate, out DateTime expiry))
                {
                    if (DateTime.Now > expiry)
                    {
                        result.IsExpired = true;
                        result.ErrorMessage = $"Plugin expired on {expiryDate}";
                        return result;
                    }
                }
                else
                {
                    result.ErrorMessage = $"Invalid expiry date format: {expiryDate}";
                    return result;
                }

                result.IsValid = true;
                result.IsExpired = false;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Verification error: {ex.Message}";
            }

            return result;
        }

        // Sign a plugin (used by developer tool)
        public static void SignPlugin(string pluginPath, string privateKeyXml, string expiryDate)
        {
            byte[] pluginBytes = File.ReadAllBytes(pluginPath);

            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);

                byte[] signature = rsa.SignData(
                    pluginBytes,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );

                string signatureBase64 = Convert.ToBase64String(signature);
                File.WriteAllText(pluginPath + ".sig", signatureBase64);
                File.WriteAllText(pluginPath + ".expiry", expiryDate);
            }
        }
    }
}