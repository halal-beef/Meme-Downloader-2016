namespace Dottik.MemeDownloader
{
    internal class EndExecution
    {
        public static void TerminateProgram()
        {
            Console.WriteLine("Waiting for downloads to finish...");
            //Wait 25 seconds to allow most pending downloads to complete and close the application
            Thread.Sleep(25000);

            //Write Blacklist
            WriteBlackList.WriteBlacklist();

            Environment.Exit(0);
        }
    }
}
