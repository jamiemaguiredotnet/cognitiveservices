using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Models;
using System.Configuration;
using Tweetinvi;
using Tweetinvi.Parameters;
using CognitiveSamples.LUIS;
using Newtonsoft.Json;

namespace CognitiveSamples
{
    class Program
    {

        private static string _ConsumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
        private static string _ConsumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
        private static string _AccessToken = ConfigurationManager.AppSettings.Get("AccessToken");
        private static string _AccessTokenSecret = ConfigurationManager.AppSettings.Get("AccessTokenSecret");

        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", "3f9cb70d2fb94d90b793cb77ece1d794");
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        private static void ProcessKeyPhrases(string tweetid, string text)
        {
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = AzureRegions.Westeurope;

            KeyPhraseBatchResult result2 = client.KeyPhrasesAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("en", tweetid + ":" + text, text)
                        })).Result;

            foreach (var document in result2.Documents)
            {
                Console.WriteLine("Document ID: {0} ", document.Id);

                Console.WriteLine("\t Key phrases:");

                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine("\t\t" + keyphrase);
                }
            }
        }


        private static void ProcessSentiment(string tweetid,string text)
        {
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = AzureRegions.Westeurope;

            SentimentBatchResult results = client.SentimentAsync(
                    new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {new MultiLanguageInput("en", tweetid + ":" + text, text), })).Result;

            foreach (var document in results.Documents)
            {
                Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
            }
        }

        private async static Task<LUISResult> GetLUISResult(string message)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "0b86ab46c22944e2a5e7f4f79a6db05d");
            var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/bb1db568-7477-4682-a427-aa761960138f?subscription-key=0b86ab46c22944e2a5e7f4f79a6db05d&timezoneOffset=0&verbose=true&q=" + message;

            HttpResponseMessage response = await client.GetAsync(uri);
            var result = await response.Content.ReadAsStringAsync();

            LUISResult lr = JsonConvert.DeserializeObject<LUISResult>(result);

            return lr;
        }

        private static void GetTweetsByHashTag(string hashtag)
        {
            Auth.SetUserCredentials(_ConsumerKey, _ConsumerSecret, _AccessToken, _AccessTokenSecret);

            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            long sinceId = 1;
            
            var searchParameter = new SearchTweetsParameters(hashtag)
            {
                SearchType = SearchResultType.Recent,
                MaximumNumberOfResults = 10,
                SinceId = sinceId,
                TweetSearchType = TweetSearchType.All,
                Lang = LanguageFilter.English,
            };

            IEnumerable<ITweet> tweets = Search.SearchTweets(searchParameter);

            foreach (ITweet tweet in tweets)
            {
            }
        }

        static void Main(string[] args)
        {
            //GetTweetsByHashTag("#machinelearning");

            //ProcessSentiment("1", "I love this new iphone");
            //ProcessSentiment("2", "I can't stand this new iphone");
            //ProcessSentiment("3", "I'm undecided about this new iphone, will wait a few more weeks");


            //ProcessKeyPhrases("1", "I love this new iphone, almost got a samsung instead!");
            //ProcessKeyPhrases("2", "I can't stand this new iphone");
            //ProcessKeyPhrases("3", "I can't make my mind up about this new iphone, I need to play with some more of its features");

            LUISResult lUISResultLead = GetLUISResult("I'm thinking of getting a new phone, any recommendations?").Result;
            LUISResult lUISResultNoLead = GetLUISResult("I doubt I'll get a new phone this month").Result;
        }
    }
}

