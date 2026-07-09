using System.Net.Sockets;
using Protocol;
using System.IO;
using System.Net;

namespace Client3
{
    class PortScan
    {
        public static void ScanPort(string ip, string port, Stream stream)
        {
            var client = new TcpClient();
            var ip2 = IPAddress.Parse(ip);
            try
            {
                client.Connect(ip2, int.Parse(port));
                stream.WriteString($"[~] status on port {port} --> open");
            }
            catch
            {
                stream.WriteString($"[~] status on port {port} --> closed");
            }
        }
    }
}
