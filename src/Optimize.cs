namespace Dottik.MemeDownloader.Optimize
{
    internal class OptimizeMemory
    {
        public static bool collectionOnProgress = false;
        /// <summary>
        /// Triggers a Garbage Collection
        /// </summary>
        /// <param name="RamLimit">The RAM on Megabytes that the program needs to consume to trigger the Garbage Collector!</param>
        public static void CallGC(int RamLimit)
        {
            while (!InternalProgramData.STOPPROGRAM)
            {
                Process pro = Process.GetCurrentProcess();
                bool shouldCollect = false;

                //Calls GC when memory is more than 1024MB
                if (pro.WorkingSet64 >= RamLimit * 1000000)
                {
                    shouldCollect = true;
                }

                if (shouldCollect)
                {
                    //Announce collection!
                    collectionOnProgress = true;
                    Thread.Sleep(1000);
                    Console.WriteLine("Garbage Collection Working. Please Hold!");
                    

                    GC.Collect();
                    collectionOnProgress = false;
                }
            }

        }
    }
    internal class ProcessOptimizer
    {
        public static void KillOrphanProcessProcName(string ProcessName)
        {
            Process proc = new();
            ProcessStartInfo info = new();

            info.FileName = "taskkill";
            info.Arguments = $" /f /im {ProcessName}";

            proc.StartInfo = info;

            proc.Start();

            proc.WaitForExit();
        }
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
    }
}
