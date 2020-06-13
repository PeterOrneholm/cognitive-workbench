using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Orneholm.CognitiveWorkbench.Web.Models.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.CustomVision
{
    public class CustomVisionResponse
    {
        public ImageInfo ImageInfo { get; set; } = new ImageInfo();

        public List<PredictionModel> Predictions { get; set; } = new List<PredictionModel>();
    }
}
