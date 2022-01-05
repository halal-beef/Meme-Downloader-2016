using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ShitpostingMachine {

    #pragma warning disable CS8602 // Dereference of a possibly null reference.
    #pragma warning disable SYSLIB0014 // Type or member is obsolete

    public class Program
    {
        public static void Main0(){

            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Shitposs");

            Console.WriteLine("Starting Hell!");

            //Fixed small error
            for (int i = 0; i < 8; i++)
            {
                Thread x = new(() => GetShitPost());
                x.IsBackground = true;
                x.Start();
            }
            Console.ReadKey();
        }

        public static void GetShitPost(){
            
            var Result = JObject.Parse(
                JArray.Parse(

                    new HttpClient()

                    .GetStringAsync("https://reddit.com/r/shitposting/random.json?limit=1").GetAwaiter().GetResult()

                    )[0]["data"]["children"][0]["data"]
                    .ToString()
                
                );

            new WebClient().DownloadFile
            (
                Result["url"].ToString(), "poopout/" + Path.GetFileName(new Uri(Result["url"].ToString()).LocalPath)
            ); 
            
            Console.WriteLine("Downloaded " + Result["title"].ToString()); 
        }
    }
} 


