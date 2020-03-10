using System.ComponentModel.DataAnnotations;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class FaceAnalyzeRequest
    {
        public string FaceSubscriptionKey { get; set; } = string.Empty;
        public string FaceEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public FaceDetectionModel DetectionModel { get; set; } = FaceDetectionModel.detection_01;
    }

    public enum FaceDetectionModel
    {
        [Display(Name = "Detection 01 - Default")]
        detection_01,
        [Display(Name = "Detection 02 - Improved accuracy especially on small, side and blurry faces but no face attributes and landmarks")]
        detection_02
    }
}
