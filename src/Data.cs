namespace Dottik.MemeDownloader
{
    internal struct InternalProgramData
    {
        public static int BotCount = 32;
        public static string TargetSubReddit = "";
        public static bool STOPPROGRAM = false;
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
        public static List<bool> aliveBots = new();
    }
    internal struct Configurations 
    {
        public string TargetSubreddit = "shitposting";
        public int ThreadCount = 32;
    }
}
