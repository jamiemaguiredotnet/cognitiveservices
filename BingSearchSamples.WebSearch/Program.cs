using System;
using System.Net.Http;

namespace BingSearchSamples.WebSearch
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                HttpClient client = new HttpClient();
                string queryString = "Brexit";
                string azureKey = "4ddd6691d27f424ba5f1f3b819fbdd67";

                // ssetup request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureKey);

                var url = "https://api.cognitive.microsoft.com/bing/v7.0/search?q=" + queryString;

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
