namespace Dottik.MemeDownloader
{
    internal class EndExecution
    {
        public static void TerminateProgram()
        {
            //Kill Orphan ffmpeg processes after 25 seconds and close the application
            Thread.Sleep(25000);
            Environment.Exit(0);
        }
    }
}
