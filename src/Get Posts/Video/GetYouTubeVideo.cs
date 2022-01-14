namespace Dottik.MemeDownloader.GetMedia
{
    internal class YouTubeDLP
    {
        public StringBuilder VideoLink { get; set; } = new("");
        private static readonly Object lockytdlp = new();
        public void GetVideoAsMP4(string OutputPath)
        {
            lock (lockytdlp)
            {
                Process ytdlpProcess = new();
                ProcessStartInfo pSIytdlp = new();

                //We added Dependency folder path to process PATH env!
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    pSIytdlp.FileName = "yt-dlp.exe";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    pSIytdlp.FileName = $"{Environment.CurrentDirectory}/Dependencies/yt-dlp";
                    pSIytdlp.UseShellExecute = true;
                }
                pSIytdlp.Arguments = $"--no-check-certificates --geo-bypass --no-playlist --concurrent-fragments 2 --hls-use-mpegts --no-windows-filenames --remux-video mp4 -o \"{OutputPath}\" \"{VideoLink}\"";

                pSIytdlp.CreateNoWindow = true;
                pSIytdlp.WindowStyle = ProcessWindowStyle.Hidden;

                ytdlpProcess.StartInfo = pSIytdlp;

                ytdlpProcess.Start();
                ytdlpProcess.WaitForExit();
            }
        }
    }
}
