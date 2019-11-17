using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VisionSamples.CustomVision
{
    public class CustomVisionHelper
    {

        private StreamReader GetImageAsStream(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            return sr;
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        public static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid Prediction-Key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "key here");

            // Prediction URL - from the web admin console
            string url = "your url and key here";

            HttpResponseMessage response;

            // Request body. 
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                // response from Custom Vision API
                response = await client.PostAsync(url, content);

                Console.WriteLine(JsonHelper.FormatJson((await response.Content.ReadAsStringAsync())));
            }
        }

        public ImagePrediction MakePrediction(string localfile)
        {
            CustomVisionPredictionClient client = new CustomVisionPredictionClient()
            {
                ApiKey = "key",
                Endpoint = "prediction url from admin console"
            };
            Guid projectId = new Guid("project GUID");

            ImagePrediction imagePrediction = null;

            MemoryStream testImage = new MemoryStream(File.ReadAllBytes(localfile));

            client.ClassifyImage(projectId, "DataEventProject", testImage);
            return imagePrediction;
        }

    }
}
