using CognitiveSamples.LUIS;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace CognitiveSamples
{
    class Program
    {

        private static string _ConsumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
        private static string _ConsumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
        private static string _AccessToken = ConfigurationManager.AppSettings.Get("AccessToken");
        private static string _AccessTokenSecret = ConfigurationManager.AppSettings.Get("AccessTokenSecret");

        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
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

        private static void ProcessSentiment(string tweetid, string text)
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

        private static void ProcessEntities(string tweetid, string text)
        {
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = AzureRegions.Westeurope;

            EntitiesBatchResult result2 = client.EntitiesAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("en", tweetid + ":" + text, text)
                        })).Result;

            foreach (var document in result2.Documents)
            {
                Console.WriteLine("Document ID: {0} ", document.Id);

                Console.WriteLine("\t Entities:");

                foreach (EntityRecord entity in document.Entities)
                {
                    Console.WriteLine("\t\t" + entity.Name + " " + entity.WikipediaUrl);
                }
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

            //ProcessEntities("1", "I love this new iphone");
            //ProcessEntities("2", "I can't stand this new iphone");
            //ProcessEntities("3", "I'm undecided about this new iphone, will wait a few more weeks");

            //ProcessKeyPhrases("1", "I love this new iphone");
            //ProcessKeyPhrases("2", "I can't stand this new iphone");
            //ProcessKeyPhrases("3", "I'm undecided about this new iphone, will wait a few more weeks");

            LUISResult lUISResultLead = GetLUISResult("I'm thinking of getting a new phone, any recommendations?").Result;
        }
    }
}

