using System.Net;
using System.Text;

namespace Client3
{
    class IpHandler
    {
        private WebClient _web { get; set; } = new WebClient();

        public string ReturnIp()
        {
            var ipBytes = _web.DownloadData("https://ifconfig.me/ip");
            return Encoding.UTF8.GetString(ipBytes);
        }
    }
}
