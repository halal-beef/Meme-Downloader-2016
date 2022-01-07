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
                Console.WriteLine("There is an internet connection available.");
            }
            else
            {
                Console.WriteLine($"There isn't an available internet connection available, please, run the program once you are connected to the internet!");
                Environment.Exit(-1);
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
            PingReply pingReply = ping.Send(ipAddress, 10000, pingBuffer, new PingOptions(64, dontFragment: true));

            if(pingReply.Status == IPStatus.Success)
            {
                //Ping was a success!
                return true;
            } 
            else if (pingReply.Status == IPStatus.PacketTooBig)
            {
                //Our buffer was  b i g
                return true;
            }
            else
            {
                Console.WriteLine(pingReply.Status.ToString());
                return false;
            }

        }
    }
}
