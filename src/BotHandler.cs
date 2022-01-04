namespace Dottik.MemeDownloader.Bots
{
    internal class BotHandler
    {
        public static Object locked = new Object();
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        public void StartBot(int timeOut = 50)
        {
            try
            {
                lock (locked)
                {  
                    BotStatus.aliveBots.Add(true);
                }
                Random rand = new();

                while (!InternalProgramData.STOPPROGRAM)
                {
                    if (!OptimizeMemory.collectionOnProgress)
                    {
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));
                        var Result = JObject.Parse(
                            JArray.Parse(

                                new HttpClient()

                                .GetStringAsync("https://reddit.com/r/shitposting/random.json?limit=1").GetAwaiter().GetResult()

                                )[0]["data"]["children"][0]["data"]
                                .ToString()

                            );

                        string usableName = Result["url"].ToString().Replace('/', '_').Replace(':', '.').Replace('?', '[');
                        string PathToResult = Environment.CurrentDirectory + @"/Shitposs/" + usableName;

                        if (!PathToResult.Contains(".jpg") && !PathToResult.Contains(".png") && !PathToResult.Contains(".gif") && !PathToResult.Contains(".jpeg") && !PathToResult.Contains(".mp4"))
                        {
                            PathToResult += ".htm";
                        }


                        if (!File.Exists(PathToResult))
                        {
                            using (FileStream fs = File.Create(PathToResult))
                            {
                                HttpClient httpClient = new();

                                HttpResponseMessage hrm = httpClient.GetAsync(Result["url"].ToString()).GetAwaiter().GetResult();

                                try
                                {
                                    hrm.EnsureSuccessStatusCode();
                                }
                                catch
                                {
                                    Console.WriteLine("YOU DON'T HAVE INTERNET YOU FOOL!");
                                    Environment.Exit(0);
                                }
                                hrm.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                                fs.Dispose();
                                fs.Close();
                            }
                            Console.WriteLine($"{Thread.CurrentThread.Name}; Downloaded {Result["title"]}");
                        }
                    }
                    else
                    {
                        Thread.Sleep(5050);
                    }
                }
                BotStatus.aliveBots.RemoveAt(0);
            }
            catch
            {
                if (!InternalProgramData.STOPPROGRAM)
                {
                    BotStatus.aliveBots.RemoveAt(0);
                    Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}. Bot Terminated, Bots Left: {BotStatus.aliveBots.Count}");
                    CheckAndReviveBots();
                }
            }
        }
        public void CheckAndReviveBots()
        {
            BotHandler botSys = new();

            float totalBots = InternalProgramData.BotCount;
            float aliveBots = BotStatus.aliveBots.Count;
            
            int deadBots = InternalProgramData.BotCount - (int)aliveBots;

            float aliveBotsPercentage = aliveBots / totalBots * 100;

            Console.WriteLine($"{aliveBotsPercentage}% of bots are alive! {deadBots} Bots have died!");
            
            if (float.Parse(aliveBotsPercentage.ToString()) <= 50)
            {
                for (float i = aliveBots; i < totalBots; i++)
                {
                    //Download posts on 64 threads 🥶👌
                    Thread x = new(() => botSys.StartBot((int)i * 2));
                    x.Name = "Shitpost bot n" + i;
                    x.IsBackground = true;
                    x.Start();
                }
                Console.WriteLine($"Created {deadBots} bots, Total Remaining: {BotStatus.aliveBots.Count}");
            }
        }
    }
}
