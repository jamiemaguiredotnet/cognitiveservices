using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BingSearchSamples.WebSearch
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                var client = new HttpClient();
                var queryString = string.Empty;

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<your key here>");

                var url = "https://api.cognitive.microsoft.com/bing/v7.0/search?q=bing maps";

                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response?.Content != null)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(responseString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
