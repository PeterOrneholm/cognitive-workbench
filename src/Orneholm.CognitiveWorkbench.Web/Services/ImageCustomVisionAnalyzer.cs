using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
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
            await Task.WhenAll(imageAnalysis, imageInfo);

            return new CustomVisionResponse
            {
                ImageInfo = imageInfo.Result,
                Predictions = imageAnalysis.Result.Predictions.ToList()
            };
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
