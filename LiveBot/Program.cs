using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using RestSharp;
namespace LiveBot
{
    class Program
    {
        public static Newtonsoft.Json.Linq.JObject jsonParse;
        public static RestClient rClient = new RestClient("https://graph.facebook.com/v2.8");
        public static Facebook.LiveStreamResult result;
        static void Main(string[] args)
        {
            Console.WriteLine("LiveBot for C#\nYour Facebook Livestream Utilites");
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("[ERROR] config.json File Not Found!");
                Console.ReadKey();
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("[INFO] config.json Found!");
            }
            StreamReader sr = new StreamReader("config.json");
            jsonParse = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(sr.ReadToEnd());
            Console.WriteLine("Requesting New Stream Post...");
            RestRequest rNewStream = new RestRequest("me/live_videos", Method.POST);
            rNewStream.AddParameter("access_token", jsonParse.GetValue("token").ToString());
            rNewStream.AddParameter("title", jsonParse.GetValue("title").ToString());
            rNewStream.AddParameter("description", jsonParse.GetValue("description").ToString());
            rNewStream.AddParameter("save_vod", jsonParse.GetValue("save_vod").ToString());
            rNewStream.AddParameter("status", jsonParse.GetValue("status").ToString());
            result = JsonConvert.DeserializeObject<Facebook.LiveStreamResult>(rClient.Execute(rNewStream).Content);
            Console.WriteLine("[INFO] Stream Request Success!\nStream URL : rtmp://rtmp-api.facebook.com:80/rtmp/");
            Console.WriteLine("Stream Key : " + result.stream_url.Replace("rtmp://rtmp-api.facebook.com:80/rtmp/", ""));
            Console.ReadKey();
            //Console.WriteLine("[WARNING] Stream Request Canceled.");
            WebServer ws = new WebServer(getComment, "http://127.0.0.1:8080/getComment/");
            ws.Run();
            Console.WriteLine("Type :quit to Exit!");
            bool isExit = false;
            while (isExit == false) {
                string arg = Console.ReadLine();
                if(arg == "quit")
                {
                    ws.Stop();
                    Environment.Exit(0);
                }
            }
        }


        public static string getComment(HttpListenerRequest request)
        {
            String url = result.id + "/comments";
            bool isFinished = false;
            String outputString = "";
            while (isFinished == false)
            {
                //Console.WriteLine(url);
                

                RestRequest rComments = new RestRequest(url);
                rComments.AddParameter("access_token", jsonParse.GetValue("token").ToString());

                Facebook.Comment facebookComment = JsonConvert.DeserializeObject<Facebook.Comment>(rClient.Execute(rComments).Content);
                foreach (Facebook.commentClass comment in facebookComment.data)
                {
                    outputString += String.Format("<b>{0}</b> : {1}<br>\n", comment.from.name, comment.message);
                }

                if (facebookComment.paging.next != null) {
                    url = facebookComment.paging.next.Replace("https://graph.facebook.com/v2.8/", "");
                }
                else
                {
                    isFinished = true;
                }
            }
            return outputString;
            


        }
    }

    public class Facebook
    {
        public class LiveStreamResult
        {
            public string id { get; set; }
            public string stream_url { get; set; }
            public string secure_stream_url { get; set; }
        }

        public class commentClass {
            public string created_time { get; set; }
            public class fromClass {
                public string name { get; set; }
                public string id { get; set; }
            }
            public fromClass from { get; set; }
            public string message { get; set; }
            public string id { get; set; }
        }

        public class pagingClass {
            public class cursorClass {
                public string before { get; set; }
                public string after { get; set; }
            }
            public cursorClass cursor { get; set; }
            public string next { get; set; }
        }

        public class Comment {
            public commentClass[] data { get; set; }
            public pagingClass paging { get; set; }
        }
    }
}

