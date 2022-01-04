namespace Dottik.MemeDownloader
{
    public class Program
    {
        public static void Main()
        {
            BotHandler botSys = new();
            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Shitposs");

            Console.WriteLine("Starting Hell!");

            for (int i = 0; i < InternalProgramData.BotCount; i++)
            {
                //Execute bots according to the ammount specified on BotCount 🥶👌
                Thread x = new(() => botSys.StartBot(i * 2));
                x.Name = "Shitpost bot n" + i;
                x.IsBackground = true;
                x.Start();
            }


            new Thread(() => OptimizeMemory.CallGC()).Start();
            
            
            Console.ReadKey();
        }
    }
}


