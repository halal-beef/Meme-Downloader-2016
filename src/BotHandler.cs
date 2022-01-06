namespace Dottik.MemeDownloader.Bots
{
    internal class BotHandler
    {
        public static Object locked = new();
        public static Object botRespawnLocker = new();
        public static Object locked0 = new();

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        public static void StartBot(bool modeA, int timeOut = 50)
        {
            bool isBot0 = false;
            try
            {
                if (modeA) {
                    isBot0 = true;
                    lock (locked)
                    {
                        BotStatus.aliveBots0.Add(true);
                    }
                } 
                else if (!modeA) 
                {
                    lock (locked0)
                    {
                        BotStatus.aliveBots1.Add(true);
                    } 
                }
                Random rand = new();

                while (!InternalProgramData.STOPPROGRAM)
                {
                    if (!OptimizeMemory.collectionOnProgress && !InternalProgramData.STOPPROGRAM || InternalProgramData.RestartBot)
                    {
                        //Thread.CurrentThread.Name

                        //Change Thread Name!

                        string[] splitedThreadName = Thread.CurrentThread.Name.Split('|');

                        if (modeA) {
                            Thread.CurrentThread.Name = InternalProgramData.TargetSubReddit0 + " " + splitedThreadName[1];
                        }
                        else
                        {
                            Thread.CurrentThread.Name = InternalProgramData.TargetSubReddit1 + " " + splitedThreadName[1];
                        }
                        
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));
                        bool requestSuccess = false;
                        string data = "";

                        while (!requestSuccess)
                        {
                            try
                            {
                                if (modeA)
                                {
                                    data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit0}/random.json").GetAwaiter().GetResult();
                                    requestSuccess = true;
                                } else
                                {
                                    data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit1}/random.json").GetAwaiter().GetResult();
                                    requestSuccess = true;
                                }
                            }
                            catch
                            {
                                if (modeA)
                                {
                                    data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit0}/random.json").GetAwaiter().GetResult();
                                    requestSuccess = true;
                                }
                                else
                                {
                                    data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit1}/random.json").GetAwaiter().GetResult();
                                    requestSuccess = true;
                                }
                            }
                        }

                        var Result = JObject.Parse(

                            JArray.Parse(data)[0]["data"]["children"][0]["data"].ToString()

                            );
                        string usableName = "", sourceLink = "";

                        try
                        {
                            usableName = Result["url_overridden_by_dest"].ToString().Replace('/', '_').Replace(':', '.').Replace('?', '[');
                            sourceLink = Result.Value<string>("url_overridden_by_dest");
                        }
                        catch
                        {
                            usableName = Result["url"].ToString().Replace('/', '_').Replace(':', '.').Replace('?', '[');
                            sourceLink = Result["url"].ToString();
                        }
                        string PathToResult = Environment.CurrentDirectory + @"/Shitposs/" + usableName;

                        if (sourceLink != null && sourceLink.Contains("v.redd.it") && Result.Value<bool>("is_video"))
                        {
                            GetRedditVideo.GetVideoMp4(PathToResult, data, sourceLink, usableName);
                        }
                        else if (sourceLink != null && sourceLink.Contains("youtu.be") || sourceLink.Contains("youtube"))
                        {
                            if (!File.Exists(sourceLink + ".mp4"))
                            {
                                Console.WriteLine("Detected YouTube Video/Clip! Attempting Download!");
                                //GET video with yt-dlp
                                YouTubeDLP YTDLP = new();

                                YTDLP.VideoLink = sourceLink;

                                YTDLP.GetVideoAsMP4(PathToResult);
                            }
                            else
                            {
                                InternalProgramData.TimesRepeated++;
                            }
                        }
                        else if (sourceLink == null || !sourceLink.Contains("v.redd.it") || !sourceLink.Contains("youtu.be") && !Result.Value<bool>("is_video"))
                        {
                            //Normal Execution
                            if (!PathToResult.Contains(".jpg") && !PathToResult.Contains(".png") && !PathToResult.Contains(".gif") && !PathToResult.Contains(".jpeg") && !PathToResult.Contains(".mp4"))
                            {
                                PathToResult += ".htm";
                            }


                            if (!File.Exists(PathToResult))
                            {
                                try
                                {
                                    using FileStream fs = File.Create(PathToResult);
                                        HttpClient httpClient = new(InternalProgramData.handler);
                                        HttpResponseMessage hrm;

                                        hrm = httpClient.GetAsync(sourceLink).GetAwaiter().GetResult();

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
                        //Add a time out after downloading.
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));
                    }
                    else
                    {
                        throw new Exception("Bot Reboot.");
                    }

                }
                if (isBot0) {
                    BotStatus.aliveBots0.RemoveAt(0);
                } else
                {
                    BotStatus.aliveBots1.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                if (!InternalProgramData.STOPPROGRAM)
                {
                    if(!ex.Message.Contains("Restart") && isBot0) 
                    {
                        Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}. Bot Terminated with error: {ex.Message}, INNER EXCEPTION: {ex.InnerException}");
                    }
                    else if(!ex.Message.Contains("Restart") && !isBot0)
                    {
                        Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}. Bot Terminated with error: {ex.Message}, INNER EXCEPTION: {ex.InnerException}");
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
                          aliveBots0 = BotStatus.aliveBots0.Count,
                          aliveBots1 = BotStatus.aliveBots1.Count;

                    float aliveBotsPercentage0 = aliveBots0 / totalBots * 100,
                          aliveBotsPercentage1 = aliveBots1 / totalBots * 100;

                    //Console.WriteLine($"{aliveBotsPercentage0}% of bots for Target0 are alive!");
                    //Console.WriteLine($"{aliveBotsPercentage1}% of bots for Target1 are alive!");


                    if(aliveBotsPercentage0 <= 50 && aliveBotsPercentage1 <= 50)
                    {
                        float amountToRevive0 = totalBots - aliveBots0;
                        for (float i = amountToRevive0; i < totalBots / 2 / 2; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandler.StartBot(true, (int)i * 2));
                            x.Name = $"{InternalProgramData.TargetSubReddit0} bot n" + i;
                            x.IsBackground = true;
                            x.Start();
                        }
                        float amountToRevive = totalBots - aliveBots1;
                        for (float i = amountToRevive; i < totalBots / 2; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandler.StartBot(false, (int)i * 2));
                            x.Name = $"{InternalProgramData.TargetSubReddit1} bot n" + i;
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
                            x.Name = $"{InternalProgramData.TargetSubReddit0} bot n" + i;
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
                            x.Name = $"{InternalProgramData.TargetSubReddit1} bot n" + i;
                            x.IsBackground = true;
                            x.Start();
                        }
                        //Console.WriteLine($"Revived bots, Total for Target1 {BotStatus.aliveBots1.Count}");
                        //Console.WriteLine($"Current Total Bot Count: {BotStatus.aliveBots0.Count + BotStatus.aliveBots1.Count}");

                    }
                }
            }
        }
    }
}
