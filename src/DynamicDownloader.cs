using static Dottik.MemeDownloader.InternalProgramData;

namespace Dottik.MemeDownloader
{
    internal class DynamicDownloader
    {
        public static void RepeatWatchdog()
        {
            string initialValueOfTSR0 = TargetSubReddit0, initialValueOfTSR1 = TargetSubReddit1;
            while (true)
            {
                //Runs code every 15s
                Thread.Sleep(15000);

                while (TimesRepeated >= MaxRepeatTimes)
                {
                    if (!SimultaneousDownload) 
                    {
                        if (TargetSubReddit0 == initialValueOfTSR0)
                        {
                            Console.WriteLine($"\n\nChanging download target!\n/r/{TargetSubReddit0} -> /r/{TargetSubReddit1}\n\n");
                            TargetSubReddit0 = TargetSubReddit1;
                        }
                        else if (TargetSubReddit0 == TargetSubReddit1)
                        {
                            Console.WriteLine($"\n\nChanging download target!\n/r/{TargetSubReddit0} -> /r/{TargetSubReddit2}\n\n");
                            TargetSubReddit0 = TargetSubReddit2;
                        }
                        else if (TargetSubReddit0 == TargetSubReddit2)
                        {
                            Console.WriteLine($"\n\nChanging download target!\n/r/{TargetSubReddit0} -> /r/{TargetSubReddit3}\n\n");
                            TargetSubReddit0 = TargetSubReddit3;
                        }
                        else if (TargetSubReddit0 == TargetSubReddit3)
                        {
                            Console.WriteLine($"\n\nChanging download target!\n/r/{TargetSubReddit0} -> /r/{TargetSubReddit4} | Last Subreddit to download!\n\n");
                            TargetSubReddit0 = TargetSubReddit4;
                        } 
                        else if (TargetSubReddit0 == TargetSubReddit4)
                        {
                            Console.WriteLine("Terminating Program. The program couldn't find more available resources to download.");
                            InternalProgramData.STOPPROGRAM = true;

                            //Kill Orphan ffmpeg processes after 25 seconds and close the application
                            Thread.Sleep(25000);
                            ProcessOptimizer.KillOrphanProcessProcName("ffmpeg.exe");
                            Environment.Exit(0);
                        }
                    } 
                    else
                    {
                        if ( TargetSubReddit0 == initialValueOfTSR0 && TargetSubReddit1 == initialValueOfTSR1)
                        {
                            Console.WriteLine($"\n\nChanging download targets!\n/r/{TargetSubReddit0} -> /r/{TargetSubReddit2}\n /r/{TargetSubReddit1} -> /r/{TargetSubReddit3}\n\n");
                            TargetSubReddit0 = TargetSubReddit2;
                            TargetSubReddit1 = TargetSubReddit3;
                        }
                        else if (TargetSubReddit0 == TargetSubReddit2 && TargetSubReddit1 == TargetSubReddit3)
                        {
                            Console.WriteLine($"\n\nChanging download targets!\n/r/{TargetSubReddit0} -> /r/{TargetSubReddit4}  | Last Subreddit to download!\n /r/{TargetSubReddit1} -> /r/{initialValueOfTSR0} | Last Subreddit to download!\n\n");
                            TargetSubReddit0 = TargetSubReddit4;
                            TargetSubReddit1 = initialValueOfTSR0;
                        }
                        else
                        {
                            Console.WriteLine("||===========||| Terminating Program. The program couldn't find more available resources to download. |||==========|||");
                            InternalProgramData.STOPPROGRAM = true;

                            //Kill Orphan ffmpeg processes after 25 seconds and close the application
                            Thread.Sleep(25000);
                            ProcessOptimizer.KillOrphanProcessProcName("ffmpeg.exe");
                            Environment.Exit(0);
                        }
                    }
                    TimesRepeated = 0;
                }
            }
        }
    }
}
