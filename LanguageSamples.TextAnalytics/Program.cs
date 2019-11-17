using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MVP.Book.TextAnalytics
{
    class Program
    {
        #region keys
        private static string _azureRegion = "";
        private static string _textAnalyticsKey = "";
        #endregion

        #region credentials class
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            private readonly string apiKey;

            public ApiKeyServiceClientCredentials(string apiKey)
            {
                this.apiKey = apiKey;
            }

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.apiKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
        #endregion

        public static TextAnalyticsClient AuthenticateTextAnalytics(string endpoint, string key)
        {
            TextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint, };
            return client;
        }

        private static void ProcessKeyPhrases(string documentid, string text)
        {
            TextAnalyticsClient client = AuthenticateTextAnalytics(_azureRegion, _textAnalyticsKey);

            KeyPhraseResult result = client.KeyPhrases(text);

            Console.WriteLine("Document ID: {0} ", documentid);

            Console.WriteLine("\t Key phrases:");

            foreach (string keyphrase in result.KeyPhrases)
            {
                Console.WriteLine("\t\t" + keyphrase);
            }
        }

        private static void ProcessSentiment(string documentid, string text)
        {
            TextAnalyticsClient client = AuthenticateTextAnalytics(_azureRegion, _textAnalyticsKey);

            SentimentResult results = client.Sentiment(text, "en");

            Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", documentid, results.Score.ToString());
        }

        private static void ProcessEntities(string documentid, string text)
        {
            TextAnalyticsClient client = AuthenticateTextAnalytics(_azureRegion, _textAnalyticsKey);

            var result = client.Entities(text);

            Console.WriteLine("Document ID: {0} ", documentid);
            Console.WriteLine("\t Entities:");

            foreach (EntityRecord entity in result.Entities)
            {
                Console.WriteLine("\t\t" + entity.Name + ".  Type: " + entity.Type.ToUpper() + ".  Wikipedia URL:"  + entity.WikipediaUrl);
            }
        }

        private static void ProcessInstagramData()
        {
            // intsagram caption data (17848572886628551)
            string caption = "LOVE THY XBOX.Took awhile, but we got it.Like and share.  #xboxcontrollers #xboxmemes #xboxplayers #xboxonecontroller " +
                             "#xboxonecontrollercollector #xboxone #xboxones #xboxelitecontroller #xbox #xboxonex #xbox360 #xboxdesignlab " +
                             "#xboxonecontrollercollection #xbox1 #xboxcontroller #xboxonecontrollers #xboxcontrollercollection #xboxcollector " +
                             "#xboxcollection #xbox360controller";

            Console.WriteLine("Processing text:" + Environment.NewLine + caption);

            ProcessSentiment("1", caption);
            ProcessEntities("2", caption);
            ProcessKeyPhrases("3", caption);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Processing text: 'Lots of great new technology unveiled at Microsoft Ignite in Orlando!'");

            ProcessEntities("1,","Lots of great new technology unveiled at Microsoft Ignite in Orlando!");
            ProcessKeyPhrases("1,", "Lots of great new technology unveiled at Microsoft Ignite in Orlando!");
            ProcessSentiment("1", "Lots of great new technology unveiled at Microsoft Ignite in Orlando!");

            ProcessInstagramData();
        }
    }
}
