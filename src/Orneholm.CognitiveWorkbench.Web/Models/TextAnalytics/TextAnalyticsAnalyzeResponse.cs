using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models.TextAnalytics
{
    public class TextAnalyticsAnalyzeResponse
    {
        public LanguageResult DetectedLanguage { get; set; }
        public EntitiesResult Entities { get; set; }
        public KeyPhraseResult KeyPhrases { get; set; }
        public SentimentResult Sentiment { get; set; }
    }
}
