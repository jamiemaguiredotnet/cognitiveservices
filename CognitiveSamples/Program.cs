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
                        {
                          new MultiLanguageInput("en", tweetid + ":" + text, text),
                                                 })).Result;

            foreach (var document in results.Documents)
            {
                Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
            }
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
                ProcessSentiment(tweet.IdStr, tweet.Text);
                ProcessKeyPhrases(tweet.IdStr, tweet.Text);
            }
        }

        static void Main(string[] args)
        {
            GetTweetsByHashTag("#machinelearning");
        }
    }
}

