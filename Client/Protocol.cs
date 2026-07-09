using System.ComponentModel;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Encryption;

namespace Protocol
{
    public static class Protocol
    {
        public static void ReadAll(this Stream stream, byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return;
            }

            var total = 0;
            while (total != buffer.Length)
            {
                
                var read = stream.Read(buffer, total, buffer.Length - total);
                if (read == 0)
                {
                    throw new Exception("Connection Closed");
                }
                total += read;
            }
        }

        /// <summary>
        /// Read the size of the message.
        /// </summary>
        /// <param name="stream"></param>
        public static int ReadSize(this Stream stream)
        {
            var sizeMsg = new byte[4];

            stream.ReadAll(sizeMsg);

            return BitConverter.ToInt32(sizeMsg, 0);
        }



        /// <summary>
        /// Read the bytes of the message.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this Stream stream)
        {  
            var size = ReadSize(stream);
            var bytesMsg = new byte[size];

            stream.ReadAll(bytesMsg);

            return bytesMsg;
        }

        /// <summary>
        /// Send the bytes trough the network.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        public static void WriteBytes(this Stream stream, byte[] buffer)
        {   
            int length = buffer.Length;

            stream.Write(BitConverter.GetBytes(length), 0, 4);
            stream.Write(buffer, 0, buffer.Length);
        }


        /// <summary>
        /// Send the message.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="message"></param>
        public static void WriteString(this Stream stream, string message)
        {
            var aes = new Encryption.Encryption(message);
            var encrypted = aes.Encrypt(message);
            var key = aes._key;
            stream.WriteBytes(key);

            stream.WriteBytes(Encoding.UTF8.GetBytes(encrypted));
        }


        /// <summary>
        /// Receive the message.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadString(this Stream stream)
        {
            var key = stream.ReadBytes();
            var decryption = new Decryption();

            var decryptionText = Encoding.UTF8.GetString(stream.ReadBytes());
            return decryption.Decrypt(decryptionText, key);
        }
    }
}
    