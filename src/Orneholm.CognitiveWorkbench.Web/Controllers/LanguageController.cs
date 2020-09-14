using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orneholm.CognitiveWorkbench.Web.Models.TextAnalytics;
using Orneholm.CognitiveWorkbench.Web.Services;

namespace Orneholm.CognitiveWorkbench.Web.Controllers
{
    public class LanguageController : Controller
    {
        private readonly ILogger<LanguageController> _logger;
        private readonly TelemetryClient _telemetryClient;

        public LanguageController(ILogger<LanguageController> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [HttpGet("/language/text-analytics")]
        public IActionResult TextAnalytics()
        {
            return View(TextAnalyticsViewModel.NotAnalyzed());
        }

        [HttpPost("/language/text-analytics")]
        public async Task<ActionResult<TextAnalyticsViewModel>> TextAnalytics([FromForm]TextAnalyticsAnalyzeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TextAnalyticsSubscriptionKey))
            {
                throw new ArgumentException("Missing or invalid TextAnalyticsSubscriptionKey", nameof(request.TextAnalyticsSubscriptionKey));
            }

            if (string.IsNullOrWhiteSpace(request.TextAnalyticsEndpoint))
            {
                throw new ArgumentException("Missing or invalid TextAnalyticsEndpoint", nameof(request.TextAnalyticsEndpoint));
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                throw new ArgumentException("Missing or invalid Text", nameof(request.Text));
            }

            Track("Language_TextAnalytics");

            var textAnalyzer = new LanguageTextAnalyticsAnalyzer(request.TextAnalyticsSubscriptionKey, request.TextAnalyticsEndpoint);
            var analyzeResult = await textAnalyzer.Analyze(request.Text, request.LanguageHint);

            return View(TextAnalyticsViewModel.Analyzed(request, analyzeResult));
        }

        private void Track(string type)
        {
            _telemetryClient.TrackEvent("AnalyzeLanguage", new Dictionary<string, string>
            {
                { "cwb_type", type }
            });
        }
    }
}
