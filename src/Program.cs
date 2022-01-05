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
            BotHandler botSys = new();
            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Shitposs");

            Console.WriteLine("Starting Hell!");

            for (int i = 0; i < InternalProgramData.BotCount; i++)
            {
                //Execute bots according to the ammount specified on BotCount 🥶👌
                Thread x = new(() => botSys.StartBot(i * 2));
                x.Name = "Shitpost bot n" + i;
                x.IsBackground = true;
                x.Start();
            }


            new Thread(() => OptimizeMemory.CallGC()).Start();

            Console.ReadKey();
            Console.Clear();

            InternalProgramData.STOPPROGRAM = true;

            //Kill Orphan ffmpeg processes
            ProcessOptimizer.KillOrphanProcessProcName("ffmpeg.exe");
            Thread.Sleep(10000);
        }
    }
}


            