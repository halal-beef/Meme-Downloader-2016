namespace Dottik.MemeDownloader
{
    internal struct Blacklist
    {
        public List<string> BlacklistedURLs = new();
    }
    internal class WriteBlackList
    {
        public static Object blacklistLocker = new();
        public static void WriteBlacklistWhileRunning()
        {
            while (true)
            {
                //Write Blacklist every 30 seconds while the program is running
                Thread.Sleep(30000);

                lock (blacklistLocker)
                {
                    Blacklist blackListData = new();
                    string BlackListPath = Environment.CurrentDirectory + @"/blacklist.json";

                    blackListData.BlacklistedURLs = BotInformation.BlackListed;

                    string jsonContent = JsonConvert.SerializeObject(blackListData);

                    File.WriteAllText(BlackListPath, jsonContent, Encoding.UTF8);
                    Console.WriteLine($"Writing Blacklist to {BlackListPath}...");
                }
            }
        }

    }
    internal class LoadBlackList
    {
        public static void ApplyBlacklist()
        {
            string BlackListPath = Environment.CurrentDirectory + @"/blacklist.json";

            string data = File.ReadAllTextAsync(BlackListPath).GetAwaiter().GetResult();
            Console.WriteLine($"Blacklist found in: {BlackListPath}");

            Blacklist blackListData = JsonConvert.DeserializeObject<Blacklist>(data);

            BotInformation.BlackListed = blackListData.BlacklistedURLs;
        }
    }
}
