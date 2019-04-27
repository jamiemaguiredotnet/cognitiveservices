using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingMaps.SnapToRoad
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            // main endpoint
            var uri = "https://dev.virtualearth.net/REST/v1/Routes/SnapToRoad?";
            
            // Leeds, York, Liverpool lat/long
            string points = "points=54.978252,-1.61778;55.008279,-1.618878;55.01942,-1.621357"; 

            // tell the api to "include the truck speed limit", "speed limit", set the "speed unit" and "travel mode"
            string attributes = "&includeTruckSpeedLimit=true&IncludeSpeedLimit=true&speedUnit=MPH&travelMode=driving";

            string key = "&key=AkiMgDiiHwTQ5Isj6y-pr20vrV5ve8IIoVXLDDMAXsDglmobQaUupD9w12Yjzfqp";

            string output = "&output=json";

            HttpResponseMessage response = client.GetAsync(uri + points + attributes + key + output).Result;

            var result = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
