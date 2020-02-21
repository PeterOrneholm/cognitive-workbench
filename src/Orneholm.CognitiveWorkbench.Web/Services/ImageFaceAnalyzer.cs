using System.Collections.Generic;
using System.Linq;
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

        public ImageFaceAnalyzer(string faceSubscriptionKey, string faceEndpoint)
        {
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(faceSubscriptionKey))
            {
                Endpoint = faceEndpoint
            };
        }

        public async Task<FaceAnalyzeResponse> Analyze(string url)
        {
            // Face
            var face = await FaceDetect(url);

            // Combine

            return new FaceAnalyzeResponse
            {
                ImageInfo = new ImageInfo
                {
                    Url = url,
                    Description = string.Empty,

                    Width = 1000, // TODO
                    Height = 1000 // TODO
                },

                FaceResult = face.ToList()
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
    }
}
