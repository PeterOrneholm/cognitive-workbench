using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Orneholm.CognitiveWorkbench.Web.Extensions;
using Orneholm.CognitiveWorkbench.Web.Models.ComputerVision;
using Orneholm.CognitiveWorkbench.Web.Models.Generic;
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

        private static readonly List<VisualFeatureTypes> AnalyzeVisualFeatureLimitedTypes = new List<VisualFeatureTypes>
        {
            VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Faces,
            VisualFeatureTypes.Adult,
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Color,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Description
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

        public async Task<ComputerVisionAnalyzeResponse> Analyze(string url, AnalysisLanguage analysisLanguage, OcrLanguages ocrLanguage, TextRecognitionMode recognizeTextMode)
        {
            // Computer vision
            var imageAnalysis = ComputerVisionAnalyzeImage(url, analysisLanguage);
            var recognizedPrintedText = ComputerVisionRecognizedPrintedText(url, ocrLanguage);
            var recognizedText = ComputerVisionRecognizedText(url, recognizeTextMode);
            var batchReadText = ComputerVisionBatchRead(url);
            var areaOfInterest = ComputerVisionGetAreaOfInterest(url);

            // Combine
            await Task.WhenAll(imageAnalysis, recognizedPrintedText, recognizedText, batchReadText, areaOfInterest);

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
                AreaOfInterestResult = areaOfInterest.Result,

                OcrResult = recognizedPrintedText.Result,
                RecognizeTextOperationResult = recognizedText.Result,
                BatchReadResult = batchReadText.Result,
            };
        }

        private async Task<ImageAnalysis> ComputerVisionAnalyzeImage(string url, AnalysisLanguage analysisLanguage)
        {
            var visualFeatures = AnalyzeVisualFeatureTypes;

            // API does not support some visual features if analysis is not in English
            if (!AnalysisLanguage.en.Equals(analysisLanguage))
            {
                visualFeatures = AnalyzeVisualFeatureLimitedTypes;
            }

            return await _computerVisionClient.AnalyzeImageAsync(
                url: url,
                visualFeatures: visualFeatures,
                details: AnalyzeDetails,
                language: analysisLanguage.ToString()
            );
        }

        private Task<AreaOfInterestResult> ComputerVisionGetAreaOfInterest(string url)
        {
            return _computerVisionClient.GetAreaOfInterestAsync(url);
        }

        private async Task<TextOperationResult> ComputerVisionRecognizedText(string url, TextRecognitionMode recognizeTextMode)
        {
            var recognizeTextHeaders = await _computerVisionClient.RecognizeTextAsync(url, recognizeTextMode);

            // Retrieve the URI where the recognized text will be stored from the Operation-Location header
            var operationLocation = recognizeTextHeaders.OperationLocation;
            var operationId = operationLocation.Substring(operationLocation.Length - _computerVisionOperationIdLength);

            var result = await _computerVisionClient.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            var iteration = 1;
            var waitDurationInMs = 500;
            var maxWaitTimeInMs = 30000;
            var maxTries = maxWaitTimeInMs / waitDurationInMs;

            while ((result.Status == TextOperationStatusCodes.Running || result.Status == TextOperationStatusCodes.NotStarted)
                && iteration <= maxTries)
            {
                await Task.Delay(waitDurationInMs);

                result = await _computerVisionClient.GetTextOperationResultAsync(operationId);
                iteration++;
            }

            return result;
        }

        private async Task<ReadOperationResult> ComputerVisionBatchRead(string url)
        {
            var batchReadFileHeaders = await _computerVisionClient.BatchReadFileAsync(url);

            // Retrieve the URI where the recognized text will be stored from the Operation-Location header
            var operationLocation = batchReadFileHeaders.OperationLocation;
            var operationId = operationLocation.Substring(operationLocation.Length - _computerVisionOperationIdLength);

            var result = await _computerVisionClient.GetReadOperationResultAsync(operationId);

            // Wait for the operation to complete
            var iteration = 1;
            var waitDurationInMs = 500;
            var maxWaitTimeInMs = 30000;
            var maxTries = maxWaitTimeInMs / waitDurationInMs;

            while ((result.Status == TextOperationStatusCodes.Running || result.Status == TextOperationStatusCodes.NotStarted)
                && iteration <= maxTries)
            {
                await Task.Delay(waitDurationInMs);

                result = await _computerVisionClient.GetReadOperationResultAsync(operationId);
                iteration++;
            }

            return result;
        }

        private async Task<OcrResult> ComputerVisionRecognizedPrintedText(string url, OcrLanguages ocrLanguage)
        {
            return await _computerVisionClient.RecognizePrintedTextAsync(true, url, ocrLanguage);
        }
    }
}
