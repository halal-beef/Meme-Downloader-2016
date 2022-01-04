namespace Dottik.MemeDownloader.Bots
{
    internal class BotHandler
    {
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        public void GetShitPost(int timeOut = 50)
        {
            try
            {
                Random rand = new();

                while (true)
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
            }
            catch
            {
                Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}");
            }
        }
    }
}
