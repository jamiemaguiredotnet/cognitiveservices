using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MVP.AI.Book
{
    class Program
    {
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint, };
            return client;
        }

        public static async Task AnalyseImageLocalMachine(string localImage)
        {
            ComputerVisionClient client = Authenticate("url here", "key here");

            // the list of features we want to find in the image 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
                {
                    VisualFeatureTypes.Objects,
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Tags,
                    VisualFeatureTypes.Color, VisualFeatureTypes.Brands
                };

            Console.WriteLine($"Analysing local image {Path.GetFileName(localImage)}...");
            
            using (Stream imageStream = File.OpenRead(localImage))
            {
                try
                {
                    // Analyse the local image.
                    ImageAnalysis results = results = await client.AnalyzeImageInStreamAsync(imageStream, features);
                    
                    // Summary of image content.
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
                        Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence}");
                    }
                    Console.WriteLine();
                    // detect Well-known brands
                    Console.WriteLine("Brands:");
                    foreach (var brand in results.Brands)
                    {
                        Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence}");
                    }
                    Console.WriteLine();

                    // colours                 
                    Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
                    Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task AnalyseImageFromURL(string url)
        {
            ComputerVisionClient client = Authenticate("url here", "key here");
            
            // the list of features we want to find in the image 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
                {
                    VisualFeatureTypes.Objects,
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Tags,
                    VisualFeatureTypes.Color, VisualFeatureTypes.Brands
                };

            Console.WriteLine($"Analysing image from URL" + url);
            Console.WriteLine();

            try
            {
                // Analyze the local image.
                ImageAnalysis results = results = await client.AnalyzeImageAsync(url, features);

                // Summary of the image content.
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
                // Image tags along with confidence score.
                Console.WriteLine("Tags:");
                foreach (var tag in results.Tags)
                {
                    Console.WriteLine($"{tag.Name} {tag.Confidence}");
                }
                Console.WriteLine();
                // any objects that exisit 
                Console.WriteLine("Objects:");
                foreach (var obj in results.Objects)
                {
                    Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence}");
                }
                Console.WriteLine();
                // detect Well-known brands
                Console.WriteLine("Brands:");
                foreach (var brand in results.Brands)
                {
                    Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence}");
                }
                Console.WriteLine();

                // colours                 
                Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
                Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static async Task Main(string[] args)
        {
            // laptop
            await AnalyseImageLocalMachine(@"C:\Users\Jamie\source\repos\cognitiveservices\VisionSamples.ComputerVision\Images\17896185895385532.jpg");

            //// windows logo and lady
            await AnalyseImageLocalMachine(@"C:\Users\Jamie\source\repos\cognitiveservices\VisionSamples.ComputerVision\Images\17906461345365757.jpg");

            //// Bill Gates
            await AnalyseImageLocalMachine(@"C:\Users\Jamie\source\repos\cognitiveservices\VisionSamples.ComputerVision\Images\18097669819063478.jpg");

            #region lion
            //// lion
            //await AnalyseImageFromURL("https://upload.wikimedia.org/wikipedia/commons/7/73/Lion_waiting_in_Namibia.jpg");
            #endregion

            // car key - problematic with Computer Vision API (trained Custom Vision API model can handle this)
            await AnalyseImageFromURL("https://upload.wikimedia.org/wikipedia/commons/5/55/BMW_Car_Key.jpg");
        }
    }
}
