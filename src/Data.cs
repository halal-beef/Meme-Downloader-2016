namespace Dottik.MemeDownloader
{
    internal struct InternalProgramData
    {
        public static ulong MaxRepeatTimes = 0;
        public static ulong TimesRepeated = 0;
        
        public static int BotCount = 32;

        public static string TargetFolder = Environment.CurrentDirectory + @"\Shitposs\";
        public static string TargetSubReddit0 = "";
        public static string TargetSubReddit1 = ""; 
        public static string TargetSubReddit2 = "";
        public static string TargetSubReddit3 = "";
        public static string TargetSubReddit4 = "";
        public static string TargetSubReddit5 = "";
        public static bool SimultaneousDownload = false;
        public static bool STOPPROGRAM = false;
        public static bool RestartBot = false;

        //public static CookieContainer CookieContainer = new();

        public static readonly HttpClientHandler handler = new()
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
            UseCookies = true,
            AutomaticDecompression = DecompressionMethods.All
        };
    }
    internal struct BotStatus
    {
        public static List<bool> aliveBots0 = new();
        public static List<bool> aliveBots1 = new();
    }
    internal struct Configurations 
    {
        public string TargetSubReddit0 = "shitposting";
        public string TargetSubReddit1 = "dankmemes";
        public string TargetSubReddit2 = "memes";
        public string TargetSubReddit3 = "crappyoffbrands";
        public string TargetSubReddit4 = "nocontextpics";
        public string TargetSubReddit5 = "funny";
        public ulong MaxRepeatTimes = 10000;
        public bool SimultaneousDownload = true;
        public int ThreadCount = 32;
    }
}
