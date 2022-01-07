namespace Dottik.MemeDownloader
{
    public class Program
    {

        public static void Main()
        {
            //Adds the possibility of closing after opening the program
            Thread exitWatch = new(() =>
            {
                LOGI($"Started Exit Thread with name: \'{Thread.CurrentThread.Name}\'");

                Console.ReadKey();
                if (InternalProgramData.WaitToKill)
                {
                    Console.Clear();

                    InternalProgramData.STOPPROGRAM = true;

                    EndExecution.TerminateProgram();
                }
                else
                {
                    Environment.Exit(0);
                }
            });
            exitWatch.Name = "Exit Watchdog Thread";
            exitWatch.Start();


            Console.WriteLine("Testing Internet Connection...");

                PrepareEnvironment.TestConnection();

            if (Directory.Exists(Environment.CurrentDirectory + @"/TEMP/"))
            {
                try
                {
                    Directory.Delete(Environment.CurrentDirectory + @"/TEMP/", true);
                }
                catch
                {
                    Console.WriteLine("Error deleting TEMP folder, There is probably a process fiddling with the files.");
                }
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
                DependencyManagment.GETWindowsDepedencies();
            }
            else 
            {
                Console.WriteLine("Verifying Install...");
                try
                {
                    string
                        ytdlpsha256 = "1A34F627CA88251BE6D17940F026F6A5B8AAAF0AA32DD60DEEC3020F81950E67",
                       ffmpegsha256 = "436844A3ECF9B2ECC13E57B2B5D000ADBB6FEA6FE99C7D5921C99284A91C50DC";

                    bool ytdlpOK = DependencyManagment.VerifyFileIntegrity(ytdlpsha256, Environment.CurrentDirectory + @"/Dependencies/yt-dlp.exe");
                    bool ffmpegOK = DependencyManagment.VerifyFileIntegrity(ffmpegsha256, Environment.CurrentDirectory + @"/Dependencies/ffmpeg.exe");
     
                    if (!ytdlpOK || !ffmpegOK)
                    {
                        Console.WriteLine("The program dependencies are corrupted! Redownloading them...");
                        DependencyManagment.GETWindowsDepedencies();
                    }
                }
                catch
                {
                    Console.WriteLine("The program dependencies are corrupted! Redownloading them...");
                    DependencyManagment.GETWindowsDepedencies();
                }
            }

            Console.WriteLine("Loading Configurations...");

            if (!File.Exists(Environment.CurrentDirectory + @"\config.json"))
            {
                Console.WriteLine("Configuration file does not exist, using default configuration and creating one...");
                ConfigurationManager.CreateConfigs(new Configurations());
                ConfigurationManager.ApplyConfigs(new Configurations());
            }
            else
            {
                ConfigurationManager.ApplyConfigs(ConfigurationManager.LoadConfigs());
            }

            Console.WriteLine("Loading blacklist...");

            if (!File.Exists(Environment.CurrentDirectory + @"\blacklist.json"))
            {
                Console.WriteLine("There isn't a blacklist yet.");
            }
            else
            {
                LoadBlackList.ApplyBlacklist();
            }

            Console.WriteLine(

                    "\nConfigurations applied:\n" +
                    "\t{\n" +
                
                    $"\t\tThreads: {InternalProgramData.BotCount}\n" +
                   
                    $"\t\tMax Repeats Allowed Until Change of Subreddit: {InternalProgramData.MaxRepeatTimes}\n" +

                    $"\t\tTarget Subreddit (n1): {InternalProgramData.TargetSubReddit0}\n" +
                    
                    $"\t\tTarget Subreddit (n2): {InternalProgramData.TargetSubReddit1}\n" +

                    $"\t\tTarget Subreddit (n3): {InternalProgramData.TargetSubReddit2}\n" +
                    
                    $"\t\tTarget Subreddit (n4): {InternalProgramData.TargetSubReddit3}\n" +
                    
                    $"\t\tTarget Subreddit (n5): {InternalProgramData.TargetSubReddit4}\n" +

                    $"\t\tTarget Subreddit (n6): {InternalProgramData.TargetSubReddit5}\n" +

                    "\t}\n"  );

            Console.WriteLine("Preparing Environment...");

                PrepareEnvironment.PrepareEnvironmentVariables();            

            Console.WriteLine("Making Dirs...");

            List<string> folders = new();

                folders.Add(Environment.CurrentDirectory + $"/Downloaded Content/");
                folders.Add(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit0}"); 
                folders.Add(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit1}"); 
                folders.Add(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit2}");
                folders.Add(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit3}");
                folders.Add(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit4}");
                folders.Add(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit5}");

            PrepareEnvironment.CreateFolders(folders);


            Console.WriteLine("Starting Hell! \n\nNOTE N1: Don't worry if you don't see any output on this console, that is because the program is finding a lot of repetitions and it isn't logging them :)");
            Console.WriteLine($"NOTE N2: You can quit on ANY moment pressing ANY key on the keyboard, doing it this way provides a lower chance of ending up with corrupted downloads (The program closes after 25 seconds)\n\n\n\n");

            //Start the Bots
            PrepareEnvironment.StartBots();

            InternalProgramData.WaitToKill = true;

            //Executes GC when Memory tops 1GB -> On every execution it raises the limit by 64MB
            new Thread(() => OptimizeMemory.CallGC(1024)).Start();

            //Manages the change between subreddits. 0 -> 1 -> 2 -> 3 -> 4 -> 5  
            new Thread(() => DynamicDownloader.RepeatWatchdog()).Start();

            //Write blacklist while the game is running
            new Thread(() => WriteBlackList.WriteBlacklistWhileRunning()).Start();
        }
    }
}


            