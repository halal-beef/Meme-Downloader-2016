namespace Dottik.MemeDownloader
{ 
    internal class MergeAudioAndVideo
    {
        public static void UseFFMPEG(string pathToFFMPEG, string PathToAudio, string PathToVideo, string FinalFilePath)
        {
            Process proc = new();
            ProcessStartInfo startInfo = new();

            startInfo.FileName = pathToFFMPEG;
            startInfo.Arguments = $" -i \"{PathToVideo}\" -i \"{PathToAudio}\" -c copy \"{FinalFilePath}.mkv\"";

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;

            proc.StartInfo = startInfo;

            proc.Start();

            proc.WaitForExit();
        }
    }
}
