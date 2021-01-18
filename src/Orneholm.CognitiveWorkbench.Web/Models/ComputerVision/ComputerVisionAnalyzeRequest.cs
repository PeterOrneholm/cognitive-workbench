using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision
{
    public class ComputerVisionAnalyzeRequest
    {
        public string ComputerVisionSubscriptionKey { get; set; } = string.Empty;
        public string ComputerVisionEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public IFormFile File { get; set; }
        public AnalysisLanguage ImageAnalysisLanguage { get; set; } = AnalysisLanguage.en;
        public ReadLanguage ImageReadLanguage { get; set; } = ReadLanguage.en;
        public OcrLanguages ImageOcrLanguage { get; set; } = OcrLanguages.Unk;
    }

    public enum AnalysisLanguage
    {
        [Display(Name = "English")]
        en,
        [Display(Name = "Spanish")]
        es,
        [Display(Name = "Japanese")]
        ja,
        [Display(Name = "Portuguese")]
        pt,
        [Display(Name = "Simplified Chinese")]
        zh
    }

    public enum ReadLanguage
    {
        [Display(Name = "English")]
        en,
        [Display(Name = "Spanish")]
        es,
        [Display(Name = "French")]
        fr,
        [Display(Name = "German")]
        de,
        [Display(Name = "Italian")]
        it,
        [Display(Name = "Dutch")]
        nl,
        [Display(Name = "Portuguese")]
        pt
    }
}
