using System.ComponentModel.DataAnnotations;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient
{
    public enum ReadV3Language
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