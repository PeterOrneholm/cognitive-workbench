using Microsoft.AspNetCore.Http.Connections;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision
{
    public class ComputerVisionViewModel
    {
        public static ComputerVisionViewModel NotAnalyzed()
        {
            return new ComputerVisionViewModel
            {
                IsAnalyzed = false,

                ComputerVisionAnalyzeRequest = new ComputerVisionAnalyzeRequest(),
                ComputerVisionAnalyzeResponse = null
            };
        }

        public static ComputerVisionViewModel Analyzed(ComputerVisionAnalyzeRequest computerVisionAnalyzeRequest, ComputerVisionAnalyzeResponse computerVisionAnalyzeResponse)
        {
            return new ComputerVisionViewModel
            {
                IsAnalyzed = true,

                ComputerVisionAnalyzeRequest = computerVisionAnalyzeRequest,
                ComputerVisionAnalyzeResponse = computerVisionAnalyzeResponse
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public ComputerVisionAnalyzeRequest ComputerVisionAnalyzeRequest { get; internal set; } = new ComputerVisionAnalyzeRequest();
        public ComputerVisionAnalyzeResponse ComputerVisionAnalyzeResponse { get; internal set; }
    }
}
