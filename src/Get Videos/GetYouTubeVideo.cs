namespace Dottik.MemeDownloader.GetMedia
{
    internal class YouTubeDLP
    {

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string VideoLink { get; set; }
        private static Object lockytdlp = new();
        public void GetVideoAsMP4(string OutputPath)
        {
            lock (lockytdlp)
            {
                Process ytdlpProcess = new();
                ProcessStartInfo pSIytdlp = new();

                //We added Dependency folder path to process PATH env!
                pSIytdlp.FileName = "yt-dlp.exe";
                pSIytdlp.Arguments = $"--no-check-certificates --geo-bypass --no-playlist --concurrent-fragments 1 --hls-use-mpegts --no-windows-filenames --remux-video mp4 -o \"{OutputPath}\" \"{VideoLink}\"";
                //pSIytdlp.CreateNoWindow = true;
                //pSIytdlp.WindowStyle = ProcessWindowStyle.Hidden;

                ytdlpProcess.StartInfo = pSIytdlp;

                ytdlpProcess.Start();
                ytdlpProcess.WaitForExit();
            }
        }
    }
}
