using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DataKeeperPeer
{
    public class PhotoIndexerService
    {
        private static PhotoIndexerService _pis = new PhotoIndexerService();
        private readonly BlockingCollection<string> _photoPaths = new BlockingCollection<string>();
        
        public PhotoIndexerService()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var photoPath = _photoPaths.Take();
                    ObjectDetection.Program.ScanImage(photoPath);
                    /*try
                    {
                        var modelScorer = new OnnxModelScorer(imagesFolder, modelFilePath);
                        modelScorer.Score();
                        //var tags = modelScorer.Score();

                        //foreach (var tag in tags)
                        //{
                        //    Console.WriteLine(tag);
                        //}
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }*/
                }
            });
        }

        public static void EnqueuePhotoForScan(string photoPath)
        {
            _pis._photoPaths.Add(photoPath);
        }
    }
}