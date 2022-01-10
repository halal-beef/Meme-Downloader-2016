namespace Dottik.MemeDownloader.Bots
{
    internal class BotHandler
    {
        //Web Related
        private readonly HttpClient _httpClient = new(InternalProgramData.handler);
        private HttpResponseMessage _hrm = new();

        //Locks
        private static readonly Object locked = new();
        private static readonly Object botRespawnLocker = new();
        private static readonly Object locked0 = new();
        private static readonly Object lockGallery = new();


        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        public void StartBot(bool modeA, int timeOut = 50)
        {
            LOGD($"Started new bot with name: {Thread.CurrentThread.Name}.");
            bool isBot0 = false;
            try
            {
                if (modeA) 
                {
                    isBot0 = true;
                    Thread.CurrentThread.Name = $"- /r/{InternalProgramData.TargetSubReddit0}" + Thread.CurrentThread.Name.Split('-')[1];
                    lock (locked)
                    {
                        BotInformation.aliveBots0++;
                    }
                } 
                else
                {
                    Thread.CurrentThread.Name = $"- /r/{InternalProgramData.TargetSubReddit1}" + Thread.CurrentThread.Name.Split('-')[1];
                    lock (locked0)
                    {
                        BotInformation.aliveBots1++;
                    } 
                }
                Random rand = new();

                while (!InternalProgramData.STOPPROGRAM)
                {

                    if (!InternalProgramData.RestartBot && !OptimizeMemory.collectionOnProgress && !InternalProgramData.STOPPROGRAM)
                    {
                       
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));
                        bool requestSuccess = false, blacklisted = false;
                        StringBuilder data = new(""), target = new("");

                        while (!requestSuccess)
                        {
                            try
                            {
                                if (modeA)
                                {
                                    if (!Thread.CurrentThread.Name.Contains(InternalProgramData.TargetSubReddit0))
                                        Thread.CurrentThread.Name = $"- /r/{InternalProgramData.TargetSubReddit0} | " + Thread.CurrentThread.Name.Split('|')[1];
                                    
                                    data.Append(new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit0}/random.json").GetAwaiter().GetResult());
                                    requestSuccess = true;
                                    target.Append(InternalProgramData.TargetSubReddit0);
                                } else
                                {
                                    if (!Thread.CurrentThread.Name.Contains(InternalProgramData.TargetSubReddit1))
                                        Thread.CurrentThread.Name = $"- /r/{InternalProgramData.TargetSubReddit1} | " + Thread.CurrentThread.Name.Split('|')[1];

                                    data.Append(new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit1}/random.json").GetAwaiter().GetResult());
                                    requestSuccess = true;
                                    target.Append(InternalProgramData.TargetSubReddit1);
                                }
                            }
                            catch
                            {
                                if (modeA)
                                {
                                    data.Append(new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit0}/random.json").GetAwaiter().GetResult());
                                    requestSuccess = true;
                                }
                                else
                                {
                                    data.Append(new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit1}/random.json").GetAwaiter().GetResult());
                                    requestSuccess = true;
                                }
                            }
                        }
                        JObject Result = new();
                        try
                        {
                            Result = JObject.Parse(

                                JArray.Parse(data.ToString())[0]["data"]["children"][0]["data"].ToString()

                                );
                        }
                        catch
                        {
                            throw new Exception("Error Parsing JSON!");
                        }
                        StringBuilder usableName = new(""), sourceLink = new("");

                        try
                        {
                            //Avoid Illegal Names
                            usableName.Append(Result["title"].ToString().Trim(InternalProgramData.illegalChars));
                            sourceLink.Append(Result.Value<string>("url_overridden_by_dest"));
                        }
                        catch
                        {
                            //Avoid Illegal Names
                            usableName.Append(Result["title"].ToString().Trim(InternalProgramData.illegalChars));
                            sourceLink.Append(Result["url"].ToString());
                        }
                        StringBuilder PathToResult = new(Environment.CurrentDirectory + @"/Downloaded Content/" + @$"/{target}/" + usableName);

                        for (int i = 0; i < BotInformation.BlackListed.Count; i++)
                        {
                            if (sourceLink.ToString() == BotInformation.BlackListed[i])
                            {
                                blacklisted = true;
                                Console.WriteLine($"{Thread.CurrentThread.Name}; Tried to download a blacklisted URL.");
                            }
                        }

                        if (!blacklisted && sourceLink != null)
                        {
                            if (sourceLink.ToString().Contains("gallery"))
                            {
                                lock (lockGallery)
                                {
                                    FormattedLinks formattedLinks = GetRedditGallery.FormatLinks(data.ToString());

                                    GetRedditGallery.GetGallery(PathToResult.ToString(), sourceLink.ToString(), formattedLinks, _httpClient);
                                }
                            }
                            else if (sourceLink.ToString().Contains("v.redd.it") && Result.Value<bool>("is_video"))
                            {
                                GetRedditVideo.GetVideoMp4(PathToResult, data, sourceLink, usableName);
                            }
                            else if (sourceLink.ToString().Contains("youtu.be")
                                 || sourceLink.ToString().Contains("youtube"))
                            {
                                if (!File.Exists(sourceLink + ".mp4"))
                                {
                                    Console.WriteLine("Detected YouTube Video/Clip! Attempting Download!");
                                    //GET video with yt-dlp
                                    YouTubeDLP YTDLP = new();

                                    YTDLP.VideoLink = sourceLink;

                                    YTDLP.GetVideoAsMP4(PathToResult.ToString());
                                }
                                else
                                {
                                    InternalProgramData.TimesRepeated++;
                                }
                            }

                            else if (!sourceLink.ToString().Contains("v.redd.it")
                                 || !sourceLink.ToString().Contains("youtu.be")
                                 && !Result.Value<bool>("is_video")
                                 )
                            {
                                //Normal Execution
                                if (sourceLink.ToString().Contains(".jpg"))
                                {
                                    PathToResult.Append(".jpg");
                                } 
                                else if (sourceLink.ToString().Contains(".png"))
                                {
                                    PathToResult.Append(".png");
                                } 
                                else if (sourceLink.ToString().Contains(".gif"))
                                {
                                    PathToResult.Append(".gif");
                                } 
                                else if (sourceLink.ToString().Contains(".jpeg")) 
                                {
                                    PathToResult.Append(".jpeg");
                                } 
                                else if (sourceLink.ToString().Contains(".mp4"))
                                {
                                    PathToResult.Append(".mp4");
                                } 
                                else
                                {
                                    PathToResult.Append(".htm");
                                }


                                if (!File.Exists(PathToResult.ToString()))
                                {
                                    try
                                    {
                                        using FileStream fs = File.Create(PathToResult.ToString());

                                        _hrm = _httpClient.GetAsync(sourceLink.ToString()).GetAwaiter().GetResult();

                                        if (_hrm.IsSuccessStatusCode)
                                            _hrm.Content.CopyToAsync(fs).GetAwaiter().GetResult();

                                        fs.Dispose();
                                        fs.Close();
                                        Console.WriteLine($"{Thread.CurrentThread.Name}; Downloaded {Result["title"]}");

                                    }
                                    catch
                                    {
                                        Console.WriteLine($"{Thread.CurrentThread.Name}; Post \"{usableName}\" is been already downloaded by another bot already!");
                                    }
                                }
                                else
                                {
                                    InternalProgramData.TimesRepeated++;
                                }
                            }
                        }
                        //Add a time out after downloading.
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));
                    }
                    else
                    {
                        LOGW($"{Thread.CurrentThread.Name} | is being rebooted");
                        throw new Exception("Bot Reboot.");
                    }

                }
                if (isBot0) {

                    BotInformation.aliveBots0--;
                } 
                else
                {
                    BotInformation.aliveBots1--;
                }
            }
            catch (Exception ex)
            {
                if (!InternalProgramData.STOPPROGRAM)
                {
                    if(!ex.Message.Contains("Reboot")) 
                    {
                        Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}. Bot Terminated with error: {ex.Message}, STACK TRACE: {ex.StackTrace}, INNER EXCEPTION: {ex.InnerException}");
                    }
                    else
                    {
                        Console.WriteLine($"Restarting {Thread.CurrentThread.Name}.");
                    }
                    CheckAndReviveBots();
                }
                else
                {
                    Console.WriteLine($"Bot {Thread.CurrentThread.Name} finished downloading last post.");
                }
            }
        }
        public static void CheckAndReviveBots()
        {
            BotHandler BotHandle = new();
            if (!InternalProgramData.STOPPROGRAM) {

                lock (botRespawnLocker)
                {
                    float totalBots = InternalProgramData.BotCount,
                          aliveBots0 = BotInformation.aliveBots0--,
                          aliveBots1 = BotInformation.aliveBots1;

                    float aliveBotsPercentage0 = aliveBots0 / totalBots * 100,
                          aliveBotsPercentage1 = aliveBots1 / totalBots * 100;

                    //Console.WriteLine($"{aliveBotsPercentage0}% of bots for Target0 are alive!");
                    //Console.WriteLine($"{aliveBotsPercentage1}% of bots for Target1 are alive!");


                    if (InternalProgramData.SimultaneousDownload) {
                        if (aliveBotsPercentage0 <= 50 && aliveBotsPercentage1 <= 50)
                        {
                            float amountToRevive0 = totalBots - aliveBots0;
                            for (float i = amountToRevive0; i < totalBots / 2 / 2; i++)
                            {
                                //Execute bots according to the ammount specified on BotCount 🥶👌
                                Thread x = new(() => BotHandle.StartBot(true, (int)i * 2));
                                x.Name = $"- | bot n{i}";
                                x.IsBackground = true;
                                x.Start();
                            }
                            float amountToRevive = totalBots - aliveBots1;
                            for (float i = amountToRevive; i < totalBots / 2 / 2; i++)
                            {
                                //Execute bots according to the ammount specified on BotCount 🥶👌
                                Thread x = new(() => BotHandle.StartBot(false, (int)i * 2));
                                x.Name = $"- | bot n{i}";
                                x.IsBackground = true;
                                x.Start();
                            }

                            //Console.WriteLine($"Revived bots, Total for Target0 {BotStatus.aliveBots0.Count}");
                            //Console.WriteLine($"Revived bots, Total for Target1 {BotStatus.aliveBots1.Count}");
                        }
                        else if (aliveBotsPercentage0 <= 50)
                        {
                            float amountToRevive = totalBots - aliveBots0;
                            for (float i = amountToRevive; i < totalBots / 2; i++)
                            {
                                //Execute bots according to the ammount specified on BotCount 🥶👌
                                Thread x = new(() => BotHandle.StartBot(true, (int)i * 2));
                                x.Name = $"- | bot n{i}";
                                x.IsBackground = true;
                                x.Start();
                            }
                            //Console.WriteLine($"Revived bots, Total for Target0 {BotStatus.aliveBots0.Count}");
                        }
                        else if (aliveBotsPercentage1 <= 50)
                        {
                            float amountToRevive = totalBots - aliveBots1;
                            for (float i = amountToRevive; i < totalBots / 2; i++)
                            {
                                //Execute bots according to the ammount specified on BotCount 🥶👌
                                Thread x = new(() => BotHandle.StartBot(false, (int)i * 2));
                                x.Name = $"- | bot n{i}";
                                x.IsBackground = true;
                                x.Start();
                            }
                            //Console.WriteLine($"Revived bots, Total for Target1 {BotStatus.aliveBots1.Count}");
                            //Console.WriteLine($"Current Total Bot Count: {BotStatus.aliveBots0.Count + BotStatus.aliveBots1.Count}");

                        } 
                    } 
                    else
                    {
                        float amountToRevive0 = totalBots - aliveBots0;
                        for (float i = amountToRevive0; i < totalBots; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandle.StartBot(true, (int)i * 2));
                            x.Name = $"- | bot n{i}";
                            x.IsBackground = true;
                            x.Start();
                        }
                    }
                }
            }
        }
    }
}
