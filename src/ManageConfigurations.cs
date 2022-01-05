namespace Dottik.MemeDownloader
{
    internal class ConfigurationManager
    {
        public static Configurations LoadConfigs()
        {
            string Path0 = Environment.CurrentDirectory + @"\config.json";
            Console.WriteLine(Path0);

            Configurations deserialized = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(Path0));

            return deserialized;
        }
        public static void CreateConfigs()
        {
            Configurations configs = new();
            string Path0 = Environment.CurrentDirectory + @"\config.json";

            string defaultConfigs = JsonConvert.SerializeObject(configs);
            Console.WriteLine(Path0);

            File.AppendAllText(Path0, defaultConfigs);
        }
        public static void ApplyConfigs(Configurations configs)
        {
            InternalProgramData.BotCount = configs.ThreadCount;
            InternalProgramData.TargetSubReddit = configs.TargetSubreddit;
        }
    }
}
