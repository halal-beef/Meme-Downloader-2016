using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ShitpostingMachine {

    public class Program
    {
        public void Main(){

            Console.WriteLine("Making Dirs...");
            Directory.CreateDirectory("Shitposs");

            Console.WriteLine("Starting Hell!");

            Thread[] threadArray = new {};
        }

        public static void GetShitPost(){
            
            var Result = JObject.Parse(
                JArray.Parse(

                    new HttpClient()

                    .GetStringAsync("https://reddit.com/r/shitposting/random.json?limit=1")
                    
                    )[0]["data"]["children"][0]["data"]
                    .ToString().GetAwaiter().GetResult();
                
                );
            
            new WebClient().DownloadFile
            (
                Result["url"].ToString(), "poopout/" + Path.GetFileName(new Uri(Result["url"].ToString()).LocalPath)
            ); 
            
            Console.WriteLine("Downloaded " + Result["title"].ToString()); 
        }
    }
} 


