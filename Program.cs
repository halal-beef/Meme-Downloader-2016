using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System;
using System.Diagnostics;

namespace ShitpostingMachine
{
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
    public class Program
    {
        public static bool collectionOnProgress = false;
        public static void Main()
        {
            Program p = new();
            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Shitposs");

            Console.WriteLine("Starting Hell!");

            for (int i = 0; i < 64; i++)
            {
                //Download posts on 64 threads 🥶👌
                Thread x = new(() => p.GetShitPost(i * 2));
                x.Name = "Shitpost bot n" + i;
                x.IsBackground = true;
                x.Start();
            }
            new Thread(() => p.CallGC()).Start();
            Console.ReadKey();
        }

        private void CallGC()
        {
            while (true)
            {
                Process pro = Process.GetCurrentProcess();
                bool shouldCollect = false;

                //Calls GC when memory is more than 512MB
                if (pro.WorkingSet64 >= 512000000)
                {
                    shouldCollect = true;
                }

                if (shouldCollect) 
                {
                    //Announce collection!
                    collectionOnProgress = true;

                    Console.WriteLine("Garbage Collection Working. Please Hold!");
                    Thread.Sleep(5000);
                    
                    GC.Collect();
                    shouldCollect = false;
                    collectionOnProgress = false;
                }
            }

        }

        private void GetShitPost(int timeOut = 50)
        {
            try
            {
                Random rand = new();
                
                while (true)
                {
                    if (!collectionOnProgress)
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
                Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name} {Process.GetCurrentProcess().WorkingSet64}");
            }
        }
    }
}


