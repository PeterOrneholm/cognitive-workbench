using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class CustomVisionAnalyzeResponse
    {
        public ImageInfo ImageInfo { get; set; } = new ImageInfo();

        public List<PredictionModel> Predictions { get; set; } = new List<PredictionModel>();
    }
}
