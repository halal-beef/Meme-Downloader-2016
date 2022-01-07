namespace Dottik.MemeDownloader.Dependencies
{
    internal class DependencyManagment
    {
        public static void GETWindowsDepedencies()
        {

            Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/");
            Directory.CreateDirectory(Environment.CurrentDirectory + @"/Dependencies/");

            Thread getFFMPEG = new(() =>
            {
                string FFMPEGZIPPATH = Environment.CurrentDirectory + @"/TEMP/FFMPEG-masterx64win.zip";
                using (FileStream fs0 = File.Create(FFMPEGZIPPATH))
                {
                    HttpResponseMessage hrm4 = new HttpClient(handler: InternalProgramData.handler).GetAsync("https://github.com/BtbN/FFmpeg-Builds/releases/download/autobuild-2022-01-06-12-21/ffmpeg-N-105193-g2b541b8c1d-win64-gpl.zip").GetAwaiter().GetResult();
                    hrm4.Content.CopyToAsync(fs0).GetAwaiter().GetResult();
                }
                ZipFile.ExtractToDirectory(FFMPEGZIPPATH, Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/");
                File.Move(Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/ffmpeg-N-105193-g2b541b8c1d-win64-gpl/bin/ffmpeg.exe", Environment.CurrentDirectory + @"/Dependencies/ffmpeg.exe", true);
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
                Thread.Sleep(1000);
            }

            Console.Write("\nDone! Proceeding with program execution in 5 seconds");

            new Thread(() =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Console.Write(".");
                        Thread.Sleep(1666);
                    }
                }).Start();


            Thread.Sleep(5000);

            if(!InternalProgramData.runningCi)
                Console.Clear();
        }
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
                    LOGI($"Hash for file \"{PathToFile}\" is the same as the expected \"{Expectedsha256}\"");
                    return true;
                }
                else
                {
                    LOGW($"Hash for file \"{PathToFile}\" is different than the expected \"{Expectedsha256}\"");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LOGE($"Fail Calculating Hash for file \"{PathToFile}\". Exception Message: {ex.Message}");
                return false;
            }
        }
    }
}
