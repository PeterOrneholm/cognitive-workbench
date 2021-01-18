using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orneholm.CognitiveWorkbench.Web.Models.ComputerVision;
using Orneholm.CognitiveWorkbench.Web.Models.CustomVision;
using Orneholm.CognitiveWorkbench.Web.Models.Face;
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
            var errorContent = "";
            if (string.IsNullOrWhiteSpace(request.ComputerVisionSubscriptionKey))
            {
                errorContent += $"Missing or invalid Computer Vision Subscription Key (see 'Azure Settings' tab){Environment.NewLine}";
            }

            if (string.IsNullOrWhiteSpace(request.ComputerVisionEndpoint))
            {
                errorContent += $"Missing or invalid Computer Vision Endpoint (see 'Azure Settings' tab){Environment.NewLine}";
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                errorContent += $"Missing or invalid Image Url";
            }

            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                return View(ComputerVisionViewModel.Analyzed(request, 
                    new ComputerVisionAnalyzeResponse
                    {
                        OtherErrorMessage = "Request not processed due to the following error(s):",
                        OtherErrorContent = errorContent
                    }));
            }

            Track("Vision_ComputerVision");

            var imageAnalyzer = new ImageComputerVisionAnalyzer(request.ComputerVisionSubscriptionKey, request.ComputerVisionEndpoint, _httpClientFactory);
            var analyzeResult = await imageAnalyzer.Analyze(request.ImageUrl, request.ImageAnalysisLanguage, request.ImageOcrLanguage, request.ImageReadV3Language);

            return View(ComputerVisionViewModel.Analyzed(request, analyzeResult));
        }

        [HttpGet("/vision/custom-vision")]
        public IActionResult CustomVision()
        {
            return View(CustomVisionViewModel.NotAnalyzed());
        }

        [HttpPost("/vision/custom-vision")]
        public async Task<ActionResult<CustomVisionViewModel>> CustomVision([FromForm]CustomVisionRequest request)
        {
            var errorContent = "";
            if (string.IsNullOrWhiteSpace(request.CustomVisionPredictionKey))
            {
                errorContent += $"Missing or invalid Custom Vision Prediction Key (see 'Azure Settings' tab){Environment.NewLine}";
            }

            if (string.IsNullOrWhiteSpace(request.CustomVisionEndpoint))
            {
                errorContent += $"Missing or invalid Custom Vision Prediction Endpoint (see 'Azure Settings' tab){Environment.NewLine}";
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                errorContent += $"Missing or invalid Image Url{Environment.NewLine}";
            }

            if (request.ProjectId == null || Guid.Empty.Equals(request.ProjectId))
            {
                errorContent += $"Missing or invalid Project Id{Environment.NewLine}";
            }

            if (string.IsNullOrWhiteSpace(request.IterationPublishedName))
            {
                errorContent += $"Missing or invalid Iteration Published Name{Environment.NewLine}";
            }

            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                return View(CustomVisionViewModel.Analyzed(request, 
                    new CustomVisionResponse
                    {
                        OtherErrorMessage = "Request not processed due to the following error(s):",
                        OtherErrorContent = errorContent
                    }));
            }

            Track("Vision_CustomVision");

            var imageAnalyzer = new ImageCustomVisionAnalyzer(request.CustomVisionPredictionKey, request.CustomVisionEndpoint, _httpClientFactory);
            var analyzeResult = await imageAnalyzer.Analyze(request.ImageUrl, request.ProjectId, request.IterationPublishedName, request.ProjectType);

            return View(CustomVisionViewModel.Analyzed(request, analyzeResult));
        }

        [HttpGet("/vision/face")]
        public IActionResult Face()
        {
            return View(FaceViewModel.NotAnalyzed());
        }

        [HttpPost("/vision/face")]
        public async Task<ActionResult<FaceViewModel>> Face([FromForm]FaceAnalyzeRequest request)
        {
            var errorContent = "";
            if (string.IsNullOrWhiteSpace(request.FaceSubscriptionKey))
            {
                errorContent += $"Missing or invalid Face Subscription Key (see 'Azure Settings' tab){Environment.NewLine}";
            }

            if (string.IsNullOrWhiteSpace(request.FaceEndpoint))
            {
                errorContent += $"Missing or invalid Face Endpoint (see 'Azure Settings' tab){Environment.NewLine}";
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                errorContent += $"Missing or invalid Image Url{Environment.NewLine}";
            }

            if (request.EnableIdentification && string.IsNullOrWhiteSpace(request.IdentificationGroupId))
            {
                errorContent += $"Missing or invalid Identification Group Id{Environment.NewLine}";
            }

            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                return View(FaceViewModel.Analyzed(request, 
                    new FaceAnalyzeResponse
                    {
                        OtherErrorMessage = "Request not processed due to the following error(s):",
                        OtherErrorContent = errorContent
                    }));
            }

            Track("Vision_Face");

            var imageAnalyzer = new ImageFaceAnalyzer(request.FaceSubscriptionKey, request.FaceEndpoint, _httpClientFactory);
            var analyzeResult = await imageAnalyzer.Analyze(request.ImageUrl, request.DetectionModel, request.EnableIdentification, request.RecognitionModel, request.IdentificationGroupType, request.IdentificationGroupId);

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
