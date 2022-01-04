namespace Dottik.MemeDownloader
{
    internal struct InternalProgramData
    {
        public static readonly int BotCount = 64;
        public static bool STOPPROGRAM = false;
    }
    internal struct BotStatus
    {
        public static List<bool> aliveBots = new();
    }
}
