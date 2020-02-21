using Microsoft.AspNetCore.Http.Connections;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class VisionIndexViewModel
    {
        public static VisionIndexViewModel NotAnalyzed()
        {
            return new VisionIndexViewModel
            {
                IsAnalyzed = false,
                VisionAnalyzeResponse = null
            };
        }

        public static VisionIndexViewModel Analyzed(VisionAnalyzeResponse visionAnalyzeResponse)
        {
            return new VisionIndexViewModel
            {
                IsAnalyzed = true,
                VisionAnalyzeResponse = visionAnalyzeResponse
            };
        }

        public bool IsAnalyzed { get; internal set; }
        public VisionAnalyzeResponse? VisionAnalyzeResponse { get; internal set; }
    }
}
