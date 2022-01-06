namespace Dottik.MemeDownloader
{
    public class Program
    {
        /// <summary>
        /// Verifies a MD5 Hash!
        /// </summary>
        /// <param name="Expectedsha256">The expected SHA-256 Hash in a string</param>
        /// <param name="PathToFile">Path to the file to calculate the hash and compare</param>
        /// <returns>true if the expected hash is the same as the one of the file; false if they are different</returns>
        public static bool VerifyFileIntegrity(string Expectedsha256, string PathToFile) 
        { 
            SHA256 sHA256 = SHA256.Create();
            try
            {
                using FileStream fs = File.OpenRead(PathToFile);
                byte[] objectiveHash = sHA256.ComputeHash(fs);

                string stringObjHash = BitConverter.ToString(objectiveHash).Replace("-", String.Empty);

                Console.WriteLine($"Expected Hash: {Expectedsha256}; Result Hash: {stringObjHash}");

                if (Expectedsha256 == stringObjHash)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static void GETWindowsDepedencies()
        {

            Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/"); 
            Directory.CreateDirectory(Environment.CurrentDirectory + @"/Dependencies/");

            Thread getFFMPEG = new(() => 
                {
                    string FFMPEGZIPPATH = Environment.CurrentDirectory + @"/TEMP/FFMPEG-masterx64win.zip";
                    using (FileStream fs0 = File.Create(FFMPEGZIPPATH))
                    {
                        HttpResponseMessage hrm4 = new HttpClient(handler: InternalProgramData.handler).GetAsync("https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip").GetAwaiter().GetResult();
                        hrm4.Content.CopyToAsync(fs0).GetAwaiter().GetResult();
                    }
                    ZipFile.ExtractToDirectory(FFMPEGZIPPATH, Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/");
                    File.Move(Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/ffmpeg-master-latest-win64-gpl/bin/ffmpeg.exe", Environment.CurrentDirectory + @"/Dependencies/ffmpeg.exe", true);
                });

            Thread getYTDLP = new(() =>
                {
                    string YTDLPPATH = Environment.CurrentDirectory + @"/Dependencies/yt-dlp.exe";

                    using FileStream fs0 = File.Create(YTDLPPATH);
                    HttpResponseMessage hrm5 = new HttpClient(handler: InternalProgramData.handler).GetAsync("https://github.com/yt-dlp/yt-dlp/releases/download/2021.12.27/yt-dlp.exe").GetAwaiter().GetResult();
                    hrm5.Content.CopyToAsync(fs0).GetAwaiter().GetResult();

                });

            getYTDLP.Name = "Get yt-dlp from Github Build!";

            getFFMPEG.Name = "Get FFMPEG from Github Build!";
            getYTDLP.Start();
            getFFMPEG.Start();

            Console.Write("Getting ffmpeg and YouTube-dlp");
            while (getFFMPEG.IsAlive || getYTDLP.IsAlive)
            {
                Console.Write('.');
                Thread.Sleep(500);
            }

            Console.WriteLine("\nDone! Please restart the program to continue.");
            
            Thread startNewInstance = new(() => 
            {
                Process proc = new();
                proc.StartInfo.FileName = Environment.ProcessPath;
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
            }
            );
            startNewInstance.IsBackground = false;
            startNewInstance.Start();

            Process proc = Process.GetCurrentProcess();
            proc.Kill();
            Console.Clear();
        }
        public static void Main()
        {
            if (Directory.Exists(Environment.CurrentDirectory + @"/TEMP/"))
            {
                Directory.Delete(Environment.CurrentDirectory + @"/TEMP/", true);

                Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/");
            }
            else
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/");

            }

            //GET Windows dependencies & Verify them

            if (!Directory.Exists(Environment.CurrentDirectory + @"/Dependencies/"))
            {
                Console.WriteLine("Getting Depedencies, please hold...");
                GETWindowsDepedencies();
            }
            else 
            {
                Console.WriteLine("Verifying Install...");
                try
                {
                    string
                       ytdlpsha256 = "1A34F627CA88251BE6D17940F026F6A5B8AAAF0AA32DD60DEEC3020F81950E67",
                       ffmpegsha256 = "D6DEEDD283FCCE95B3709C7FBF716419443E14209AD6866F3D14A78D1F077E39";

                    bool ytdlpOK = VerifyFileIntegrity(ytdlpsha256, Environment.CurrentDirectory + @"/Dependencies/yt-dlp.exe");
                    bool ffmpegOK = VerifyFileIntegrity(ffmpegsha256, Environment.CurrentDirectory + @"/Dependencies/ffmpeg.exe");
     
                    if (!ytdlpOK || !ffmpegOK)
                    {
                        Console.WriteLine("The program dependencies are corrupted! Redownloading them...");
                        GETWindowsDepedencies();
                    }
                }
                catch
                {
                    Console.WriteLine("The program dependencies are corrupted! Redownloading them...");
                    GETWindowsDepedencies();
                }
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
                   
                    $"Max Repeats Allowed Until Change of Subreddit: {InternalProgramData.MaxRepeatTimes}\n" +

                    $"Target Subreddit (n1): {InternalProgramData.TargetSubReddit0}\n" +
                    
                    $"Target Subreddit (n2): {InternalProgramData.TargetSubReddit1}\n" +

                    $"Target Subreddit (n3): {InternalProgramData.TargetSubReddit2}\n" +
                    
                    $"Target Subreddit (n4): {InternalProgramData.TargetSubReddit3}\n" +
                    
                    $"Target Subreddit (n5): {InternalProgramData.TargetSubReddit4}\n" +

                    $"Target Subreddit (n6): {InternalProgramData.TargetSubReddit5}\n" +

                    ""  );

            Console.WriteLine("Preparing Environment...");

            string newPath = Environment.GetEnvironmentVariable("PATH") + ';' + Environment.CurrentDirectory + @"/Dependencies/;";
            Environment.SetEnvironmentVariable("PATH", newPath);

            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Downloaded Content");
            Directory.CreateDirectory($"Downloaded Content/{InternalProgramData.TargetSubReddit0}");
            Directory.CreateDirectory($"Downloaded Content/{InternalProgramData.TargetSubReddit1}");
            Directory.CreateDirectory($"Downloaded Content/{InternalProgramData.TargetSubReddit2}");
            Directory.CreateDirectory($"Downloaded Content/{InternalProgramData.TargetSubReddit3}");
            Directory.CreateDirectory($"Downloaded Content/{InternalProgramData.TargetSubReddit4}");
            Directory.CreateDirectory($"Downloaded Content/{InternalProgramData.TargetSubReddit5}");

            Console.WriteLine("Starting Hell! \n\nNOTE N1: Don't worry if you don't see any output on this console, that is because the program is finding a lot of repetitions and it isn't logging them :)");
            Console.WriteLine($"NOTE N2: You can quit on ANY moment pressing ANY key on the keyboard, doing it this way provides a lower chance of ending up with corrupted downloads (The program closes after 25 seconds)\n\n\n\n\n");
            if (InternalProgramData.SimultaneousDownload) {

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
            } else
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


            new Thread(() => OptimizeMemory.CallGC(1024)).Start();
            new Thread(() => DynamicDownloader.RepeatWatchdog()).Start();









            Console.ReadKey();
            Console.Clear();

            InternalProgramData.STOPPROGRAM = true;

            EndExecution.TerminateProgram();
        }
    }
}


            