using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Encryption
{
    /// <summary>
    /// AES-256-CBC encryption cu ECDH key exchange.
    /// Cheia AES e derivata din shared secret-ul Diffie-Hellman —
    /// nu e transmisa niciodata pe retea, elimina atacurile passive MitM.
    /// </summary>
    public class Encryption
    {
        // Cheia AES de 32 bytes derivata din ECDH — ramane mereu locala
        private byte[] _key;

        // Instanta ECDH-256 — genereaza perechea de chei la constructie
        private readonly ECDiffieHellmanCng _ecdh;

        /// <summary>
        /// Genereaza o pereche de chei ECDH-256 fresh.
        /// Apeleaza GetPublicKey() si DeriveSharedKey() inainte de Encrypt/Decrypt.
        /// </summary>
        public Encryption()
        {
            _ecdh = new ECDiffieHellmanCng(256);
            // SHA-256 aplica direct pe shared secret => rezulta 32 bytes pentru AES-256
            _ecdh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            _ecdh.HashAlgorithm = CngAlgorithm.Sha256;
        }

        /// <summary>
        /// Returneaza cheia publica ECDH ca byte[] (format ECC public blob).
        /// Trimite asta celuilalt capăt o singura data la conectare.
        /// </summary>
        public byte[] GetPublicKey()
        {
            return _ecdh.PublicKey.ToByteArray();
        }

        /// <summary>
        /// Deriveaza cheia AES din cheia publica a celuilalt capăt.
        /// Trebuie apelata o singura data dupa schimbul de chei publice.
        /// </summary>
        /// <param name="otherPublicKey">Cheia publica primita de la celalalt capăt.</param>
        public void DeriveSharedKey(byte[] otherPublicKey)
        {
            // Importa cheia publica (acelasi format EccPublicBlob ca GetPublicKey)
            var otherKey = CngKey.Import(otherPublicKey, CngKeyBlobFormat.EccPublicBlob);
            // Rezultat: SHA-256(ECDH shared secret) = 32 bytes cheie AES
            _key = _ecdh.DeriveKeyMaterial(otherKey);
        }

        /// <summary>
        /// Encripteaza plainText cu AES-256-CBC.
        /// Un IV random de 16 bytes e prefixat la fiecare mesaj — fiecare ciphertext e unic.
        /// Returneaza Base64( [IV 16 bytes] + [ciphertext] ).
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (_key == null)
                throw new InvalidOperationException("Apeleaza DeriveSharedKey() inainte de Encrypt.");

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.GenerateIV(); // IV random nou per mesaj

                using (var ms = new MemoryStream())
                {
                    // Prefixam IV-ul ca Decrypt() sa-l poata extrage
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
        /// Decripteaza un string Base64 produs de Encrypt().
        /// Primii 16 bytes sunt IV-ul, restul e ciphertext-ul.
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (_key == null)
                throw new InvalidOperationException("Apeleaza DeriveSharedKey() inainte de Decrypt.");

            byte[] combined = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;

                // Extrage IV-ul din primii 16 bytes prefixati de Encrypt()
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
