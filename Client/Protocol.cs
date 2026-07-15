using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Encryption;

namespace Protocol
{
    public static class Protocol
    {

        private static readonly ConditionalWeakTable<Stream, Encryption.Encryption> _sessions
            = new ConditionalWeakTable<Stream, Encryption.Encryption>();


        public static void ReadAll(this Stream stream, byte[] buffer)
        {
            if (buffer.Length == 0) return;

            var total = 0;
            while (total != buffer.Length)
            {
                var read = stream.Read(buffer, total, buffer.Length - total);
                if (read == 0) throw new Exception("Connection Closed");
                total += read;
            }
        }

        public static int ReadSize(this Stream stream)
        {
            var sizeMsg = new byte[4];
            stream.ReadAll(sizeMsg);
            return BitConverter.ToInt32(sizeMsg, 0);
        }

        public static byte[] ReadBytes(this Stream stream)
        {
            var size = ReadSize(stream);
            var buf = new byte[size];
            stream.ReadAll(buf);
            return buf;
        }

        public static void WriteBytes(this Stream stream, byte[] buffer)
        {
            stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
            stream.Write(buffer, 0, buffer.Length);
        }


        /// <summary>
        /// Does the ECDH handshake on this stream. The server sends the public key, and waits for the client's one. (the client doing the same)
        /// </summary>
        public static void InitializeEncryption(this Stream stream, bool isServer)
        {
            var enc = new Encryption.Encryption();

            if (isServer)
            {
                // the server
                stream.WriteBytes(enc.GetPublicKey());
                var clientPubKey = stream.ReadBytes();
                enc.DeriveSharedKey(clientPubKey);
            }
            else
            {
                // the client
                var serverPubKey = stream.ReadBytes();
                stream.WriteBytes(enc.GetPublicKey());
                enc.DeriveSharedKey(serverPubKey);
            }

            _sessions.Add(stream, enc);
        }


        /// <summary>
        /// Write a string.
        /// </summary>
        public static void WriteString(this Stream stream, string message)
        {
            if (!_sessions.TryGetValue(stream, out var enc))
                throw new InvalidOperationException("Apeleaza InitializeEncryption() inainte de WriteString.");

            var encrypted = enc.Encrypt(message);
            stream.WriteBytes(Encoding.UTF8.GetBytes(encrypted));
        }

        /// <summary>
        /// Reads and decrypts.
        /// </summary>
        public static string ReadString(this Stream stream)
        {
            if (!_sessions.TryGetValue(stream, out var enc))
                throw new InvalidOperationException("Apeleaza InitializeEncryption() inainte de ReadString.");

            var cipherText = Encoding.UTF8.GetString(stream.ReadBytes());
            return enc.Decrypt(cipherText);
        }
    }
}
