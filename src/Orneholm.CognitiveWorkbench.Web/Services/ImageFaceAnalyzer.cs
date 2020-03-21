using System.Collections.Generic;
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

        public async Task<FaceAnalyzeResponse> Analyze(string url, FaceDetectionModel detectionModel, bool enableIdentification, FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
            // Face
            var face = FaceDetect(url, detectionModel, enableIdentification, identificationGroupType, identificationGroupId);
            var imageInfo = ImageInfoProcessor.GetImageInfo(url, _httpClientFactory);

            // Combine
            await Task.WhenAll(face, imageInfo);

            return new FaceAnalyzeResponse
            {
                ImageInfo = imageInfo.Result,
                FaceResult = face.Result.ToList()
            };
        }

        private Task<IList<DetectedFace>> FaceDetect(string url, FaceDetectionModel detectionModel, bool enableIdentification, FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
            var returnFaceLandmarks = true;
            var returnFaceAttributes = FaceAttributes;

            if (FaceDetectionModel.detection_02.Equals(detectionModel))
            {
                returnFaceLandmarks = false;
                returnFaceAttributes = new List<FaceAttributeType>();
            }

            return _faceClient.Face.DetectWithUrlAsync(
                url,
                returnFaceId: false,
                returnFaceLandmarks: returnFaceLandmarks,
                returnFaceAttributes: returnFaceAttributes,
                recognitionModel: null,
                returnRecognitionModel: true,
                detectionModel: detectionModel.ToString()
            );
        }
    }
}
