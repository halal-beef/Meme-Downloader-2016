namespace Dottik.MemeDownloader
{
    internal class PrepareEnvironment
    {
        public static void PrepareEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ';' + Environment.CurrentDirectory + @"/Dependencies/;");
        }
        /// <summary>
        /// Creates a lots of folders in one go
        /// </summary>
        /// <param name="paths">The paths made into a sole StringBuilder, separate them using a | at the end of the path</param>
        public static void CreateFolders(StringBuilder paths)
        {
            string[] folders = paths.ToString().Split('|');
            
            for (int i = 0; i < folders.Length; i++)
            {
                if (folders[i] != null && folders[i] != "")
                {
                    Directory.CreateDirectory(folders[i]);
                }
            }
        }
        public static void TestConnection()
        {
            string[] pingDeez = {"1.1.1.1", "1.0.0.1", "8.8.8.8", "8.8.4.4"};

            IPAddress[] objectives = new IPAddress[4];
            bool[] pingResult = new bool[4];

            int totalSuccess = 0;

            for (int i = 0; i < pingResult.Length; i++)
            {
                objectives.SetValue(IPAddress.Parse(pingDeez[i]), i);
                Console.WriteLine($"Pinging {objectives[i]}...");
                pingResult.SetValue(PingAddress(objectives[i]), i);
            }

            for (int i = 0; i < pingResult.Length; i++)
            {
                if (pingResult[i] == true)
                {
                    totalSuccess++;

                    if(totalSuccess > 2)
                        break;
                }
            }

            if(totalSuccess > 0)
            {
                LOGI("PING/s were a Success!");
                Console.WriteLine("There is an internet connection available.");
            }
            else
            {
                Console.WriteLine("Tests through PING failed! Trying to establish a connection with 8.8.8.8...");

                try
                {
                    HttpResponseMessage hrm = new HttpClient().GetAsync("8.8.8.8")
                                                              .GetAwaiter()
                                                              .GetResult();
                    
                    hrm.EnsureSuccessStatusCode();

                    LOGI("| Your PC -> Server | Pings are disabled on this router");

                    Console.WriteLine($"Petition answered with HTTP code ({hrm.StatusCode})OK");
                    Console.WriteLine("There is an internet connection available.");
                }
                catch
                {
                    LOGE("Internet connection couldn't be validated, Terminating Program...");
                    Console.WriteLine($"There isn't an available internet connection available, please, run the program once you are connected to the internet!");
                    Environment.Exit(-1);
                }
            }
        }
        public static void StartBots()
        {
            BotHandler BotHandle = new();
            if (InternalProgramData.SimultaneousDownload)
            {

                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandle.StartBot(true, (int)i * 2));
                    x.Name = $"- bot n{i}";
                    x.IsBackground = true;
                    x.Start();
                }
                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandle.StartBot(false, (int)i * 2));
                    x.Name = $"- bot n{i}";
                    x.IsBackground = true;
                    x.Start();
                }
            }
            else
            {
                for (int i = 0; i < InternalProgramData.BotCount; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandle.StartBot(true, (int)i * 2));
                    x.Name = $"- bot n{i}";
                    x.IsBackground = true;
                    x.Start();
                }
            }
        }
        private static bool PingAddress(IPAddress ipAddress)
        {
            string bufferText = $"This is a connection test {ipAddress}, anyways, how are you doing? Fine? Oh, I don't your response yet, right?";
            byte[] pingBuffer = Encoding.ASCII.GetBytes(bufferText);
            Ping ping = new();

            PingOptions pOpt = new();
            pOpt.DontFragment = true;

            LOGI($"PINGing {ipAddress} with ICMP echo message: {bufferText}");
            PingReply pingReply = ping.Send(ipAddress, 10000, pingBuffer, new PingOptions(64, dontFragment: true));


            if(pingReply.Status == IPStatus.Success)
            {
                //Ping was a success!
                LOGI($"Response of {ipAddress} was OK");
                return true;
            } 
            else if (pingReply.Status == IPStatus.PacketTooBig)
            {
                LOGW($"Response of {ipAddress} was OK, but sent package was too big");
                //Our buffer was  b i g
                return true;
            }
            else
            {
                LOGE($"Response of {ipAddress} was {pingReply.Status}");
                Console.WriteLine(pingReply.Status.ToString());
                return false;
            }

        }
    }
}
