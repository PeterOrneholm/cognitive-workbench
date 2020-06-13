using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision
{
    public class ComputerVisionAnalyzeRequest
    {
        public string ComputerVisionSubscriptionKey { get; set; } = string.Empty;
        public string ComputerVisionEndpoint { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public AnalysisLanguage ImageAnalysisLanguage { get; set; } = AnalysisLanguage.en;
        public OcrLanguages ImageOcrLanguage { get; set; } = OcrLanguages.Unk;
        public TextRecognitionMode ImageRecognizeTextMode { get; set; } = TextRecognitionMode.Printed;
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
