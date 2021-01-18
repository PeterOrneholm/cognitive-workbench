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

        public static ComputerVisionViewModel Analyzed(ComputerVisionAnalyzeRequest request, ComputerVisionAnalyzeResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.ApiRequestErrorMessage)
                || !string.IsNullOrWhiteSpace(response.ApiRequestErrorContent)
                || !string.IsNullOrWhiteSpace(response.OtherErrorMessage)
                || !string.IsNullOrWhiteSpace(response.OtherErrorContent))
            {
                return new ComputerVisionViewModel
                {
                    IsAnalyzed = false,
                    
                    ComputerVisionAnalyzeRequest = request,
                    ComputerVisionAnalyzeResponse = null,
                    
                    ApiRequestErrorMessage = response.ApiRequestErrorMessage,
                    ApiRequestErrorContent = response.ApiRequestErrorContent,
                    OtherErrorMessage = response.OtherErrorMessage,
                    OtherErrorContent = response.OtherErrorContent
                };
            }

            return new ComputerVisionViewModel
            {
                IsAnalyzed = true,

                ComputerVisionAnalyzeRequest = request,
                ComputerVisionAnalyzeResponse = response
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public ComputerVisionAnalyzeRequest ComputerVisionAnalyzeRequest { get; internal set; } = new ComputerVisionAnalyzeRequest();
        public ComputerVisionAnalyzeResponse ComputerVisionAnalyzeResponse { get; internal set; }

        public string ApiRequestErrorMessage { get; set; }
        public string ApiRequestErrorContent { get; set; }
        public string OtherErrorMessage { get; set; }
        public string OtherErrorContent { get; set; }
    }
}
