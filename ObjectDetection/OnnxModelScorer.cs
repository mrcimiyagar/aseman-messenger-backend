using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using ObjectDetection.DataStructures;
using ObjectDetection.YoloParser;

namespace ObjectDetection
{
    class OnnxModelScorer
    {
        private readonly string photoPath;
        private readonly string modelLocation;
        private readonly MLContext mlContext;

        private IList<YoloBoundingBox> _boundingBoxes = new List<YoloBoundingBox>();
        private readonly YoloWinMlParser _parser = new YoloWinMlParser();

        public OnnxModelScorer(string imagesFolder, string modelLocation)
        {
            this.photoPath = imagesFolder;
            this.modelLocation = modelLocation;
            mlContext = new MLContext();
        }

        public struct ImageNetSettings
        {
            public const int imageHeight = 416;
            public const int imageWidth = 416;
        }

        public struct TinyYoloModelSettings
        {
            public const string ModelInput = "image";
            public const string ModelOutput = "grid";
        }

        public void Score()
        {
            var model = LoadModel(modelLocation);

            PredictDataUsingModel(model);
        }

        private PredictionEngine<ImageNetData, ImageNetPrediction> LoadModel(string modelLocation)
        {
            var data = CreateEmptyDataView();
            var pipeline = mlContext.Transforms.LoadImages(outputColumnName: "image", imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
                            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: ImageNetSettings.imageWidth, imageHeight: ImageNetSettings.imageHeight, inputColumnName: "image"))
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
                            .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelLocation, outputColumnNames: new[] { TinyYoloModelSettings.ModelOutput }, inputColumnNames: new[] { TinyYoloModelSettings.ModelInput }));
            var model = pipeline.Fit(data);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageNetData, ImageNetPrediction>(model);
            return predictionEngine;
        }

        protected void PredictDataUsingModel(PredictionEngine<ImageNetData, ImageNetPrediction> model)
        {
            var sample = new ImageNetData {ImagePath = photoPath, Label = "1"};
            var probs = model.Predict(sample).PredictedLabels;
            _boundingBoxes = _parser.ParseOutputs(probs);
            var filteredBoxes = _parser.NonMaxSuppress(_boundingBoxes, 5, .5F);
            foreach (var box in filteredBoxes)
            {
                Console.WriteLine(box.Label + " and its Confidence score: " + box.Confidence);
            }
        }

        private IDataView CreateEmptyDataView()
        {
            List<ImageNetData> list = new List<ImageNetData>();
            IEnumerable<ImageNetData> enumerableData = list;
            var dv = mlContext.Data.LoadFromEnumerable(enumerableData);
            return dv;
        }
    }
}

