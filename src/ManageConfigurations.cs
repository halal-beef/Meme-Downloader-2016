namespace Dottik.MemeDownloader
{
    internal class ConfigurationManager
    {
        public static Configurations LoadConfigs()
        {
            string Path0 = Environment.CurrentDirectory + @"\config.json";
            Console.WriteLine($"Config File found in: {Path0}");

            Configurations deserialized = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(Path0));

            return deserialized;
        }
        public static void CreateConfigs(Configurations configs)
        {
            string Path0 = Environment.CurrentDirectory + @"\config.json";

            string defaultConfigs = JsonConvert.SerializeObject(configs);
            Console.WriteLine($"Creating Config File on: {Path0}");

            File.AppendAllText(Path0, defaultConfigs);
        }
        public static void ApplyConfigs(Configurations configs)
        {
            InternalProgramData.BotCount = configs.ThreadCount;

            InternalProgramData.TargetSubReddit0 = configs.TargetSubReddit0;
            InternalProgramData.TargetSubReddit1 = configs.TargetSubReddit1;
            InternalProgramData.TargetSubReddit2 = configs.TargetSubReddit2;
            InternalProgramData.TargetSubReddit3 = configs.TargetSubReddit3;
            InternalProgramData.TargetSubReddit4 = configs.TargetSubReddit4;
            InternalProgramData.TargetSubReddit5 = configs.TargetSubReddit5;


            if (!InternalProgramData.runningCi)
            {
                InternalProgramData.MaxRepeatTimes = configs.MaxRepeatTimes;
            } 
            else
            {
                InternalProgramData.MaxRepeatTimes = 100;
            }
            InternalProgramData.SimultaneousDownload = configs.SimultaneousDownload;
        }
    }
}
