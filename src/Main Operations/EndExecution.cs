namespace Dottik.MemeDownloader
{
    internal class EndExecution
    {
        public static void TerminateProgram()
        {
            //Wait 25 seconds to allow most pending downloads to complete and close the application
            Thread.Sleep(25000);
            Environment.Exit(0);
        }
    }
}
