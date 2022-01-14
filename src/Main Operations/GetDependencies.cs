namespace Dottik.MemeDownloader.Dependencies
{
    internal class DependencyManagment
    {
        private static Object chmodLock = new();
        public static void GETLinuxDependencies()
        {
            //Declare chmod.
            Process chmod = new();
            chmod.StartInfo.FileName = "chmod";
            chmod.StartInfo.CreateNoWindow = true;
            chmod.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            chmod.StartInfo.UseShellExecute = true;

            Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/");
            Directory.CreateDirectory(Environment.CurrentDirectory + @"/Dependencies/");

            Thread getlinuxFFMPEG = new(() =>
            {
                //https://github.com/usrDottik/Stuff/releases/download/fmpglin/ffmpeg
                string FFMPEGPATH = Environment.CurrentDirectory + @"/Dependencies/ffmpeg";
                using (FileStream fs0 = File.Create(FFMPEGPATH))
                {
                    HttpResponseMessage hrm4 = InternalProgramData.client.GetAsync("https://github.com/usrDottik/Stuff/releases/download/fmpglin/ffmpeg").GetAwaiter().GetResult();
                    hrm4.Content.CopyToAsync(fs0).GetAwaiter().GetResult();
                }
                File.SetAttributes(FFMPEGPATH, FileAttributes.Normal);

                lock (chmodLock)
                {
                    chmod.StartInfo.Arguments = $"+x {FFMPEGPATH}";
                    chmod.Start();
                    chmod.WaitForExit();
                }
            });

            Thread getlinuxFYTDLP = new(() =>
            {
                string YTDLPPATH = Environment.CurrentDirectory + @"/Dependencies/yt-dlp";

                using FileStream fs0 = File.Create(YTDLPPATH);
                    HttpResponseMessage hrm5 = InternalProgramData.client.GetAsync("https://github.com/yt-dlp/yt-dlp/releases/download/2021.12.27/yt-dlp").GetAwaiter().GetResult();
                    hrm5.Content.CopyToAsync(fs0).GetAwaiter().GetResult();
                    fs0.Dispose();
                    fs0.Close();

                File.SetAttributes(YTDLPPATH, FileAttributes.Normal);

                lock (chmodLock)
                {
                    chmod.StartInfo.Arguments = $"+x {YTDLPPATH}";
                    chmod.Start();
                    chmod.WaitForExit();
                }
            });

            getlinuxFYTDLP.Name = "Get yt-dlp from Github Build!";

            getlinuxFFMPEG.Name = "Get FFMPEG from Github Build!";
            getlinuxFYTDLP.Start();
            getlinuxFFMPEG.Start();

            Console.Write("Getting ffmpeg and YouTube-dlp");
            while (getlinuxFFMPEG.IsAlive || getlinuxFYTDLP.IsAlive)
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

            if (!InternalProgramData.runningCi)
                Console.Clear();
        }
        public static void GETWindowsDependencies()
        {

            Directory.CreateDirectory(Environment.CurrentDirectory + @"/TEMP/");
            Directory.CreateDirectory(Environment.CurrentDirectory + @"/Dependencies/");

            Thread getwinFFMPEG = new(() =>
            {
                string FFMPEGZIPPATH = Environment.CurrentDirectory + @"/TEMP/FFMPEG-masterx64win.zip";
                using (FileStream fs0 = File.Create(FFMPEGZIPPATH))
                {
                    HttpResponseMessage hrm4 = InternalProgramData.client.GetAsync("https://github.com/BtbN/FFmpeg-Builds/releases/download/autobuild-2022-01-06-12-21/ffmpeg-N-105193-g2b541b8c1d-win64-gpl.zip").GetAwaiter().GetResult();
                    hrm4.Content.CopyToAsync(fs0).GetAwaiter().GetResult();
                }
                ZipFile.ExtractToDirectory(FFMPEGZIPPATH, Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/");
                File.Move(Environment.CurrentDirectory + @"/TEMP/FFMPEGFILES/ffmpeg-N-105193-g2b541b8c1d-win64-gpl/bin/ffmpeg.exe", Environment.CurrentDirectory + @"/Dependencies/ffmpeg.exe", true);
            });

            Thread getwinYTDLP = new(() =>
            {
                string YTDLPPATH = Environment.CurrentDirectory + @"/Dependencies/yt-dlp.exe";

                using FileStream fs0 = File.Create(YTDLPPATH);
                HttpResponseMessage hrm5 = InternalProgramData.client.GetAsync("https://github.com/yt-dlp/yt-dlp/releases/download/2021.12.27/yt-dlp.exe").GetAwaiter().GetResult();
                hrm5.Content.CopyToAsync(fs0).GetAwaiter().GetResult();

            });

            getwinYTDLP.Name = "Get yt-dlp from Github Build!";

            getwinFFMPEG.Name = "Get FFMPEG from Github Build!";
            getwinYTDLP.Start();
            getwinFFMPEG.Start();

            Console.Write("Getting ffmpeg and YouTube-dlp");
            while (getwinFFMPEG.IsAlive || getwinYTDLP.IsAlive)
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
        private static bool VerifyFileIntegrity(string Expectedsha256, string PathToFile)
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
        public static void VerifyInstallWindows()
        {
            try
            {
                const string
                    ytdlpsha256 = "1A34F627CA88251BE6D17940F026F6A5B8AAAF0AA32DD60DEEC3020F81950E67",
                   ffmpegsha256 = "436844A3ECF9B2ECC13E57B2B5D000ADBB6FEA6FE99C7D5921C99284A91C50DC";

                bool ytdlpOK = VerifyFileIntegrity(ytdlpsha256, Environment.CurrentDirectory + @"/Dependencies/yt-dlp.exe");
                bool ffmpegOK = VerifyFileIntegrity(ffmpegsha256, Environment.CurrentDirectory + @"/Dependencies/ffmpeg.exe");

                if (!ytdlpOK || !ffmpegOK)
                {
                    Console.WriteLine("The program dependencies are corrupted! Redownloading them...");
                    DependencyManagment.GETWindowsDependencies();
                }
            }
            catch
            {
                Console.WriteLine("Some program dependencies are missing! Redownloading them...");
                DependencyManagment.GETWindowsDependencies();
            }
        }
        public static void VerifyInstallLinux()
        {
            try
            {
                const string
                    ytdlpsha256 = "254289d79a896b828720e3120bbdd00e48546009cfabbe5d86fa4bb9f9e77d48",
                   ffmpegsha256 = "B8ABA52A98315C8B23917CCCEFA86D11CD2D630C459009FECECE3752AD2155DC";

                bool ytdlpOK = VerifyFileIntegrity(ytdlpsha256, Environment.CurrentDirectory + @"/Dependencies/yt-dlp");
                bool ffmpegOK = VerifyFileIntegrity(ffmpegsha256, Environment.CurrentDirectory + @"/Dependencies/ffmpeg");

                if (!ytdlpOK || !ffmpegOK)
                {
                    Console.WriteLine("The program dependencies are corrupted! Redownloading them...");
                    DependencyManagment.GETLinuxDependencies();
                }
            }
            catch
            {
                Console.WriteLine("Some program dependencies are missing! Redownloading them...");
                DependencyManagment.GETLinuxDependencies();
            }
        }
    }
}
