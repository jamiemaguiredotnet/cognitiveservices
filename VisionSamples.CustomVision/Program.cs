using System;
using System.Threading.Tasks;

namespace VisionSamples.CustomVision
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CustomVisionHelper.MakePredictionRequest(
                @"C:\Users\Jamie\source\repos\cognitiveservices\VisionSamples.CustomVision\Images\BMW_Car_Key.jpg"
                );
        }
    }
}
