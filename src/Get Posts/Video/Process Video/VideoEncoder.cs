namespace Dottik.MemeDownloader
{ 
    internal class MergeAudioAndVideo : ManageBrokenMedia
    {
        /// <summary>
        /// Use if there isn't a video & audio track available and there is only Audio available
        /// </summary>
        /// <param name="PathToMedia">Path the media file (Absolute path)</param>
        /// <param name="FinalFilePath">Where to drop the final file (Absolute Path)</param>
        /// <param name="URL">Add URL to Blacklist if ffmpeg is unable to complete operation</param>
        public static void UseFFMPEG(string PathToMedia, string FinalFilePath, string URL)
        {
            ManageBrokenMedia MbM = new();
            Process proc = new();
            ProcessStartInfo startInfo = new();

            startInfo.FileName = "ffmpeg.exe";
            startInfo.Arguments = $"-i \"{PathToMedia}\" \"{FinalFilePath}\"";

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.StartInfo = startInfo;
            proc.Start();
            Thread MbMBlackAndTerm = new(() => MbM.BlackListAndTerminate(URL, proc));
            MbMBlackAndTerm.Start();


            proc.WaitForExit();

            MbM.finished = true;
        }
        public static void UseFFMPEG(string PathToAudio, string PathToVideo, string FinalFilePath, string URL)
        {
            ManageBrokenMedia MbM = new();
            Process proc = new();
            ProcessStartInfo startInfo = new();

            startInfo.FileName = "ffmpeg.exe";
            startInfo.Arguments = $"-i \"{PathToVideo}\" -i \"{PathToAudio}\" \"{FinalFilePath}\"";

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.StartInfo = startInfo;
            proc.Start();
            Thread MbMBlackAndTerm = new(() => MbM.BlackListAndTerminate(URL, proc));
            MbMBlackAndTerm.Start();

            proc.WaitForExit();
            MbM.finished = true;
        }
    }
    internal class ManageBrokenMedia
    {
        public bool finished = false;
        public void BlackListAndTerminate(string URL, Process ProcessToKill)
        {
            Thread.Sleep(25 * 1000);

            if (!finished)
            {
                ProcessOptimizer.AddToBlacklist(URL);
                ProcessToKill.Kill(true);
            }
        }
    }
}
