namespace Dottik.MemeDownloader.Optimize
{
    internal class ProcessOptimizer
    {
        public static void KillOrphanProcess(Process targetProcess)
        {
            targetProcess.Kill(true);
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
