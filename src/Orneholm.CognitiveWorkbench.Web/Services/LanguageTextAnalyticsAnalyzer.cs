using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Rest;
using Orneholm.CognitiveWorkbench.Web.Models.TextAnalytics;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public class LanguageTextAnalyticsAnalyzer
    {
        private readonly string _endpoint;
        private readonly string _subscriptionKey;

        public LanguageTextAnalyticsAnalyzer(string subscriptionKey, string endpoint)
        {
            _endpoint = endpoint;
            _subscriptionKey = subscriptionKey;
        }

        public async Task<TextAnalyticsAnalyzeResponse> Analyze(string text, string languageHint)
        {
            var credentials = new ApiKeyServiceClientCredentials(_subscriptionKey);
            var client = new TextAnalyticsClient(credentials)
            {
                Endpoint = _endpoint
            };

            var detectedLanguage = await client.DetectLanguageAsync(text, languageHint, true);
            if (string.IsNullOrWhiteSpace(languageHint))
            {
                languageHint = detectedLanguage.DetectedLanguages.FirstOrDefault()?.Iso6391Name ?? "";
            }

            var entities = client.EntitiesAsync(text, languageHint, true);
            var keyPhrases = client.KeyPhrasesAsync(text, languageHint, true);
            var sentiment = client.SentimentAsync(text, languageHint, true);

            await Task.WhenAll(entities, keyPhrases, sentiment);

            return new TextAnalyticsAnalyzeResponse
            {
                DetectedLanguage = detectedLanguage,
                Entities = entities.Result,
                KeyPhrases = keyPhrases.Result,
                Sentiment = sentiment.Result
            };
        }

        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            private readonly string _apiKey;

            public ApiKeyServiceClientCredentials(string apiKey)
            {
                _apiKey = apiKey;
            }

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }
                request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
    }
}
