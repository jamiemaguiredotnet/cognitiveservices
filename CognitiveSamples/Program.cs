using CognitiveSamples.LUIS;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

        //private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        //{
        //    public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //    {
        //        request.Headers.Add("Ocp-Apim-Subscription-Key", "3f9cb70d2fb94d90b793cb77ece1d794");
        //        return base.ProcessHttpRequestAsync(request, cancellationToken);
        //    }
        //}

        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            private string _key;
            public ApiKeyServiceClientCredentials(string key)
            {
                _key = key;
            }

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        private static void ProcessKeyPhrases(string documentid, string text)
        {
            var credentials = new ApiKeyServiceClientCredentials("key here");
           
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials("key here"));

            client.AzureRegion = AzureRegions.Westeurope;

            KeyPhraseBatchResult result2 = client.KeyPhrasesAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("en", documentid + ":" + text, text)
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

        private static void ProcessSentiment(string documentid, string text)
        {
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials("your key here"));

            client.AzureRegion = AzureRegions.Westeurope;

            SentimentBatchResult results = client.SentimentAsync(
                    new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {new MultiLanguageInput("en", documentid + ":" + text, text), })).Result;

            foreach (var document in results.Documents)
            {
                Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
            }
        }


        private static void ProcessEntities(string documentid, string text)
        {
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials("key here"));
            client.AzureRegion = AzureRegions.Westeurope;
            
            EntitiesBatchResult result2 = client.EntitiesAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("en", documentid + ":" + text, text)
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

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "your key here");
            var uri = "your luis url here" + message;

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

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint, };
            return client;
        }

        public static async Task AnalyzeImageLocal(string localImage)
        {
            ComputerVisionClient client = Authenticate("your endpoint here", "your key here");


            //client.AnalyzeImageInStreamWithHttpMessagesAsync()

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - LOCAL IMAGE");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
                {
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Tags,
                    VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                    VisualFeatureTypes.Objects
                };

            Console.WriteLine($"Analyzing the local image {Path.GetFileName(localImage)}...");
            Console.WriteLine();

            using (System.IO.Stream analyzeImageStream = File.OpenRead(localImage))
            {
                try
                {
                    // Analyze the local image.
                    ImageAnalysis results = null;
                    try
                    {
                        results = await client.AnalyzeImageInStreamAsync(analyzeImageStream, features);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    // Sunmarizes the image content.
                    Console.WriteLine("Summary:");
                    foreach (var caption in results.Description.Captions)
                    {
                        Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
                    }
                    Console.WriteLine();

                    // Display categories the image is divided into.
                    Console.WriteLine("Categories:");
                    foreach (var category in results.Categories)
                    {
                        Console.WriteLine($"{category.Name} with confidence {category.Score}");
                    }
                    Console.WriteLine();

                    // Image tags with confidence score.
                    Console.WriteLine("Tags:");
                    foreach (var tag in results.Tags)
                    {
                        Console.WriteLine($"{tag.Name} {tag.Confidence}");
                    }
                    Console.WriteLine();


                    // Objects.
                    Console.WriteLine("Objects:");
                    foreach (var obj in results.Objects)
                    {
                        Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                            $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
                    }
                    Console.WriteLine();

                    // Well-known brands, if any.
                    Console.WriteLine("Brands:");
                    foreach (var brand in results.Brands)
                    {
                        Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
                            $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
                    }
                    Console.WriteLine();
                    // Identifies the color scheme.
                    Console.WriteLine("Color Scheme:");
                    Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
                    Console.WriteLine("Accent color: " + results.Color.AccentColor);
                    Console.WriteLine("Dominant background color: " + results.Color.DominantColorBackground);
                    Console.WriteLine("Dominant foreground color: " + results.Color.DominantColorForeground);
                    Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task Main(string[] args)
        {
            //ProcessSentiment("1", "The new audi a3 looks nice.");
            //GetTweetsByHashTag("#machinelearning");

            //ProcessSentiment("2", "I can't stand this new iphone");
            //ProcessSentiment("3", "I'm undecided about this new iphone, will wait a few more weeks");

            //ProcessEntities("1", "Are you going to Microsoft Build?");
            //ProcessEntities("2", "I booked my flight with British Airways through John Doe.");
            //ProcessEntities("3", "There is a new Audi dealership opening in London.");

            ProcessKeyPhrases("1", "I love this new iphone");
            ProcessKeyPhrases("2", "I can't stand this new samsung");
            ProcessKeyPhrases("3", "I'm undecided about this new iphone, will wait a few more months");

            //LUISResult lUISResultLead = GetLUISResult("I'm thinking of getting a new phone, any recommendations?").Result;

            // move image recognition to another project..
            //await AnalyzeImageLocal(@"C:\temp\image_18019745230236222.jpg");
            //await Vision.CustomVision.MakePredictionRequest(@"C:\Users\Jamie\source\repos\cognitiveservices\CognitiveSamples\Vision\Images\BMW_Car_Key.jpg");

            //Vision.CustomVision customVision = new Vision.CustomVision();
            //ImagePrediction imagePrediction = customVision.MakePrediction(@"C:\Users\Jamie\source\repos\cognitiveservices\CognitiveSamples\Vision\Images\BMW_Car_Key.jpg");
        }
    }
}

