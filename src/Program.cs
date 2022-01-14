namespace Dottik.MemeDownloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("-ci"))
                {
                    Thread KillOnCI = new(() =>
                    {
                        //Wait 2 Minutes and kill the program. Made to test the program
                        Thread.Sleep(60 * 1000 * 2);
                        EndExecution.TerminateProgram();
                    });
                    
                    KillOnCI.Priority = ThreadPriority.BelowNormal;
                    KillOnCI.Name = $"Github CI Meme Downloader 2016 Test Ran on: {DateTime.Now}";
                    KillOnCI.IsBackground = false;
                    KillOnCI.Start();
                    InternalProgramData.runningCi = true;
                }
            }
            RunLogic();
        }
        private static void RunLogic()
        {
            //Adds the possibility of closing after opening the program
            if (InternalProgramData.runningCi == false)
            {
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
            }

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

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DependencyManagment.GETWindowsDependencies();
                } 
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    DependencyManagment.GETLinuxDependencies();
                }
            }
            else 
            {
                Console.WriteLine("Verifying Install...");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DependencyManagment.VerifyInstallWindows();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    DependencyManagment.VerifyInstallLinux();
                }
                
            }

            if (!File.Exists(Environment.CurrentDirectory + @"/config.json"))
            {
                Console.WriteLine("Configuration file does not exist, using default configuration and creating one...");
                ConfigurationManager.CreateConfigs(new Configurations());
                ConfigurationManager.ApplyConfigs(new Configurations());
            }
            else
            {
                Console.WriteLine("Loading Configurations...");
                ConfigurationManager.ApplyConfigs(ConfigurationManager.LoadConfigs());
            }

            if (!File.Exists(Environment.CurrentDirectory + @"/blacklist.json"))
            {
                Console.WriteLine("There isn't a blacklist available.");
            }
            else
            {
                Console.WriteLine("Loading blacklist...");
                LoadBlackList.ApplyBlacklist();
            }

            Console.WriteLine(

                    "\nConfigurations applied:\n" +
                    "{\n" +
                
                    $"\tThreads: {InternalProgramData.BotCount}\n" +
                   
                    $"\tMax Repeats Allowed Until Change of Subreddit: {InternalProgramData.MaxRepeatTimes}\n" +

                    $"\tTarget Subreddit (n1): {InternalProgramData.TargetSubReddit0}\n" +
                  
                    $"\tTarget Subreddit (n2): {InternalProgramData.TargetSubReddit1}\n" +

                    $"\tTarget Subreddit (n3): {InternalProgramData.TargetSubReddit2}\n" +
                    
                    $"\tTarget Subreddit (n4): {InternalProgramData.TargetSubReddit3}\n" +
                    
                    $"\tTarget Subreddit (n5): {InternalProgramData.TargetSubReddit4}\n" +

                    $"\tTarget Subreddit (n6): {InternalProgramData.TargetSubReddit5}\n" +

                    "}\n"  );

            Console.WriteLine("Preparing Environment...");

                PrepareEnvironment.PrepareEnvironmentVariables();            

            Console.WriteLine("Making Dirs...");

                StringBuilder folders = new(); // Use | as a Separator

                    folders.Append(Environment.CurrentDirectory + $"/Downloaded Content/|");
                    folders.Append(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit0}/|"); 
                    folders.Append(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit1}/|"); 
                    folders.Append(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit2}/|");
                    folders.Append(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit3}/|");
                    folders.Append(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit4}/|");
                    folders.Append(Environment.CurrentDirectory + $"/Downloaded Content/{InternalProgramData.TargetSubReddit5}/");

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


            