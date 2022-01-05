namespace Dottik.MemeDownloader.Bots
{
    internal class BotHandler
    {
        public static Object locked = new();
        public static Object botRespawnLocker = new();
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
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
                    if (!OptimizeMemory.collectionOnProgress && !InternalProgramData.STOPPROGRAM)
                    {
                        Thread.Sleep(timeOut + rand.Next(0, timeOut));
                        //Setup the request to use TLS 1.2
                        string data = "";
                        try
                        {
                            data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit}/random.json?limit=1").GetAwaiter().GetResult();
                        }
                        finally
                        {
                            data = new HttpClient(InternalProgramData.handler).GetStringAsync($"http://reddit.com/r/{InternalProgramData.TargetSubReddit}/random.json?limit=1").GetAwaiter().GetResult();
                        }


                        var Result = JObject.Parse(

                            JArray.Parse(data)[0]["data"]["children"][0]["data"].ToString()

                            );

                        string usableName = Result["url"].ToString().Replace('/', '_').Replace(':', '.').Replace('?', '[');
                        string PathToResult = Environment.CurrentDirectory + @"/Shitposs/" + usableName;

                        //Get Video if content is of that type.

                        string sourceLink = Result.Value<string>("url_overridden_by_dest");

                        //if (sourceLink == null)
                        //{

                        //}

                        if (sourceLink != null && sourceLink.Contains("v.redd.it"))
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
                                    Thread.Sleep(5);
                                }

                                //Delete temp files!
                                File.Delete(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4");
                                File.Delete(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");
                            }
                            else
                            {
                                Console.WriteLine("Video already exists! Restarting Bot");
                                throw new Exception("Restart Bot");
                            }
                        }
                        else if (sourceLink == null || !sourceLink.Contains("v.redd.it"))
                        {
                            //Normal Execution
                            if (!PathToResult.Contains(".jpg") && !PathToResult.Contains(".png") && !PathToResult.Contains(".gif") && !PathToResult.Contains(".jpeg") && !PathToResult.Contains(".mp4"))
                            {
                                PathToResult += ".htm";
                            }


                            if (!File.Exists(PathToResult))
                            {
                                using (FileStream fs = File.Create(PathToResult))
                                {
                                    HttpClient httpClient = new(InternalProgramData.handler);

                                    HttpResponseMessage hrm = httpClient.GetAsync(Result["url"].ToString()).GetAwaiter().GetResult();

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
                                Console.WriteLine($"{Thread.CurrentThread.Name}; Downloaded {Result["title"]}");
                            }
                            else
                            {
                                Console.WriteLine($"{Thread.CurrentThread.Name}; Post \"{usableName}\" Already Downloaded!!!!");
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
                BotStatus.aliveBots.RemoveAt(0);
            }
            catch (Exception ex)
            {
                if (!InternalProgramData.STOPPROGRAM)
                {
                    BotStatus.aliveBots.RemoveAt(0);
                    Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}. Bot Terminated with error: {ex.Message} and STACK TRACE: {ex.StackTrace}, INNER EXCEPTION: {ex.InnerException}, Bots Left: {BotStatus.aliveBots.Count}");
                    CheckAndReviveBots();
                }
                else
                {
                    Console.WriteLine($"Bot {Thread.CurrentThread.Name} finished downloading last post.");
                }
            }
        }
        public void CheckAndReviveBots()
        {
            if (!InternalProgramData.STOPPROGRAM) {

                lock (botRespawnLocker)
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
                            x.Name = $"{InternalProgramData.TargetSubReddit} bot n" + i;
                            x.IsBackground = true;
                            x.Start();
                        }
                        Console.WriteLine($"Created {deadBots} new bots, Total Remaining: {BotStatus.aliveBots.Count}");
                    }
                } 
            }
        }
    }
}
