namespace Dottik.MemeDownloader
{
    public class Program
    {
        public static void GETWindowsDepedencies()
        {
            Console.WriteLine("Getting Depedencies, please hold...");

            Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/"); 
            Directory.CreateDirectory(Environment.CurrentDirectory + @"/Dependencies/");

            Thread getFFMPEG = new(
                () => 
                        {
                            string FFMPEGZIPPATH = Environment.CurrentDirectory + @"/TEMP/FFMPEG-masterx64win.zip";
                            using (FileStream fs0 = File.Create(FFMPEGZIPPATH))
                            {
                                HttpResponseMessage hrm0 = new HttpClient().GetAsync("https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip").GetAwaiter().GetResult();
                                hrm0.Content.CopyToAsync(fs0).GetAwaiter().GetResult();
                            }
                            ZipFile.ExtractToDirectory(FFMPEGZIPPATH, Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/");
                            File.Move(Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/ffmpeg-master-latest-win64-gpl/bin/ffmpeg.exe", Environment.CurrentDirectory + @"/Dependencies/ffmpeg.exe");
                        }
                );
            getFFMPEG.Name = "Get FFMPEG from Github Build!";
            getFFMPEG.Start();

            while (getFFMPEG.IsAlive)
            {
                Thread.Sleep(5);
            }

            Console.WriteLine("Done! Proceeding with execution in 5 seconds...");
            Thread.Sleep(5000);
            Console.Clear();
        }
        public static void Main()
        {
            //GET Windows dependencies...

            if (!Directory.Exists(Environment.CurrentDirectory + @"/Dependencies/"))
            {
                GETWindowsDepedencies();
            }
            if (!Directory.Exists(Environment.CurrentDirectory + @"/TEMP/"))
            {
                //ffmpeg file processing depends on TEMP!
                Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/");
            }

            Console.WriteLine("Loading Configurations...");

            if (!File.Exists(Environment.CurrentDirectory + @"\config.json"))
            {
                Console.WriteLine("Configuration file does not exist, using default configuration and creating one...");
                ConfigurationManager.CreateConfigs(new Configurations());
            }
            else
            {
                ConfigurationManager.ApplyConfigs(ConfigurationManager.LoadConfigs());
            }

            Console.WriteLine(

                    "Configurations applied:\n" +
                    "\n" +
                
                    $"Threads: {InternalProgramData.BotCount}\n" +
                   
                    $"Target Subreddit (n1): {InternalProgramData.TargetSubReddit0}\n" +
                    
                    $"Target Subreddit (n2): {InternalProgramData.TargetSubReddit1}\n" +
                    "" +
                    "" +
                    "" +
                    "" +
                    "" +
                    ""  );

            Console.WriteLine("Preparing Environment...");

            string newPath = Environment.GetEnvironmentVariable("PATH") + ';' + Environment.CurrentDirectory + @"/Dependencies/;";
            Environment.SetEnvironmentVariable("PATH", newPath);

            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Shitposs");

            Console.WriteLine("Starting Hell!");

            if (InternalProgramData.SimultaneousDownload) {

                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandler.StartBot(InternalProgramData.TargetSubReddit0, i * 2));
                    x.Name = $"{InternalProgramData.TargetSubReddit0} bot n" + i;
                    x.IsBackground = true;
                    x.Start();
                }
                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandler.StartBot(InternalProgramData.TargetSubReddit1, i * 2));
                    x.Name = $"{InternalProgramData.TargetSubReddit1} bot n" + i;
                    x.IsBackground = true;
                    x.Start();
                }
            } else
            {
                for (int i = 0; i < InternalProgramData.BotCount / 2; i++)
                {
                    //Execute bots according to the ammount specified on BotCount 🥶👌
                    Thread x = new(() => BotHandler.StartBot(InternalProgramData.TargetSubReddit0, i * 2));
                    x.Name = $"{InternalProgramData.TargetSubReddit0} bot n" + i;
                    x.IsBackground = true;
                    x.Start();
                }
            }


            new Thread(() => OptimizeMemory.CallGC(1024)).Start();

            Console.ReadKey();
            Console.Clear();

            InternalProgramData.STOPPROGRAM = true;

            //Kill Orphan ffmpeg processes
            ProcessOptimizer.KillOrphanProcessProcName("ffmpeg.exe");
            Thread.Sleep(10000);
        }
    }
}


            