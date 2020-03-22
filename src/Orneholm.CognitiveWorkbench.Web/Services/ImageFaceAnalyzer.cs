using System;
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

        public async Task<FaceAnalyzeResponse> Analyze(string url, FaceDetectionModel detectionModel, bool enableIdentification, FaceRecognitionModel recognitionModel, FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
            // Face
            var face = FaceProcess(url, detectionModel, enableIdentification, recognitionModel, identificationGroupType, identificationGroupId);
            var imageInfo = ImageInfoProcessor.GetImageInfo(url, _httpClientFactory);

            // Combine
            await Task.WhenAll(face, imageInfo);

            return new FaceAnalyzeResponse
            {
                ImageInfo = imageInfo.Result,
                FaceResult = face.Result
            };
        }

        private async Task<List<FaceAnalyzeItem>> FaceProcess(string url, FaceDetectionModel detectionModel, bool enableIdentification, FaceRecognitionModel recognitionModel, FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
            // Detect faces
            var detectedFaces = await FaceDetectAsync(url, detectionModel, enableIdentification, recognitionModel);
            var unidentifiedResults = detectedFaces.Select(f => new FaceAnalyzeItem { DetectedFace = f }).ToList();

            if (!enableIdentification || detectedFaces.Count == 0)
            {
                return unidentifiedResults;
            }

            // Check identification group training status and getting person list for name consolidation
            TrainingStatus identificationGroupTrainingStatus;
            IList<Person> identificationGroupPersons;

            if (FaceIdentificationGroupType.PersonGroup.Equals(identificationGroupType))
            {
                try
                {
                    identificationGroupTrainingStatus = await _faceClient.PersonGroup.GetTrainingStatusAsync(identificationGroupId);
                }
                catch (APIErrorException ex)
                {
                    // If group has never been trained, a 404 is thrown by the API
                    if ("PersonGroupNotTrained".Equals(ex.Body.Error.Code))
                    {
                        // TO DO - Add error info in reply
                        return unidentifiedResults;
                    }
                    else
                    {
                        throw;
                    }
                }

                identificationGroupPersons = await _faceClient.PersonGroupPerson.ListAsync(identificationGroupId);
            }
            else
            {
                try
                {
                    identificationGroupTrainingStatus = await _faceClient.LargePersonGroup.GetTrainingStatusAsync(identificationGroupId);
                }
                catch (APIErrorException ex)
                {
                    // If group has never been trained, a 404 is thrown by the API
                    if ("PersonGroupNotTrained".Equals(ex.Body.Error.Code))
                    {
                        // TO DO - Add error info in reply
                        return unidentifiedResults;
                    }
                    else
                    {
                        throw;
                    }
                }

                identificationGroupPersons = await _faceClient.LargePersonGroupPerson.ListAsync(identificationGroupId);
            }

            if (!TrainingStatusType.Succeeded.Equals(identificationGroupTrainingStatus.Status)
                || identificationGroupPersons == null || identificationGroupPersons.Count == 0)
            {
                // TO DO - Add error info in reply
                return unidentifiedResults;
            }

            // Identification request: max 10 faces / request (API limitation)
            var batchSize = 10;
            var batchCount = (int)Math.Ceiling((double)detectedFaces.Count / batchSize);
            var identificationTasks = new Task<IEnumerable<FaceAnalyzeItem>>[batchCount];

            for (var i = 0; i < batchCount; i++)
            {
                identificationTasks[i] = FaceIdentifyAsync(
                    detectedFaces.Skip(i * batchSize).Take(batchSize),
                    identificationGroupType,
                    identificationGroupId,
                    identificationGroupPersons
                    );
            }

            var identificationResults = await Task.WhenAll(identificationTasks.ToArray());
            var identifiedFaces = identificationResults.SelectMany(i => i).ToList();

            return identifiedFaces;
        }

        private async Task<IList<DetectedFace>> FaceDetectAsync(string url, FaceDetectionModel detectionModel, bool enableIdentification, FaceRecognitionModel recognitionModel)
        {
            var returnFaceId = false;
            var returnFaceLandmarks = true;
            var returnFaceAttributes = FaceAttributes;
            var targetRecognitionModel = FaceRecognitionModel.recognition_01;
           
            if (FaceDetectionModel.detection_02.Equals(detectionModel))
            {
                returnFaceLandmarks = false;
                returnFaceAttributes = new List<FaceAttributeType>();
            }

            if (enableIdentification)
            {
                returnFaceId = true;
                targetRecognitionModel = recognitionModel;
            }

            return await _faceClient.Face.DetectWithUrlAsync(
                url,
                returnFaceId: returnFaceId,
                returnFaceLandmarks: returnFaceLandmarks,
                returnFaceAttributes: returnFaceAttributes,
                recognitionModel: targetRecognitionModel.ToString(),
                returnRecognitionModel: true,
                detectionModel: detectionModel.ToString()
            );
        }

        private async Task<IEnumerable<FaceAnalyzeItem>> FaceIdentifyAsync(IEnumerable<DetectedFace> detectedFaces, FaceIdentificationGroupType identificationGroupType, string identificationGroupId, IList<Person> identificationGroupPersons)
        {
            // Call Identification operation
            var identificationResults = FaceIdentificationGroupType.PersonGroup.Equals(identificationGroupType)
                ? await _faceClient.Face.IdentifyAsync(detectedFaces.Select(d => d.FaceId.Value).ToList(),
                    personGroupId: identificationGroupId,
                    largePersonGroupId: null)
                : await _faceClient.Face.IdentifyAsync(detectedFaces.Select(d => d.FaceId.Value).ToList(),
                    personGroupId: null,
                    largePersonGroupId: identificationGroupId);

            // Consolidate identifications with info from identification group
            return detectedFaces.Select(f => new FaceAnalyzeItem()
            {
                DetectedFace = f,
                IdentificationCandidates =
                    identificationResults?
                        .SingleOrDefault(r => r.FaceId == f.FaceId.Value)?
                        .Candidates?
                        .Select(c =>
                            new IdentificationCandidate
                            {
                                IdentifyCandidate = c,
                                Person = identificationGroupPersons.SingleOrDefault(p => p.PersonId == c.PersonId)
                            }).ToList()
            });
        }
    }
}
