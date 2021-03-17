using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Orneholm.CognitiveWorkbench.Web.Models.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision
{
    public class ComputerVisionAnalyzeResponse
    {
        public ImageInfo ImageInfo { get; set; } = new ImageInfo();

        public List<VisualFeatureTypes?> AnalyzeVisualFeatureTypes { get; set; } = new List<VisualFeatureTypes?>();
        public List<Details?> AnalyzeDetails { get; set; } = new List<Details?>();

        public ImageAnalysis AnalysisResult { get; set; } = new ImageAnalysis();
        public AreaOfInterestResult AreaOfInterestResult { get; set; } = new AreaOfInterestResult();
        public ReadOperationResult ReadResult { get; set; } = new ReadOperationResult();
        public OcrResult OcrResult { get; set; } = new OcrResult();
        
        public List<DetectedFace> FaceResult { get; set; } = new List<DetectedFace>();

        public string ApiRequestErrorMessage { get; set; }
        public string ApiRequestErrorContent { get; set; }
        public string OtherErrorMessage { get; set; }
        public string OtherErrorContent { get; set; }
    }
}
