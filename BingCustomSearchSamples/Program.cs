﻿using System;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using BingSearchSamples.CustomSearch.Models;

namespace BingSearchSamples.CustomSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            //var subscriptionKey = "YOUR-SUBSCRIPTION-KEY";
            //var customConfigId = "YOUR-CUSTOM-CONFIG-ID";

            string subscriptionKey = "ebc25e481b50416bac331e703ac7f1e1";
            string customConfigId = "f7e6d23c-b3ed-4e4a-9bd2-7099e90d2140";

            var searchTerm = "Twitter";

            var url = "https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search?" + "q=" + searchTerm + "&customconfig=" + customConfigId;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            var httpResponseMessage = client.GetAsync(url).Result;
            var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
            BingCustomSearchResponse response = JsonConvert.DeserializeObject<BingCustomSearchResponse>(responseContent);

            for (int i = 0; i < response.webPages.value.Length; i++)
            {
                var webPage = response.webPages.value[i];

                Console.WriteLine("name: " + webPage.name);
                Console.WriteLine("url: " + webPage.url);
                Console.WriteLine("displayUrl: " + webPage.displayUrl);
                Console.WriteLine("snippet: " + webPage.snippet);
                Console.WriteLine("dateLastCrawled: " + webPage.dateLastCrawled);
                Console.WriteLine();
                
            }
        }
    }
}

