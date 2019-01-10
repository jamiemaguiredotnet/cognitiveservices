using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BingSearchSamples.VisualSearch
{
    class Program
    {
        const string accessKey = "";
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/visualsearch";
        static string imagePath = @"C:\Users\jamie\Documents\Visual Studio 2015\Projects\CognitiveSamples\BingSearchSamples.VisualSearch\satya.jpg";

        // Boundary strings for form data in body of POST.
        const string CRLF = "\r\n";
        static string BoundaryTemplate = "batch_{0}";
        static string StartBoundaryTemplate = "--{0}";
        static string EndBoundaryTemplate = "--{0}--";

        const string CONTENT_TYPE_HEADER_PARAMS = "multipart/form-data; boundary={0}";
        const string POST_BODY_DISPOSITION_HEADER = "Content-Disposition: form-data; name=\"image\"; filename=\"{0}\"" + CRLF + CRLF;

        static string GetImageFileName(string path)
        {
            return new FileInfo(path).Name;
        }

        static byte[] GetImageBinary(string path)
        {
            return File.ReadAllBytes(path);
        }

        static string BuildFormDataStart(string boundary, string filename)
        {
            var startBoundary = string.Format(StartBoundaryTemplate, boundary);

            var requestBody = startBoundary + CRLF;
            requestBody += string.Format(POST_BODY_DISPOSITION_HEADER, filename);

            return requestBody;
        }

        static string BuildFormDataEnd(string boundary)
        {
            return CRLF + CRLF + string.Format(EndBoundaryTemplate, boundary) + CRLF;
        }

        static string BingImageSearch(string startFormData, string endFormData, byte[] image, string contentTypeValue)
        {
            WebRequest request = HttpWebRequest.Create(uriBase);
            request.ContentType = contentTypeValue;
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            request.Method = "POST";

            // Writes the boundary and Content-Disposition header, then writes
            // the image binary, and finishes by writing the closing boundary.
            using (Stream requestStream = request.GetRequestStream())
            {
                StreamWriter writer = new StreamWriter(requestStream);
                writer.Write(startFormData);
                writer.Flush();
                requestStream.Write(image, 0, image.Length);
                writer.Write(endFormData);
                writer.Flush();
                writer.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return json;
        }

        static string JsonPrettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }

        static void Main(string[] args)
        {
            var filename = GetImageFileName(imagePath);
            var imageBinary = GetImageBinary(imagePath);

            var boundary = string.Format(BoundaryTemplate, Guid.NewGuid());

            var startFormData = BuildFormDataStart(boundary, filename);
            var endFormData = BuildFormDataEnd(boundary);

            var contentTypeHdrValue = string.Format(CONTENT_TYPE_HEADER_PARAMS, boundary);

            var json = BingImageSearch(startFormData, endFormData, imageBinary, contentTypeHdrValue);

            Console.WriteLine(JsonPrettify(json));
            Console.WriteLine("enter any key to continue");
            Console.ReadKey();
        }

    }
}
