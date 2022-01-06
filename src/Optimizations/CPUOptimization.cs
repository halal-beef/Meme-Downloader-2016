namespace Dottik.MemeDownloader.Optimize
{
    internal class ProcessOptimizer
    {
        public static void KillOrphanProcessPID(int PID)
        {
            Process proc = new();
            ProcessStartInfo info = new();

            info.FileName = "taskkill";
            info.Arguments = $" /f /pid {PID}";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            proc.StartInfo = info;

            proc.Start();

            proc.WaitForExit();
        }
        /// <summary>
        /// Blacklist URL from being downloaded, use only if necessary, adding many links will make the program slower!
        /// </summary>
        /// <param name="Link">The Link To Blacklist</param>
        public static void AddToBlacklist(string Link)
        {
            Console.WriteLine($"The URL \"{Link}\" Was Blacklisted.");
            BotInformation.BlackListed.Add(Link);
        }
    }
}
