using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Encryption
{
    public class Encryption
    {
        private byte[] _key;

        private readonly ECDiffieHellmanCng _ecdh;

        /// <summary>
        /// Generates a new pair of ECDH keys.
        /// </summary>
        public Encryption()
        {
            _ecdh = new ECDiffieHellmanCng(256);
            _ecdh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            _ecdh.HashAlgorithm = CngAlgorithm.Sha256;
        }

        /// <summary>
        /// Returns ECDH as bytes.
        /// </summary>
        public byte[] GetPublicKey()
        {
            return _ecdh.PublicKey.ToByteArray();
        }

        /// <summary>
        /// Deriving the shared key from the other public key
        /// </summary>
        public void DeriveSharedKey(byte[] otherPublicKey)
        {
            var otherKey = CngKey.Import(otherPublicKey, CngKeyBlobFormat.EccPublicBlob);
            _key = _ecdh.DeriveKeyMaterial(otherKey);
        }


        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="plainText"></param>
        public string Encrypt(string plainText)
        {
            if (_key == null)
                throw new InvalidOperationException("Calling DerivedSharedKey() before encryption.");

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.GenerateIV();

                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var encryptor = aes.CreateEncryptor())
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        var bytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(bytes, 0, bytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrrypt a base64 string.
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (_key == null)
                throw new InvalidOperationException("Calling DerivedSharedKey() before encryption.");

            byte[] combined = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;

                byte[] iv = new byte[16];
                Array.Copy(combined, 0, iv, 0, 16);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(combined, 16, combined.Length - 16))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
