using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orneholm.CognitiveWorkbench.Web.Models.CustomVision;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public class ImageCustomVisionAnalyzer
    {
        private readonly CustomVisionPredictionClient _customVisionPredictionClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImageCustomVisionAnalyzer(string customVisionPredictionKey, string customVisionEndpoint, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _customVisionPredictionClient = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(customVisionPredictionKey))
            {
                Endpoint = customVisionEndpoint
            };
        }

        public async Task<CustomVisionResponse> Analyze(string url, Guid projectId, string iterationPublishedName, CustomVisionProjectType projectType)
        {
            // Custom vision
            var imageAnalysis = CustomVisionAnalyzeImage(url, projectId, iterationPublishedName, projectType);
            var imageInfo = ImageInfoProcessor.GetImageInfo(url, _httpClientFactory);

            // Combine
            var task = Task.WhenAll(imageAnalysis, imageInfo);

            try
            {
                await task;

                return new CustomVisionResponse
                {
                    ImageInfo = imageInfo.Result,
                    Predictions = imageAnalysis.Result.Predictions.ToList()
                };
            }
            catch (CustomVisionErrorException ex)
            {
                var exceptionMessage = ex.Response.Content;
                var parsedJson = JToken.Parse(exceptionMessage);

                if (ex.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return new CustomVisionResponse
                    {
                        ApiRequestErrorMessage = $"Bad request thrown by the underlying API from Microsoft:",
                        ApiRequestErrorContent = parsedJson.ToString(Formatting.Indented)
                    };
                }
                else
                {
                    return new CustomVisionResponse
                    {
                        OtherErrorMessage = $"Error thrown by the underlying API from Microsoft:",
                        OtherErrorContent = parsedJson.ToString(Formatting.Indented)
                    };
                }
            }
        }

        private async Task<ImagePrediction> CustomVisionAnalyzeImage(string url, Guid projectId, string publishedName, CustomVisionProjectType projectType)
        {
            ImagePrediction imagePrediction = null;

            switch (projectType)
            {
                case CustomVisionProjectType.Classification:
                    imagePrediction = await _customVisionPredictionClient.ClassifyImageUrlWithNoStoreAsync(
                        projectId: projectId,
                        publishedName: publishedName,
                        imageUrl: new ImageUrl(url));
                    break;
                case CustomVisionProjectType.Object_Detection:
                    imagePrediction = await _customVisionPredictionClient.DetectImageUrlWithNoStoreAsync(
                        projectId: projectId,
                        publishedName: publishedName,
                        imageUrl: new ImageUrl(url));
                    break;
            }

            return imagePrediction;
        }
    }
}
