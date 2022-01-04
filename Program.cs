using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System;

namespace ShitpostingMachine
{
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
    public class Program
    {
        public static void Main()
        {

            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Shitposs");

            Console.WriteLine("Starting Hell!");

            for (int i = 0; i < 64; i++)
            {
                //Download posts on 64 threads 🥶👌
                Thread x = new(() => GetShitPost(i * 2));
                x.Name = "Shitpost bot n" + i;
                x.IsBackground = true;
                x.Start();
            }
            Console.ReadKey();
        }

        public static void GetShitPost(int timeOut = 50)
        {
            try
            {
                Random rand = new();
                
                while (true)
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
            }
            catch
            {
                Console.WriteLine($"Reddit rate limited {Thread.CurrentThread.Name}");
            }
        }
    }
}


