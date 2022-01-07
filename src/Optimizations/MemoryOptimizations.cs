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

                //Calls GC when memory is more than {RamLimit}MB

                if (pro.WorkingSet64 >= RamLimit * 1000000)
                {
                    shouldCollect = true;
                }

                if (shouldCollect)
                {
                    //Announce collection!
                    collectionOnProgress = true;
                    Thread.Sleep(1000);
                    Console.WriteLine("Memory Limit Reached! Garbage Collection Triggered!");
                    
                    GC.Collect();

                    //Sleep for 10 secs & Add 64MB more to the RamLimit
                    Thread.Sleep(10000);
                    RamLimit += 64;
                    collectionOnProgress = false;
                }
            }

        }
    }
}
