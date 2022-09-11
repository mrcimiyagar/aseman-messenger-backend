using Microsoft.ML.Data;

namespace ObjectDetection.DataStructures
{
    public class ImageNetPrediction
    {
        [ColumnName(OnnxModelScorer.TinyYoloModelSettings.ModelOutput)]
        public float[] PredictedLabels;
    }
}
