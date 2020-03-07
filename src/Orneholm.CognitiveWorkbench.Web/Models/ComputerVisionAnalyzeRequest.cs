namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class ComputerVisionAnalyzeRequest
    {
        public string ComputerVisionSubscriptionKey { get; set; } = string.Empty;
        public string ComputerVisionEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public string ImageAnalysisLanguage { get; set; } = string.Empty;
        public string ImageOcrLanguage { get; set; } = string.Empty;
        public string ImageRecognizeTextMode { get; set; } = string.Empty;
    }
}
