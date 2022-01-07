namespace Dottik.MemeDownloader
{
    internal class PrepareEnvironment
    {
        public static void PrepareEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ';' + Environment.CurrentDirectory + @"/Dependencies/;");
        }
        public static void CreateFolders(List<string> paths)
        {
            foreach (string path in paths)
            {
                Directory.CreateDirectory(path);
            }
        }
        public static void TestConnection()
        {
            string[] pingDeez = {"1.1.1.1", "1.0.0.1", "8.8.8.8", "8.8.4.4"};
            List<bool> pingResult = new();

            int totalSuccess = 0; 

            foreach (string ip in pingDeez)
            {
                Console.WriteLine($"Pinging {IPAddress.Parse(ip)}...");
                pingResult.Add(PingAddress(IPAddress.Parse(ip)));
            }

            foreach (bool success in pingResult)
            {
                if(success)
                    totalSuccess++;
            }

            if(totalSuccess > 0)
            {
                LOGI("PINGs were ");
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
            if (InternalProgramData.SimultaneousDownload)
            {

                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandler.StartBot(true, (int)i * 2));
                    x.Name = $"- bot n{i}";
                    x.IsBackground = true;
                    x.Start();
                }
                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandler.StartBot(false, (int)i * 2));
                    x.Name = $"- bot n{i}";
                    x.IsBackground = true;
                    x.Start();
                }
            }
            else
            {
                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandler.StartBot(true, (int)i * 2));
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
