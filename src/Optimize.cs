namespace Dottik.MemeDownloader.Optimize
{
    internal class OptimizeMemory
    {
        public static bool collectionOnProgress = false;
        public static void CallGC()
        {
            while (true)
            {
                Process pro = Process.GetCurrentProcess();
                bool shouldCollect = false;

                //Calls GC when memory is more than 512MB
                if (pro.WorkingSet64 >= 512000000)
                {
                    shouldCollect = true;
                }

                if (shouldCollect)
                {
                    //Announce collection!
                    collectionOnProgress = true;

                    Console.WriteLine("Garbage Collection Working. Please Hold!");
                    Thread.Sleep(5000);

                    GC.Collect();
                    collectionOnProgress = false;
                }
            }

        }
    }
}
