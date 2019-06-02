using System;
using System.IO;
using System.Net;

namespace BingSearchSamples.VideoSearch
{
    class Program
    {
        const string accessKey = "4ddd6691d27f424ba5f1f3b819fbdd67";
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/videos/search";
        const string searchTerm = "cats";
        const int count = 1;

        static string BingVideoSearch(string toSearch)
        {
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(toSearch) + "&count=" + count.ToString();

            WebRequest request = HttpWebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;

            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return json;
        }

        public static void Main(string[] args)
        {
            string searchResult = BingVideoSearch(searchTerm);

            Console.WriteLine(searchResult);
        }
    }
}
