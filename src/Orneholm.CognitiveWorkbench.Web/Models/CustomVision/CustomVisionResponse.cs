using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Orneholm.CognitiveWorkbench.Web.Models.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.CustomVision
{
    public class CustomVisionResponse
    {
        public ImageInfo ImageInfo { get; set; } = new ImageInfo();

        public List<PredictionModel> Predictions { get; set; } = new List<PredictionModel>();

        public string ApiRequestErrorMessage { get; set; }
        public string ApiRequestErrorContent { get; set; }
        public string OtherErrorMessage { get; set; }
        public string OtherErrorContent { get; set; }
    }
}
