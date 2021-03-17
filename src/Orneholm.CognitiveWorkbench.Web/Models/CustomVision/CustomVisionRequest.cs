using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Orneholm.CognitiveWorkbench.Web.Models.CustomVision
{
    public class CustomVisionRequest
    {
        public string CustomVisionPredictionKey { get; set; } = string.Empty;
        public string CustomVisionEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public IFormFile File { get; set; }
        public Guid ProjectId { get; set; } = Guid.Empty;
        public string IterationPublishedName { get; set; } = string.Empty;
        public CustomVisionProjectType ProjectType { get; set; } = CustomVisionProjectType.Classification;
    }

    public enum CustomVisionProjectType
    {
        [Display(Name = "Classification")]
        Classification,
        [Display(Name = "Object detection")]
        Object_Detection
    }
}
