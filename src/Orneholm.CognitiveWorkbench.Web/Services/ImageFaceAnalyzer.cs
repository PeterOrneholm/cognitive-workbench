using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Vision.Face.ApiKeyServiceClientCredentials;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orneholm.CognitiveWorkbench.Web.Models.Face;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public class ImageFaceAnalyzer
    {
        private static readonly List<FaceAttributeType?> FaceAttributes = new List<FaceAttributeType?>
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

        public async Task<FaceAnalyzeResponse> AnalyzeAsync(string imageUrl, IFormFile file, 
            FaceDetectionModel detectionModel, bool enableIdentification, FaceRecognitionModel recognitionModel, 
            FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
            // Face
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                var face = FaceProcessByUrlAsync(imageUrl, detectionModel, enableIdentification, recognitionModel, identificationGroupType, identificationGroupId);
                var imageInfo = ImageInfoProcessor.GetImageInfoFromImageUrlAsync(imageUrl, _httpClientFactory);

                // Combine
                var task = Task.WhenAll(face, imageInfo);
                try
                {
                    await task;
                    return new FaceAnalyzeResponse
                    {
                        ImageInfo = imageInfo.Result,
                        FaceResult = face.Result
                    };
                }
                catch (APIErrorException ex)
                {
                    var exceptionMessage = ex.Response.Content;
                    var parsedJson = JToken.Parse(exceptionMessage);

                    if (ex.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        return new FaceAnalyzeResponse
                        {
                            ApiRequestErrorMessage = $"Bad request thrown by the underlying API from Microsoft:",
                            ApiRequestErrorContent = parsedJson.ToString(Formatting.Indented)
                        };
                    }
                    else
                    {
                        return new FaceAnalyzeResponse
                        {
                            OtherErrorMessage = $"Error thrown by the underlying API from Microsoft:",
                            OtherErrorContent = parsedJson.ToString(Formatting.Indented)
                        };
                    }
                }
            }
            else
            {
                using (var analyzeStream = new MemoryStream())
                using (var outputStream = new MemoryStream())
                {
                    // Get initial value
                    await file.CopyToAsync(analyzeStream);

                    analyzeStream.Seek(0, SeekOrigin.Begin);
                    await analyzeStream.CopyToAsync(outputStream);

                    // Reset the stream for consumption
                    analyzeStream.Seek(0, SeekOrigin.Begin);
                    outputStream.Seek(0, SeekOrigin.Begin);
                    
                    try
                    {
                        var face = await FaceProcessByStreamAsync(analyzeStream, detectionModel, enableIdentification, recognitionModel, identificationGroupType, identificationGroupId);
                        var imageInfo = ImageInfoProcessor.GetImageInfoFromStream(outputStream, file.ContentType);

                        return new FaceAnalyzeResponse
                        {
                            ImageInfo = imageInfo,
                            FaceResult = face
                        };
                    }
                    catch (APIErrorException ex)
                    {
                        var exceptionMessage = ex.Response.Content;
                        var parsedJson = JToken.Parse(exceptionMessage);

                        if (ex.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            return new FaceAnalyzeResponse
                            {
                                ApiRequestErrorMessage = $"Bad request thrown by the underlying API from Microsoft:",
                                ApiRequestErrorContent = parsedJson.ToString(Formatting.Indented)
                            };
                        }
                        else
                        {
                            return new FaceAnalyzeResponse
                            {
                                OtherErrorMessage = $"Error thrown by the underlying API from Microsoft:",
                                OtherErrorContent = parsedJson.ToString(Formatting.Indented)
                            };
                        }
                    }
                }
            }
        }

        private async Task<List<FaceAnalyzeItem>> FaceProcessByUrlAsync(string imageUrl, FaceDetectionModel detectionModel, bool enableIdentification, FaceRecognitionModel recognitionModel, FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
            var returnFaceId = false;
            var returnFaceLandmarks = true;
            var returnFaceAttributes = FaceAttributes;
            var targetRecognitionModel = FaceRecognitionModel.recognition_01;
           
            if (FaceDetectionModel.detection_02.Equals(detectionModel))
            {
                returnFaceLandmarks = false;
                returnFaceAttributes = new List<FaceAttributeType?>();
            }

            if (enableIdentification)
            {
                returnFaceId = true;
                targetRecognitionModel = recognitionModel;
            }

            var detectedFaces = await _faceClient.Face.DetectWithUrlAsync(
                imageUrl,
                returnFaceId: returnFaceId,
                returnFaceLandmarks: returnFaceLandmarks,
                returnFaceAttributes: returnFaceAttributes,
                recognitionModel: targetRecognitionModel.ToString(),
                returnRecognitionModel: true,
                detectionModel: detectionModel.ToString()
            );

            // Process detected faces
            return await DetectedFacesProcessAsync(detectedFaces, detectionModel, enableIdentification, recognitionModel, identificationGroupType, identificationGroupId);
        }

        private async Task<List<FaceAnalyzeItem>> FaceProcessByStreamAsync(Stream imageStream, FaceDetectionModel detectionModel, bool enableIdentification, FaceRecognitionModel recognitionModel, FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
            var returnFaceId = false;
            var returnFaceLandmarks = true;
            var returnFaceAttributes = FaceAttributes;
            var targetRecognitionModel = FaceRecognitionModel.recognition_01;
           
            if (FaceDetectionModel.detection_02.Equals(detectionModel))
            {
                returnFaceLandmarks = false;
                returnFaceAttributes = new List<FaceAttributeType?>();
            }

            if (enableIdentification)
            {
                returnFaceId = true;
                targetRecognitionModel = recognitionModel;
            }

            var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(
                imageStream,
                returnFaceId: returnFaceId,
                returnFaceLandmarks: returnFaceLandmarks,
                returnFaceAttributes: returnFaceAttributes,
                recognitionModel: targetRecognitionModel.ToString(),
                returnRecognitionModel: true,
                detectionModel: detectionModel.ToString()
            );
        
            // Process detected faces
            return await DetectedFacesProcessAsync(detectedFaces, detectionModel, enableIdentification, recognitionModel, identificationGroupType, identificationGroupId);
        }

        private async Task<List<FaceAnalyzeItem>> DetectedFacesProcessAsync(IList<DetectedFace> detectedFaces, FaceDetectionModel detectionModel, bool enableIdentification, FaceRecognitionModel recognitionModel, FaceIdentificationGroupType identificationGroupType, string identificationGroupId)
        {
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

        private async Task<IEnumerable<FaceAnalyzeItem>> FaceIdentifyAsync(IEnumerable<DetectedFace> detectedFaces, FaceIdentificationGroupType identificationGroupType, string identificationGroupId, IList<Person> identificationGroupPersons)
        {
            // Call Identification operation
            var identificationResults = FaceIdentificationGroupType.PersonGroup.Equals(identificationGroupType)
                ? await _faceClient.Face.IdentifyAsync(detectedFaces.Select(d => d.FaceId).ToList(),
                    personGroupId: identificationGroupId,
                    largePersonGroupId: null)
                : await _faceClient.Face.IdentifyAsync(detectedFaces.Select(d => d.FaceId).ToList(),
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
                                PersonName = identificationGroupPersons.SingleOrDefault(p => p.PersonId == c.PersonId)?.Name
                            }).ToList()
            });
        }
    }
}
