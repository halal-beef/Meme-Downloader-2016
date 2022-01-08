namespace Dottik.MemeDownloader.Bots
{
    internal class BotHandler
    {
        public static Object locked = new();
        public static Object botRespawnLocker = new();
        public static Object locked0 = new();

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        public static void StartBot(bool modeA, int timeOut = 50)
        {
            LOGD($"Started new bot with name: {Thread.CurrentThread.Name}.");
            bool isBot0 = false;
            try
            {
                if (modeA) 
                {
                    isBot0 = true;
                    lock (locked)
                    {
                        BotInformation.aliveBots0++;
                    }
                } 
                else
                {
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
                                    data.Append(new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit0}/random.json").GetAwaiter().GetResult());
                                    requestSuccess = true;
                                    target.Append(InternalProgramData.TargetSubReddit0);
                                } else
                                {
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
                            usableName.Append(Result["url_overridden_by_dest"].ToString().Replace('/', '_').Replace(':', '.').Replace('?', '['));
                            sourceLink.Append(Result.Value<string>("url_overridden_by_dest"));
                        }
                        catch
                        {
                            usableName.Append(Result["url"].ToString().Replace('/', '_').Replace(':', '.').Replace('?', '['));
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

                        if (!blacklisted)
                        {
                            if (sourceLink != null && sourceLink.ToString().Contains("v.redd.it") && Result.Value<bool>("is_video"))
                            {
                                GetRedditVideo.GetVideoMp4(PathToResult, data, sourceLink, usableName);
                            }
                            else if ( ( sourceLink != null && sourceLink.ToString().Contains("youtu.be") ) || sourceLink.ToString().Contains("youtube"))
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
                            else if (sourceLink.ToString() == null
                                 || !sourceLink.ToString().Contains("v.redd.it")
                                 || !sourceLink.ToString().Contains("youtu.be")
                                 && !Result.Value<bool>("is_video")
                                 )
                            {
                                //Normal Execution
                                if (!PathToResult.ToString().Contains(".jpg") && !PathToResult.ToString().Contains(".png") && !PathToResult.ToString().Contains(".gif") && !PathToResult.ToString().Contains(".jpeg") && !PathToResult.ToString().Contains(".mp4"))
                                {
                                    PathToResult.Append(".htm");
                                }


                                if (!File.Exists(PathToResult.ToString()))
                                {
                                    try
                                    {
                                        using FileStream fs = File.Create(PathToResult.ToString());
                                        HttpClient httpClient = new(InternalProgramData.handler);
                                        HttpResponseMessage hrm;

                                        hrm = httpClient.GetAsync(sourceLink.ToString()).GetAwaiter().GetResult();

                                        try
                                        {
                                            hrm.EnsureSuccessStatusCode();
                                        }
                                        catch
                                        {
                                            Console.WriteLine("YOU DON'T HAVE INTERNET YOU FOOL!");
                                            throw new Exception("Did you unplig the WiFi?");
                                        }

                                        hrm.Content.CopyToAsync(fs).GetAwaiter().GetResult();
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
                                Thread x = new(() => BotHandler.StartBot(true, (int)i * 2));
                                x.Name = $"- bot n{i}";
                                x.IsBackground = true;
                                x.Start();
                            }
                            float amountToRevive = totalBots - aliveBots1;
                            for (float i = amountToRevive; i < totalBots / 2 / 2; i++)
                            {
                                //Execute bots according to the ammount specified on BotCount 🥶👌
                                Thread x = new(() => BotHandler.StartBot(false, (int)i * 2));
                                x.Name = $"- bot n{i}";
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
                                Thread x = new(() => BotHandler.StartBot(true, (int)i * 2));
                                x.Name = $"- bot n{i}";
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
                                Thread x = new(() => BotHandler.StartBot(false, (int)i * 2));
                                x.Name = $"- bot n{i}";
                                x.IsBackground = true;
                                x.Start();
                            }
                            //Console.WriteLine($"Revived bots, Total for Target1 {BotStatus.aliveBots1.Count}");
                            //Console.WriteLine($"Current Total Bot Count: {BotStatus.aliveBots0.Count + BotStatus.aliveBots1.Count}");

                        } 
                    } else
                    {
                        float amountToRevive0 = totalBots - aliveBots0;
                        for (float i = amountToRevive0; i < totalBots; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandler.StartBot(true, (int)i * 2));
                            x.Name = $"- bot n{i}";
                            x.IsBackground = true;
                            x.Start();
                        }
                    }
                }
            }
        }
    }
}
