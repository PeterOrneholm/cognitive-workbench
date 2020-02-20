using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class VisionAnalyzeRequest
    {
        public string ComputerVisionSubscriptionKey { get; set; } = string.Empty;
        public string ComputerVisionEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public string ImageAnalysisLanguage { get; set; } = string.Empty;
        public string ImageOcrLanguage { get; set; } = string.Empty;
    }

    public class VisionAnalyzeResponse
    {
        public string ImageUrl { get; set; } = string.Empty;

        public List<VisualFeatureTypes> AnalyzeVisualFeatureTypes { get; set; } = new List<VisualFeatureTypes>();
        public List<Details> AnalyzeDetails { get; set; } = new List<Details>();

        public ImageAnalysis? AnalysisResult { get; set; }
        public OcrResult? OcrResult { get; set; }
        public AreaOfInterestResult? AreaOfInterestResult { get; set; }
    }
}
