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

        public static CustomVisionViewModel Analyzed(CustomVisionRequest request, CustomVisionResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.ApiRequestErrorMessage)
                || !string.IsNullOrWhiteSpace(response.ApiRequestErrorContent)
                || !string.IsNullOrWhiteSpace(response.OtherErrorMessage)
                || !string.IsNullOrWhiteSpace(response.OtherErrorContent))
            {
                return new CustomVisionViewModel
                {
                    IsAnalyzed = false,
                    
                    CustomVisionAnalyzeRequest = request,
                    CustomVisionAnalyzeResponse = null,
                    
                    ApiRequestErrorMessage = response.ApiRequestErrorMessage,
                    ApiRequestErrorContent = response.ApiRequestErrorContent,
                    OtherErrorMessage = response.OtherErrorMessage,
                    OtherErrorContent = response.OtherErrorContent
                };
            }

            return new CustomVisionViewModel
            {
                IsAnalyzed = true,

                CustomVisionAnalyzeRequest = request,
                CustomVisionAnalyzeResponse = response
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public CustomVisionRequest CustomVisionAnalyzeRequest { get; internal set; } = new CustomVisionRequest();
        public CustomVisionResponse CustomVisionAnalyzeResponse { get; internal set; }

        public string ApiRequestErrorMessage { get; set; }
        public string ApiRequestErrorContent { get; set; }
        public string OtherErrorMessage { get; set; }
        public string OtherErrorContent { get; set; }
    }
}
