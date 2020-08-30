using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ComputerVisionApiClient = Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient;
using Newtonsoft.Json;
using Orneholm.CognitiveWorkbench.Web.Extensions;
using Orneholm.CognitiveWorkbench.Web.Models.ComputerVision;
using Orneholm.CognitiveWorkbench.Web.Models.Generic;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public class ImageComputerVisionAnalyzer
    {
        private static readonly List<VisualFeatureTypes?> AnalyzeVisualFeatureTypes = new List<VisualFeatureTypes?>
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

        private static readonly List<VisualFeatureTypes?> AnalyzeVisualFeatureLimitedTypes = new List<VisualFeatureTypes?>
        {
            VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Faces,
            VisualFeatureTypes.Adult,
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Color,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Description
        };

        private static readonly List<Details?> AnalyzeDetails = new List<Details?>
        {
            Details.Celebrities,
            Details.Landmarks
        };

        private readonly string _endpoint;
        private readonly string _subscriptionKey;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly int _computerVisionOperationIdLength = 36;
        private ComputerVisionClient _computerVisionClient;

        public ImageComputerVisionAnalyzer(string computerVisionSubscriptionKey, string computerVisionEndpoint, IHttpClientFactory httpClientFactory)
        {
            _endpoint = computerVisionEndpoint;
            _subscriptionKey = computerVisionSubscriptionKey;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ComputerVisionAnalyzeResponse> Analyze(string url, AnalysisLanguage analysisLanguage, 
            OcrLanguages ocrLanguage, ComputerVisionApiClient.ReadV3Language readLanguage)
        {
            // Setup
            _computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_subscriptionKey)) { Endpoint = _endpoint };

            // Computer vision
            var imageAnalysis = ComputerVisionAnalyzeImage(url, analysisLanguage);
            var areaOfInterest = ComputerVisionGetAreaOfInterest(url);
            var readV3 = ComputerVisionReadV3(url, readLanguage);
            var recognizedPrintedText = ComputerVisionRecognizedPrintedText(url, ocrLanguage);

            // Combine
            await Task.WhenAll(imageAnalysis, areaOfInterest, readV3, recognizedPrintedText);

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
                ReadV3Result = readV3.Result
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

        private async Task<ComputerVisionApiClient.ReadOperationResult> ComputerVisionReadV3(string url, ComputerVisionApiClient.ReadV3Language readLanguage)
        {
            var requestOperationEndpoint = "/vision/v3.0/read/analyze";
            var resultOperationEndpoint = "/vision/v3.0/read/analyzeResults/";

            // Request headers
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            // Read request
            var readRequestUrl = QueryHelpers.AddQueryString(new UriBuilder($"{_endpoint}{requestOperationEndpoint}").ToString(), "language", readLanguage.ToString());
            var requestContent = new ComputerVisionApiClient.ReadOperationRequest() { Url = url };
            var content = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync(readRequestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"ComputerVisionReadV3 - Read request failed - Reponse code: {response.StatusCode} - Response message: '{await response.Content.ReadAsStringAsync()}'");
            }

            // Retrieve the URI where the result will be stored from the Operation-Location header
            if (!response.Headers.TryGetValues("Operation-Location", out IEnumerable<string> operationLocationValues))
            {
                throw new Exception($"ComputerVisionReadV3 - Read request failed - No 'operation-location' provided");
            }
            
            var operationLocation = operationLocationValues.First();
            var operationId = operationLocation.Substring(operationLocation.Length - _computerVisionOperationIdLength);
            var readResultBuilder = new UriBuilder($"{_endpoint}{resultOperationEndpoint}{operationId}");
            var readResultUrl = readResultBuilder.ToString();
            
            var result = await GetReadOperationResultAsync(readResultUrl, httpClient);

            // Wait for the operation to complete
            int i = 1;
            int waitDurationInMs = 500;
            var maxWaitTimeInMs = 30000;
            int maxTries = maxWaitTimeInMs / waitDurationInMs;

            while ((result.Status == ComputerVisionApiClient.ReadOperationStatus.Running || result.Status == ComputerVisionApiClient.ReadOperationStatus.NotStarted)
                && i <= maxTries)
            {
                Console.WriteLine($"ComputerVisionReadV3 - Server status: {0}, try {1}, waiting {2} milliseconds...", result.Status, i, waitDurationInMs);
                await Task.Delay(waitDurationInMs);

                result = await GetReadOperationResultAsync(readResultUrl, httpClient);
                i++;
            }

            return result;
        }

        private async Task<ComputerVisionApiClient.ReadOperationResult> GetReadOperationResultAsync(string url, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"ComputerVisionReadV3 - Read request failed - Reponse code: {response.StatusCode} - Response message: '{await response.Content.ReadAsStringAsync()}'");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ComputerVisionApiClient.ReadOperationResult>(responseContent);

            return result;
        }
    
        private async Task<OcrResult> ComputerVisionRecognizedPrintedText(string url, OcrLanguages ocrLanguage)
        {
            return await _computerVisionClient.RecognizePrintedTextAsync(true, url, ocrLanguage);
        }
    }
}
