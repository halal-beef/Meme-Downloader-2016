namespace Dottik.MemeDownloader.Bots
{
    internal class BotHandler
    {
        private static bool tests = true;
        public static Object locked = new();
        public static Object botRespawnLocker = new();
        public static Object locked0 = new();
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        public static void StartBot(string Target, int timeOut = 50)
        {
            bool isBot0 = false;
            try
            {
                if (Target == InternalProgramData.TargetSubReddit0) {
                    isBot0 = true;
                    lock (locked)
                    {
                        BotStatus.aliveBots0.Add(true);
                    }
                } else if (Target == InternalProgramData.TargetSubReddit1) 
                {
                    lock (locked0)
                    {
                        BotStatus.aliveBots1.Add(true);
                    } 
                }
                Random rand = new();

                while (!InternalProgramData.STOPPROGRAM)
                {
                    if (!OptimizeMemory.collectionOnProgress && !InternalProgramData.STOPPROGRAM)
                    {
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));

                        string data = "";
                        try
                        {
                            data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{Target}/random.json").GetAwaiter().GetResult();
                        }
                        catch
                        {
                            data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{Target}/random.json").GetAwaiter().GetResult();
                        }


                        var Result = JObject.Parse(

                            JArray.Parse(data)[0]["data"]["children"][0]["data"].ToString()

                            );
                        string usableName = "", sourceLink = null;
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
                            if (!File.Exists(PathToResult + ".mkv"))
                            {
                                var result0 = JObject.Parse(

                                JArray.Parse(data)[0]["data"]["children"][0]["data"]["secure_media"]["reddit_video"].ToString()

                                );
                                string VideoLink = "";
                                try
                                {
                                    VideoLink = result0.Value<string>("fallback_url");
                                }
                                catch
                                {

                                }

                                string AudioLink = sourceLink + "/DASH_audio.mp4";

                                Console.WriteLine($"AUDIOLINK: {AudioLink}");
                                Console.WriteLine($"VIDEOLINK: {VideoLink}");
                                HttpClient client = new(InternalProgramData.handler);

                                HttpResponseMessage hrm0 = client.GetAsync(AudioLink).GetAwaiter().GetResult();
                                HttpResponseMessage hrm1 = client.GetAsync(VideoLink).GetAwaiter().GetResult();

                                try
                                {
                                    hrm0.EnsureSuccessStatusCode();

                                    hrm1.EnsureSuccessStatusCode();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"{Thread.CurrentThread.Name}; Detected reddit video, but failed to get it. {ex.Message}");
                                }

                                using (FileStream fs = File.Create(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4"))
                                {
                                    hrm0.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                                }
                                using (FileStream fs = File.Create(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4"))
                                {
                                    hrm1.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                                }

                                Thread encoder = new(
                                    () =>
                                MergeAudioAndVideo.UseFFMPEG(
                                    Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4",
                                    Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4", PathToResult));
                                encoder.IsBackground = true;
                                encoder.Start();

                                while (encoder.IsAlive)
                                {
                                    Thread.Sleep(10000);
                                }

                                //Delete temp files!
                                try
                                {
                                    File.Delete(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4");
                                    File.Delete(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");
                                }
                                catch
                                {
                                    //Sleep for 20 seconds and try again
                                    Thread.Sleep(20000);
                                }
                                finally
                                {
                                    File.Delete(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4");
                                    File.Delete(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Video already exists! Restarting Bot");
                                throw new Exception("Restart Bot");
                            }
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
                                Console.WriteLine("Video already exists! Restarting Bot");
                                throw new Exception("Restart Bot");
                            }
                        }
                        else if (sourceLink == null || !sourceLink.Contains("v.redd.it") || sourceLink.Contains("youtu.be") && !Result.Value<bool>("is_video"))
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

                                        try
                                        {
                                            hrm = httpClient.GetAsync(Result["url_overriden_by_dest"].ToString()).GetAwaiter().GetResult();
                                        }
                                        catch
                                        {
                                            hrm = httpClient.GetAsync(Result["url"].ToString()).GetAwaiter().GetResult();
                                        }

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
                                    
                                }
                                catch
                                {
                                    Console.WriteLine($"{Thread.CurrentThread.Name}; Post \"{usableName}\" is been already downloaded by another bot already!");
                                }
                                Console.WriteLine($"{Thread.CurrentThread.Name}; Downloaded {Result["title"]}");
                            }
                            else
                            {
                                Console.WriteLine($"{Thread.CurrentThread.Name}; Post \"{usableName}\" Already Downloaded!");
                            }
                        }
                        //Add a time out after downloading.
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));
                    }
                    else
                    {
                        Thread.Sleep(200);
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
                        Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}. Bot Terminated with error: {ex.Message} and STACK TRACE: {ex.StackTrace}, INNER EXCEPTION: {ex.InnerException}, bots {Target} Left: {BotStatus.aliveBots0.Count}");
                    }
                    else if(!ex.Message.Contains("Restart") && !isBot0)
                    {
                        Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}. Bot Terminated with error: {ex.Message} and STACK TRACE: {ex.StackTrace}, INNER EXCEPTION: {ex.InnerException}, bots {Target} Left: {BotStatus.aliveBots1.Count}");
                    }
                    else
                    {
                        Console.WriteLine($"Restarting {Thread.CurrentThread.Name}");
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

                    Console.WriteLine($"{aliveBotsPercentage0}% of bots for Target0 are alive!");
                    Console.WriteLine($"{aliveBotsPercentage1}% of bots for Target1 are alive!");


                    if(aliveBotsPercentage0 <= 50 && aliveBotsPercentage1 <= 50)
                    {
                        float amountToRevive0 = totalBots - aliveBots0;
                        for (float i = amountToRevive0; i < totalBots / 2 / 2; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandler.StartBot(InternalProgramData.TargetSubReddit0, (int)i * 2));
                            x.Name = $"{InternalProgramData.TargetSubReddit0} bot n" + i;
                            x.IsBackground = true;
                            x.Start();
                        }
                        float amountToRevive = totalBots - aliveBots1;
                        for (float i = amountToRevive; i < totalBots / 2; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandler.StartBot(InternalProgramData.TargetSubReddit1, (int)i * 2));
                            x.Name = $"{InternalProgramData.TargetSubReddit1} bot n" + i;
                            x.IsBackground = true;
                            x.Start();
                        }

                        Console.WriteLine($"Revived bots, Total for Target0 {BotStatus.aliveBots0.Count}");
                        Console.WriteLine($"Revived bots, Total for Target1 {BotStatus.aliveBots1.Count}");
                    }
                    else if (aliveBotsPercentage0 <= 50)
                    {
                        float amountToRevive = totalBots - aliveBots0;
                        for (float i = amountToRevive; i < totalBots / 2; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandler.StartBot(InternalProgramData.TargetSubReddit0, (int)i * 2));
                            x.Name = $"{InternalProgramData.TargetSubReddit0} bot n" + i;
                            x.IsBackground = true;
                            x.Start();
                        }
                        Console.WriteLine($"Revived bots, Total for Target0 {BotStatus.aliveBots0.Count}");
                    }
                    else if (aliveBotsPercentage1 <= 50)
                    {
                        float amountToRevive = totalBots - aliveBots1;
                        for (float i = amountToRevive; i < totalBots / 2; i++)
                        {
                            //Execute bots according to the ammount specified on BotCount 🥶👌
                            Thread x = new(() => BotHandler.StartBot(InternalProgramData.TargetSubReddit1, (int)i * 2));
                            x.Name = $"{InternalProgramData.TargetSubReddit1} bot n" + i;
                            x.IsBackground = true;
                            x.Start();
                        }
                        Console.WriteLine($"Revived bots, Total for Target1 {BotStatus.aliveBots1.Count}");
                        Console.WriteLine($"Current Total Bot Count: {BotStatus.aliveBots0.Count + BotStatus.aliveBots1.Count}");

                    }
                }
            }
        }
    }
}
