using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Orneholm.CognitiveWorkbench.Web.Models;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
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

        private readonly ComputerVisionClient _computerVisionClient;
        private readonly FaceClient _faceClient;

        public ImageAnalyzer(string computerVisionSubscriptionKey, string computerVisionEndpoint, string faceSubscriptionKey, string faceEndpoint)
        {
            _computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(computerVisionSubscriptionKey))
            {
                Endpoint = computerVisionEndpoint
            };

            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(faceSubscriptionKey))
            {
                Endpoint = faceEndpoint
            };
        }

        public async Task<VisionAnalyzeResponse> Analyze(string url, string analysisLanguage, string ocrLanguage)
        {
            // Computer vision
            var imageAnalysis = ComputerVisionAnalyzeImage(url, analysisLanguage);
            var recognizedPrintedText = ComputerVisionRecognizedPrintedText(url, ocrLanguage);
            var areaOfInterest = ComputerVisionGetAreaOfInterest(url);

            // Face
            var face = FaceDetect(url);

            // Combine
            await Task.WhenAll(imageAnalysis, recognizedPrintedText, areaOfInterest, face);

            return new VisionAnalyzeResponse
            {
                ImageUrl = url,

                AnalyzeVisualFeatureTypes = AnalyzeVisualFeatureTypes,
                AnalyzeDetails = AnalyzeDetails,
                FaceAttributes = FaceAttributes,

                AnalysisResult = imageAnalysis.Result,
                OcrResult = recognizedPrintedText.Result,
                AreaOfInterestResult = areaOfInterest.Result,

                FaceResult = face.Result.ToList()
            };
        }

        private Task<AreaOfInterestResult> ComputerVisionGetAreaOfInterest(string url)
        {
            return _computerVisionClient.GetAreaOfInterestAsync(url);
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

        private async Task<OcrResult> ComputerVisionRecognizedPrintedText(string url, string ocrLanguage)
        {
            var parsedOcrLanguage = GetOcrLanguage(ocrLanguage);
            return await _computerVisionClient.RecognizePrintedTextAsync(true, url, parsedOcrLanguage);
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
    }
}
