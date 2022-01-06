namespace Dottik.MemeDownloader
{ 
    internal class MergeAudioAndVideo
    {
        /// <summary>
        /// Use if there isn't a video & audio track available and there is only Audio available
        /// </summary>
        /// <param name="PathToMedia">Path the media file (Absolute path)</param>
        /// <param name="FinalFilePath">Where to drop the final file (Absolute Path)</param>
        public static void UseFFMPEG(string PathToMedia, string FinalFilePath)
        {
            Process proc = new();
            ProcessStartInfo startInfo = new();

            startInfo.FileName = "ffmpeg.exe";
            startInfo.Arguments = $"-i \"{PathToMedia}\" \"{FinalFilePath}\"";

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.StartInfo = startInfo;
            proc.Start();
            int pid = proc.Id;

            new Thread(() =>
            {
                KIllIfNotDead(pid);
            }).Start();

            proc.WaitForExit();
        }
        public static void UseFFMPEG(string PathToAudio, string PathToVideo, string FinalFilePath)
        {
            Process proc = new();
            ProcessStartInfo startInfo = new();

            startInfo.FileName = "ffmpeg.exe";
            startInfo.Arguments = $"-i \"{PathToVideo}\" -i \"{PathToAudio}\" \"{FinalFilePath}\"";

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.StartInfo = startInfo;
            proc.Start();
            int pid = proc.Id;
            
            new Thread(() =>
            {
                KIllIfNotDead(pid);
            }).Start();

            proc.WaitForExit();
        }
        public static void KIllIfNotDead(int PID)
        {
            Thread.Sleep(25000);
            ProcessOptimizer.KillOrphanProcessPID(PID);
        }
    }
}
