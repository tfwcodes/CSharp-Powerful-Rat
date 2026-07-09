using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Encryption
{
    public class Encryption
    {
        private readonly byte[] _key;

        public Encryption(string msgToEncrypt)
        {
            var salt = new byte[8];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var pdkdf2 = new Rfc2898DeriveBytes(msgToEncrypt, salt, 1000);
            _key = pdkdf2.GetBytes(16);
        }

        public string Encrypt(string plainText)
        {
            var aes = Aes.Create();
            aes.Key = _key;

            aes.GenerateIV();

            var msEncrypt = new MemoryStream();
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);

            var encryptor = aes.CreateEncryptor();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            var bytes = Encoding.UTF8.GetBytes(plainText);
            csEncrypt.Write(bytes, 0, bytes.Length);
            csEncrypt.FlushFinalBlock();

            return Convert.ToBase64String(msEncrypt.ToArray());

        }

        public string Decrypt(string cipherText)
        {
            byte[] combineBytes = Convert.FromBase64String(cipherText);

            var aes = Aes.Create();
            aes.Key = _key;

            byte[] iv = new byte[16];
            Array.Copy(combineBytes, 0, iv, 0, 16);
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor();
            var msDecrypt = new MemoryStream(combineBytes, 16, combineBytes.Length - 16);
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
