using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace BingSearchSamples.NewsSearch
{
    class Program
    {
        const string accessKey = "";
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/news/search";
        const string searchTerm = "weather";
        const int count = 3;

        static string BingNewsSearch(string toSearch)
        {
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(toSearch) + "&count=" + count.ToString();

            WebRequest request = HttpWebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;

            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return json;
        }

        static void Main(string[] args)
        {
            string searchResult = BingNewsSearch(searchTerm);

            Console.WriteLine(searchResult);
        }
    }
}
