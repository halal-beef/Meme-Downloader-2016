namespace Dottik.MemeDownloader
{
    internal struct InternalProgramData
    {
        public static int BotCount = 32;
        public static string TargetSubReddit = "";
        public static bool STOPPROGRAM = false;
        //public static CookieContainer CookieContainer = new();
    }
    internal struct BotStatus
    {
        public static List<bool> aliveBots = new();
    }
    internal struct Configurations 
    {
        public string TargetSubreddit = "shitposting";
        public int ThreadCount = 32;
    }
}
