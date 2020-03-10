using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orneholm.CognitiveWorkbench.Web.Models;
using Orneholm.CognitiveWorkbench.Web.Services;

namespace Orneholm.CognitiveWorkbench.Web.Controllers
{
    public class VisionController : Controller
    {
        private readonly ILogger<VisionController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TelemetryClient _telemetryClient;

        public VisionController(ILogger<VisionController> logger, IHttpClientFactory httpClientFactory, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _telemetryClient = telemetryClient;
        }

        [HttpGet("/vision/computer-vision")]
        public IActionResult ComputerVision()
        {
            return View(ComputerVisionViewModel.NotAnalyzed());
        }

        [HttpPost("/vision/computer-vision")]
        public async Task<ActionResult<ComputerVisionViewModel>> ComputerVision([FromForm]ComputerVisionAnalyzeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ComputerVisionSubscriptionKey))
            {
                throw new ArgumentException("Missing or invalid ComputerVisionSubscriptionKey", nameof(request.ComputerVisionSubscriptionKey));
            }

            if (string.IsNullOrWhiteSpace(request.ComputerVisionEndpoint))
            {
                throw new ArgumentException("Missing or invalid ComputerVisionEndpoint", nameof(request.ComputerVisionEndpoint));
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                throw new ArgumentException("Missing or invalid ImageUrl", nameof(request.ImageUrl));
            }

            Track("Vision_ComputerVision");

            var imageAnalyzer = new ImageComputerVisionAnalyzer(request.ComputerVisionSubscriptionKey, request.ComputerVisionEndpoint);
            var analyzeResult = await imageAnalyzer.Analyze(request.ImageUrl, request.ImageAnalysisLanguage, request.ImageOcrLanguage, request.ImageRecognizeTextMode);

            return View(ComputerVisionViewModel.Analyzed(request, analyzeResult));
        }


        [HttpGet("/vision/face")]
        public IActionResult Face()
        {
            return View(FaceViewModel.NotAnalyzed());
        }

        [HttpPost("/vision/face")]
        public async Task<ActionResult<FaceViewModel>> Face([FromForm]FaceAnalyzeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FaceSubscriptionKey))
            {
                throw new ArgumentException("Missing or invalid FaceSubscriptionKey", nameof(request.FaceSubscriptionKey));
            }

            if (string.IsNullOrWhiteSpace(request.FaceEndpoint))
            {
                throw new ArgumentException("Missing or invalid FaceEndpoint", nameof(request.FaceEndpoint));
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                throw new ArgumentException("Missing or invalid ImageUrl", nameof(request.ImageUrl));
            }

            Track("Vision_Face");

            var imageAnalyzer = new ImageFaceAnalyzer(request.FaceSubscriptionKey, request.FaceEndpoint, _httpClientFactory);
            var analyzeResult = await imageAnalyzer.Analyze(request.ImageUrl);

            return View(FaceViewModel.Analyzed(request, analyzeResult));
        }

        private void Track(string type)
        {
            _telemetryClient.TrackEvent("AnalyzeImage", new Dictionary<string, string>()
            {
                { "cwb_type", type }
            });
        }
    }
}
