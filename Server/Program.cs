using rat3;
using System.Net;
using System.Net.Sockets;




var main = new Main();
main.ProgramMain();


class Main
{
    public Handle _handle = new Handle();

    
    public void ProgramMain()
    {
        var ip = IPAddress.Parse("0.0.0.0");
        var port = 7777;

        var tcpClient = new TcpListener(ip, port);
        tcpClient.Start();

        var connection = tcpClient.AcceptTcpClient();
        var stream = connection.GetStream();

        _handle.HandleClient(stream);
        
    }
    
}
