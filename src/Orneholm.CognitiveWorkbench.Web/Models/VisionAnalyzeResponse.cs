using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class VisionAnalyzeResponse
    {
        public string ImageUrl { get; set; } = string.Empty;

        public List<VisualFeatureTypes> AnalyzeVisualFeatureTypes { get; set; } = new List<VisualFeatureTypes>();
        public List<Details> AnalyzeDetails { get; set; } = new List<Details>();
        public List<FaceAttributeType> FaceAttributes { get; set; } = new List<FaceAttributeType>();

        public ImageAnalysis? AnalysisResult { get; set; }
        public OcrResult? OcrResult { get; set; }
        public AreaOfInterestResult? AreaOfInterestResult { get; set; }

        public List<DetectedFace> FaceResult { get; set; } = new List<DetectedFace>();
    }
}
