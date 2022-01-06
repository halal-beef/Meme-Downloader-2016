namespace Dottik.MemeDownloader.GetMedia
{
    internal class GetRedditVideo
    {
        public static Object audioMediaLockera = new();
        public static Object videoMediaLockera = new(); 
        public static Object audioMediaLockerb = new();
        public static Object videoMediaLockerb = new();
        public static Object mediaLockera = new();
        public static Object mediaLockerb = new();

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        public static void GetVideoMp4(string PathToResult, string data, string sourceLink, string usableName)
        {
            Random rand = new();

            Thread.Sleep(rand.Next(0, 1500));

            if (!File.Exists(PathToResult + ".mp4"))
            {
                bool audioAvailable = true, 
                     videoAvailable = true;
                var result0 = JObject.Parse(

                JArray.Parse(data)[0]["data"]["children"][0]["data"]["secure_media"]["reddit_video"].ToString()

                );
                string VideoLink = "";

                VideoLink = result0.Value<string>("fallback_url");

                string AudioLink = sourceLink + "/DASH_audio.mp4";

                Console.WriteLine($"{Thread.CurrentThread.Name}; AUDIOLINK: {AudioLink}");
                Console.WriteLine($"{Thread.CurrentThread.Name}; VIDEOLINK: {VideoLink}");
                HttpClient client = new(InternalProgramData.handler);

                HttpResponseMessage hrm0 = client.GetAsync(AudioLink).GetAwaiter().GetResult();
                HttpResponseMessage hrm1 = client.GetAsync(VideoLink).GetAwaiter().GetResult();

                try
                {
                    hrm0.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name}; Detected reddit audio for video, but failed to get audio. {ex.Message}");
                    audioAvailable = false;
                }
                try
                {
                    hrm1.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name}; Detected reddit video for video, but failed to get video. {ex.Message}");
                    videoAvailable = false;
                }

                if(!audioAvailable && !videoAvailable)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name}; Video and Audio aren't available.");
                }

                if (audioAvailable && videoAvailable && !File.Exists(PathToResult + ".mp4"))
                {
                    lock (mediaLockera)
                    {
                        using FileStream fs0 = File.Create(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4");

                            hrm1.Content.CopyToAsync(fs0).GetAwaiter().GetResult();
                            fs0.Dispose();
                            fs0.Close();

                        using FileStream fs = File.Create(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");

                            hrm0.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                            fs.Dispose();
                            fs.Close();

                        lock (mediaLockerb)
                        {
                            Thread encoder = new(
                                () =>
                                MergeAudioAndVideo.UseFFMPEG(
                                Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4",
                                Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4", PathToResult + ".mp4", sourceLink));
                            encoder.IsBackground = true;
                            encoder.Start();

                            while (encoder.IsAlive)
                            {
                                Thread.Sleep(50);
                            }
                            //Delete temp files!
                            try
                            {
                                File.Delete(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4");
                                File.Delete(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");
                            }
                            catch
                            {
                                //Sleep for 10 seconds and try again
                                Thread.Sleep(10000);
                                File.Delete(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4");
                                File.Delete(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");
                            }
                        }
                    }
                }
                else if (audioAvailable && !File.Exists(PathToResult + "AUDIO_ONLY.mp3"))
                {
                    lock (audioMediaLockera)
                    {
                        using FileStream fs = File.Create(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");

                            hrm0.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                            fs.Dispose();
                            fs.Close();

                        lock (audioMediaLockerb)
                        {
                            PathToResult += "AUDIO_ONLY";
                            Thread audioEncoder = new(
                                () =>
                                MergeAudioAndVideo.UseFFMPEG(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4", PathToResult + ".mp3", sourceLink));
                            audioEncoder.IsBackground = true;
                            audioEncoder.Start();

                            while (audioEncoder.IsAlive)
                            {
                                Thread.Sleep(50);
                            }
                            File.Delete(Environment.CurrentDirectory + "/TEMP/" + "AUDIOPART." + usableName + @".mp4");
                        }
                    }
                }
                else if (videoAvailable && !File.Exists(PathToResult + "VIDEO_ONLY.mp4"))
                {
                    lock (videoMediaLockera)
                    {
                        using FileStream fs = File.Create(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4");

                            hrm1.Content.CopyToAsync(fs).GetAwaiter().GetResult();
                            fs.Dispose();
                            fs.Close();

                        Console.WriteLine(PathToResult);
                        PathToResult += "VIDEO_ONLY.mp4";

                        lock (videoMediaLockerb)
                        {
                            File.Move(Environment.CurrentDirectory + "/TEMP/" + "VIDEOPART." + usableName + @".mp4", PathToResult, true);
                        }
                    }
                } else
                {
                    ProcessOptimizer.AddToBlacklist(sourceLink);
                }
            }
            else
            {
                InternalProgramData.TimesRepeated++;
            }
        }
    }
}
