namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class VisionAnalyzeRequest
    {
        public string ComputerVisionSubscriptionKey { get; set; } = string.Empty;
        public string ComputerVisionEndpoint { get; set; } = string.Empty;

        public string FaceSubscriptionKey { get; set; } = string.Empty;
        public string FaceEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public string ImageAnalysisLanguage { get; set; } = string.Empty;
        public string ImageOcrLanguage { get; set; } = string.Empty;
    }
}
