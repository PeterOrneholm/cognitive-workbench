namespace Orneholm.CognitiveWorkbench.Web.Models.TextAnalytics
{
    public class TextAnalyticsViewModel
    {
        public static TextAnalyticsViewModel NotAnalyzed()
        {
            return new TextAnalyticsViewModel
            {
                IsAnalyzed = false,

                TextAnalyticsAnalyzeRequest = new TextAnalyticsAnalyzeRequest(),
                TextAnalyticsAnalyzeResponse = null
            };
        }

        public static TextAnalyticsViewModel Analyzed(TextAnalyticsAnalyzeRequest computerVisionAnalyzeRequest, TextAnalyticsAnalyzeResponse computerVisionAnalyzeResponse)
        {
            return new TextAnalyticsViewModel
            {
                IsAnalyzed = true,

                TextAnalyticsAnalyzeRequest = computerVisionAnalyzeRequest,
                TextAnalyticsAnalyzeResponse = computerVisionAnalyzeResponse
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public TextAnalyticsAnalyzeRequest TextAnalyticsAnalyzeRequest { get; internal set; } = new TextAnalyticsAnalyzeRequest();
        public TextAnalyticsAnalyzeResponse TextAnalyticsAnalyzeResponse { get; internal set; }
    }
}
