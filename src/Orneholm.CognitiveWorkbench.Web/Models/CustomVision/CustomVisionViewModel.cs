using Microsoft.AspNetCore.Http.Connections;

namespace Orneholm.CognitiveWorkbench.Web.Models.CustomVision
{
    public class CustomVisionViewModel
    {
        public static CustomVisionViewModel NotAnalyzed()
        {
            return new CustomVisionViewModel
            {
                IsAnalyzed = false,

                CustomVisionAnalyzeRequest = new CustomVisionRequest(),
                CustomVisionAnalyzeResponse = null
            };
        }

        public static CustomVisionViewModel Analyzed(CustomVisionRequest customVisionAnalyzeRequest, CustomVisionResponse customVisionAnalyzeResponse)
        {
            return new CustomVisionViewModel
            {
                IsAnalyzed = true,

                CustomVisionAnalyzeRequest = customVisionAnalyzeRequest,
                CustomVisionAnalyzeResponse = customVisionAnalyzeResponse
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public CustomVisionRequest CustomVisionAnalyzeRequest { get; internal set; } = new CustomVisionRequest();
        public CustomVisionResponse CustomVisionAnalyzeResponse { get; internal set; }
    }
}
