namespace Orneholm.CognitiveWorkbench.Web.Models.TextAnalytics
{
    public class TextAnalyticsAnalyzeRequest
    {
        public string TextAnalyticsSubscriptionKey { get; set; } = string.Empty;
        public string TextAnalyticsEndpoint { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public string LanguageHint { get; set; } = string.Empty;
    }
}
