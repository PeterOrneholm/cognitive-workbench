using System.ComponentModel.DataAnnotations;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class FaceAnalyzeRequest
    {
        public string FaceSubscriptionKey { get; set; } = string.Empty;
        public string FaceEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public FaceDetectionModel DetectionModel { get; set; } = FaceDetectionModel.detection_01;
        public bool EnableIdentification { get; set; } = false;
        public FaceRecognitionModel RecognitionModel { get; set; } = FaceRecognitionModel.recognition_02;
        public FaceIdentificationGroupType IdentificationGroupType { get; set; } = FaceIdentificationGroupType.PersonGroup;
        public string IdentificationGroupId { get; set; } = string.Empty;
    }

    public enum FaceDetectionModel
    {
        [Display(Name = "Detection 01 - Default")]
        detection_01,
        [Display(Name = "Detection 02 - Improved accuracy especially on small, side and blurry faces but no face attributes and landmarks")]
        detection_02
    }

    public enum FaceRecognitionModel
    {
        [Display(Name = "Recognition 02 - Better overall accuracy")]
        recognition_02,
        [Display(Name = "Recognition 01")]
        recognition_01
    }

    public enum FaceIdentificationGroupType
    {
        [Display(Name = "Person group")]
        PersonGroup,
        [Display(Name = "Large person group")]
        LargePersonGroup
    }
}
