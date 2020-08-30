using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision
{
    public class ComputerVisionAnalyzeRequest
    {
        public string ComputerVisionSubscriptionKey { get; set; } = string.Empty;
        public string ComputerVisionEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public AnalysisLanguage ImageAnalysisLanguage { get; set; } = AnalysisLanguage.en;
        public ReadV3Language ImageReadV3Language { get; set; } = ReadV3Language.en;
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
}
