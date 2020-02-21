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

                VisionAnalyzeRequest = new VisionAnalyzeRequest(),
                VisionAnalyzeResponse = null
            };
        }

        public static VisionIndexViewModel Analyzed(VisionAnalyzeRequest visionAnalyzeRequest, VisionAnalyzeResponse visionAnalyzeResponse)
        {
            return new VisionIndexViewModel
            {
                IsAnalyzed = true,

                VisionAnalyzeRequest = visionAnalyzeRequest,
                VisionAnalyzeResponse = visionAnalyzeResponse
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public VisionAnalyzeRequest VisionAnalyzeRequest { get; internal set; } = new VisionAnalyzeRequest();
        public VisionAnalyzeResponse VisionAnalyzeResponse { get; internal set; }
    }
}
