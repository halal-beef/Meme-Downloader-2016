namespace Dottik.MemeDownloader
{
    internal class EndExecution
    {
        public static void TerminateProgram()
        {
            Console.WriteLine("Waiting for downloads to finish...");
            //Wait 35 seconds to allow most pending downloads and the latest blacklist possible to complete and close the application
            Thread.Sleep(35000);

            lock (WriteBlackList.blacklistLocker) {
                Environment.Exit(0);
            }
        }
    }
}
