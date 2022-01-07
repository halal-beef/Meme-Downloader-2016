namespace Dottik.MemeDownloader
{
    internal struct Blacklist
    {
        public List<string> BlacklistedURLs = new();
    }
    internal class WriteBlackList
    {
        public static void WriteBlacklist()
        {
            Blacklist blackListData = new();
            string BlackListPath = Environment.CurrentDirectory + @"/blacklist.json";

            blackListData.BlacklistedURLs = BotInformation.BlackListed;

            string jsonContent = JsonConvert.SerializeObject(blackListData);

            File.WriteAllTextAsync(BlackListPath, jsonContent, Encoding.UTF8).GetAwaiter().GetResult();
            Console.WriteLine($"Creating Blacklist on: {BlackListPath}");
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
