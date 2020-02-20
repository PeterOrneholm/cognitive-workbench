using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Logging;
using Orneholm.CognitiveWorkbench.Web.Models;

namespace Orneholm.CognitiveWorkbench.Web.Controllers
{
    [ApiController]
    [Route("/api")]
    public class ApiController : Controller
    {
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
        }

        [HttpPost("vision/analyze")]
        public async Task<ActionResult<VisionAnalyzeResponse>> VisionAnalyze([FromBody]VisionAnalyzeRequest request)
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

            var analyzeResult = await ImageAnalyzer.Analyze(request);

            return analyzeResult;
        }
    }

    public class ImageAnalyzer
    {
        private static readonly List<VisualFeatureTypes> AnalyzeVisualFeatureTypes = new List<VisualFeatureTypes>
        {
            VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Faces,
            VisualFeatureTypes.Adult,
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Color,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Objects,
            VisualFeatureTypes.Brands
        };

        private static readonly List<Details> AnalyzeDetails = new List<Details>
        {
            Details.Celebrities,
            Details.Landmarks
        };

        public static async Task<ActionResult<VisionAnalyzeResponse>> Analyze(VisionAnalyzeRequest request)
        {
            var computerVisionClient = GetComputerVisionClient(request.ComputerVisionSubscriptionKey, request.ComputerVisionEndpoint);

            var imageAnalysis = AnalyzeImage(request, computerVisionClient);
            var recognizedPrintedText = RecognizedPrintedText(request, computerVisionClient);
            var areaOfInterest = computerVisionClient.GetAreaOfInterestAsync(request.ImageUrl);

            await Task.WhenAll(imageAnalysis, recognizedPrintedText, areaOfInterest);

            return new VisionAnalyzeResponse
            {
                ImageUrl = request.ImageUrl,

                AnalyzeVisualFeatureTypes = AnalyzeVisualFeatureTypes,
                AnalyzeDetails = AnalyzeDetails,

                AnalysisResult = imageAnalysis.Result,
                OcrResult = recognizedPrintedText.Result,
                AreaOfInterestResult = areaOfInterest.Result
            };
        }

        private static async Task<OcrResult> RecognizedPrintedText(VisionAnalyzeRequest request,
            ComputerVisionClient computerVisionClient)
        {
            var ocrLanguage = GetOcrLanguage(request.ImageOcrLanguage);
            return await computerVisionClient.RecognizePrintedTextAsync(true, request.ImageUrl, ocrLanguage);
        }

        private static async Task<ImageAnalysis> AnalyzeImage(VisionAnalyzeRequest request, ComputerVisionClient computerVisionClient)
        {
            return await computerVisionClient.AnalyzeImageAsync(
                request.ImageUrl,
                AnalyzeVisualFeatureTypes,
                AnalyzeDetails,
                language: string.IsNullOrWhiteSpace(request.ImageAnalysisLanguage) ? null : request.ImageAnalysisLanguage
            );
        }

        private static OcrLanguages? GetOcrLanguage(string imageOcrLanguage)
        {
            OcrLanguages? ocrLanguage = null;

            if (!string.IsNullOrWhiteSpace(imageOcrLanguage))
            {
                if (Enum.TryParse<OcrLanguages>(imageOcrLanguage, true, out var parsedPcrLanguage))
                {
                    ocrLanguage = parsedPcrLanguage;
                }
            }

            return ocrLanguage;
        }

        private static ComputerVisionClient GetComputerVisionClient(string subscriptionKey, string endpoint)
        {
            return new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = endpoint
            };
        }
    }
}
