using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class ComputerVisionAnalyzeResponse
    {
        public ImageInfo ImageInfo { get; set; } = new ImageInfo();

        public List<VisualFeatureTypes> AnalyzeVisualFeatureTypes { get; set; } = new List<VisualFeatureTypes>();
        public List<Details> AnalyzeDetails { get; set; } = new List<Details>();

        public ImageAnalysis AnalysisResult { get; set; } = new ImageAnalysis();
        public OcrResult OcrResult { get; set; } = new OcrResult();
        public TextOperationResult RecognizeTextOperationResult { get; set; } = new TextOperationResult();
        public ReadOperationResult BatchReadResult { get; set; } = new ReadOperationResult();
        public AreaOfInterestResult AreaOfInterestResult { get; set; } = new AreaOfInterestResult();

        public List<DetectedFace> FaceResult { get; set; } = new List<DetectedFace>();
    }
}
