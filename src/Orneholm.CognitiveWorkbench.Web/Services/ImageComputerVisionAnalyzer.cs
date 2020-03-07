using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Orneholm.CognitiveWorkbench.Web.Extensions;
using Orneholm.CognitiveWorkbench.Web.Models;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public class ImageComputerVisionAnalyzer
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

        private readonly ComputerVisionClient _computerVisionClient;

        private readonly int _computerVisionOperationIdLength = 36;

        public ImageComputerVisionAnalyzer(string computerVisionSubscriptionKey, string computerVisionEndpoint)
        {
            _computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(computerVisionSubscriptionKey))
            {
                Endpoint = computerVisionEndpoint
            };
        }

        public async Task<ComputerVisionAnalyzeResponse> Analyze(string url, string analysisLanguage, string ocrLanguage, string recognizeTextMode)
        {
            // Computer vision
            var imageAnalysis = ComputerVisionAnalyzeImage(url, analysisLanguage);
            var recognizedPrintedText = ComputerVisionRecognizedPrintedText(url, ocrLanguage);
            var recognizedText = ComputerVisionRecognizedText(url, recognizeTextMode);
            var areaOfInterest = ComputerVisionGetAreaOfInterest(url);

            // Combine
            await Task.WhenAll(imageAnalysis, recognizedPrintedText, recognizedText, areaOfInterest);

            return new ComputerVisionAnalyzeResponse
            {
                ImageInfo = new ImageInfo
                {
                    Url = url,
                    Description = imageAnalysis.Result.Description?.Captions?.FirstOrDefault()?.Text.ToSentence(),

                    Width = imageAnalysis.Result.Metadata.Width,
                    Height = imageAnalysis.Result.Metadata.Height
                },

                AnalyzeVisualFeatureTypes = AnalyzeVisualFeatureTypes,
                AnalyzeDetails = AnalyzeDetails,

                AnalysisResult = imageAnalysis.Result,
                OcrResult = recognizedPrintedText.Result,
                RecognizeTextOperationResult = recognizedText?.Result,
                AreaOfInterestResult = areaOfInterest.Result
            };
        }

        private async Task<ImageAnalysis> ComputerVisionAnalyzeImage(string url, string analysisLanguage)
        {
            return await _computerVisionClient.AnalyzeImageAsync(
                url,
                AnalyzeVisualFeatureTypes,
                AnalyzeDetails,
                language: string.IsNullOrWhiteSpace(analysisLanguage) ? null : analysisLanguage
            );
        }

        private Task<AreaOfInterestResult> ComputerVisionGetAreaOfInterest(string url)
        {
            return _computerVisionClient.GetAreaOfInterestAsync(url);
        }

        private async Task<TextOperationResult> ComputerVisionRecognizedText(string url, string recognizeTextMode)
        {
            var parsedRecognizeTextMode = GetTextRecognitionMode(recognizeTextMode, TextRecognitionMode.Printed);
            var recognizeTextHeaders = await _computerVisionClient.RecognizeTextAsync(url, parsedRecognizeTextMode);

            // Retrieve the URI where the recognized text will be stored from the Operation-Location header
            var operationLocation = recognizeTextHeaders.OperationLocation;
            var operationId = operationLocation.Substring(operationLocation.Length - _computerVisionOperationIdLength);

            var result = await _computerVisionClient.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            var i = 1;
            var waitDurationInMs = 500;
            var maxWaitTimeInMs = 30000;
            var maxTries = maxWaitTimeInMs / waitDurationInMs;

            while ((result.Status == TextOperationStatusCodes.Running || result.Status == TextOperationStatusCodes.NotStarted)
                && i <= maxTries)
            {
                await Task.Delay(waitDurationInMs);

                result = await _computerVisionClient.GetTextOperationResultAsync(operationId);
                i++;
            }

            return result;
        }

        private static TextRecognitionMode GetTextRecognitionMode(string recognizeTextMode, TextRecognitionMode defaultMode)
        {
            var recognitionMode = defaultMode;

            if (!string.IsNullOrWhiteSpace(recognizeTextMode))
            {
                if (Enum.TryParse<TextRecognitionMode>(recognizeTextMode, true, out var parsedRecognitionMode))
                {
                    recognitionMode = parsedRecognitionMode;
                }
            }

            return recognitionMode;
        }

        private async Task<OcrResult> ComputerVisionRecognizedPrintedText(string url, string ocrLanguage)
        {
            var parsedOcrLanguage = GetOcrLanguage(ocrLanguage);
            return await _computerVisionClient.RecognizePrintedTextAsync(true, url, parsedOcrLanguage);
        }

        private static OcrLanguages? GetOcrLanguage(string imageOcrLanguage)
        {
            OcrLanguages? ocrLanguage = null;

            if (!string.IsNullOrWhiteSpace(imageOcrLanguage))
            {
                if (Enum.TryParse<OcrLanguages>(imageOcrLanguage, true, out var parsedOcrLanguage))
                {
                    ocrLanguage = parsedOcrLanguage;
                }
            }

            return ocrLanguage;
        }
    }
}
