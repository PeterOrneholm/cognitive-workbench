using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ComputerVisionApiClient = Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient;
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
        public ComputerVisionApiClient.ReadOperationResult ReadV3Result { get; set; } = new ComputerVisionApiClient.ReadOperationResult();
        public OcrResult OcrResult { get; set; } = new OcrResult();
        
        public List<DetectedFace> FaceResult { get; set; } = new List<DetectedFace>();

        public string ApiRequestErrorMessage { get; set; }
        public string ApiRequestErrorContent { get; set; }
        public string OtherErrorMessage { get; set; }
        public string OtherErrorContent { get; set; }
    }
}
