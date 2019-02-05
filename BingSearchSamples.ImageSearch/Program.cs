using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BingSearchSamples.ImageSearch
{
    class Program
    {
        const string subscriptionKey = "your key";
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";
        const string searchTerm = "audi a3";

        

        static string BingImageSearch(string searchTerm)
        {
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchTerm);
            WebRequest request = WebRequest.Create(uriQuery);

            request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;

            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return json;
        }

        public static void Main(string[] args)
        {
            string response = BingImageSearch(searchTerm);
        }

    }
}
