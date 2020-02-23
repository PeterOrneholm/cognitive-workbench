using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Orneholm.CognitiveWorkbench.Web.Models;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public class ImageFaceAnalyzer
    {
        private static readonly List<FaceAttributeType> FaceAttributes = new List<FaceAttributeType>
        {
            FaceAttributeType.Age,
            FaceAttributeType.Gender,
            FaceAttributeType.HeadPose,
            FaceAttributeType.Smile,
            FaceAttributeType.FacialHair,
            FaceAttributeType.Glasses,
            FaceAttributeType.Emotion,
            FaceAttributeType.Hair,
            FaceAttributeType.Makeup,
            FaceAttributeType.Occlusion,
            FaceAttributeType.Accessories,
            FaceAttributeType.Blur,
            FaceAttributeType.Exposure,
            FaceAttributeType.Noise
        };

        private readonly FaceClient _faceClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImageFaceAnalyzer(string faceSubscriptionKey, string faceEndpoint, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(faceSubscriptionKey))
            {
                Endpoint = faceEndpoint
            };
        }

        public async Task<FaceAnalyzeResponse> Analyze(string url)
        {
            // Face
            var face = FaceDetect(url);
            var imageInfo = GetImageInfo(url);

            // Combine
            await Task.WhenAll(face, imageInfo);

            return new FaceAnalyzeResponse
            {
                ImageInfo = imageInfo.Result,

                FaceResult = face.Result.ToList()
            };
        }

        private Task<IList<DetectedFace>> FaceDetect(string url)
        {
            return _faceClient.Face.DetectWithUrlAsync(
                url,
                false,
                true,
                FaceAttributes,
                null,
                true
            );
        }

        private async Task<ImageInfo> GetImageInfo(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var stream = await httpClient.GetStreamAsync(url);
            using var image = Image.FromStream(stream);

            return new ImageInfo
            {
                Url = url,

                Width = image.Width,
                Height = image.Height,

                Description = $"Image from {url}"
            };
        }
    }
}
