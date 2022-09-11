using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ObjectDetection
{
    public class Program
    {
        public static void ScanImage(string path)
        {
            Console.WriteLine("Scanning image...");
            var modelFilePath = "/home/keyhan/projects/dotnet/SkyLabMicroServicesV1/ObjectDetection/YoloModel/TinyYolo2_model.onnx";
            
            try
            {
                var modelScorer = new OnnxModelScorer(path, modelFilePath);
                modelScorer.Score();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}



