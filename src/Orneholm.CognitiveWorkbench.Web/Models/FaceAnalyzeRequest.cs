namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class FaceAnalyzeRequest
    {
        public string FaceSubscriptionKey { get; set; } = string.Empty;
        public string FaceEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
    }
}
