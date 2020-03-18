using Microsoft.AspNetCore.Http.Connections;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class CustomVisionViewModel
    {
        public static CustomVisionViewModel NotAnalyzed()
        {
            return new CustomVisionViewModel
            {
                IsAnalyzed = false,

                CustomVisionAnalyzeRequest = new CustomVisionAnalyzeRequest(),
                CustomVisionAnalyzeResponse = null
            };
        }

        public static CustomVisionViewModel Analyzed(CustomVisionAnalyzeRequest customVisionAnalyzeRequest, CustomVisionAnalyzeResponse customVisionAnalyzeResponse)
        {
            return new CustomVisionViewModel
            {
                IsAnalyzed = true,

                CustomVisionAnalyzeRequest = customVisionAnalyzeRequest,
                CustomVisionAnalyzeResponse = customVisionAnalyzeResponse
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public CustomVisionAnalyzeRequest CustomVisionAnalyzeRequest { get; internal set; } = new CustomVisionAnalyzeRequest();
        public CustomVisionAnalyzeResponse CustomVisionAnalyzeResponse { get; internal set; }
    }
}
