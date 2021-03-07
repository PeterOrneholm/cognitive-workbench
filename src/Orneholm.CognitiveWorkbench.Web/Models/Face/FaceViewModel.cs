namespace Orneholm.CognitiveWorkbench.Web.Models.Face
{
    public class FaceViewModel
    {
        public static FaceViewModel NotAnalyzed()
        {
            return new FaceViewModel
            {
                IsAnalyzed = false,

                FaceAnalyzeRequest = new FaceAnalyzeRequest(),
                FaceAnalyzeResponse = null
            };
        }

        public static FaceViewModel Analyzed(FaceAnalyzeRequest request, FaceAnalyzeResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.ApiRequestErrorMessage)
                || !string.IsNullOrWhiteSpace(response.ApiRequestErrorContent)
                || !string.IsNullOrWhiteSpace(response.OtherErrorMessage)
                || !string.IsNullOrWhiteSpace(response.OtherErrorContent))
            {
                return new FaceViewModel
                {
                    IsAnalyzed = false,
                    
                    FaceAnalyzeRequest = request,
                    FaceAnalyzeResponse = null,
                    
                    ApiRequestErrorMessage = response.ApiRequestErrorMessage,
                    ApiRequestErrorContent = response.ApiRequestErrorContent,
                    OtherErrorMessage = response.OtherErrorMessage,
                    OtherErrorContent = response.OtherErrorContent
                };
            }

            return new FaceViewModel
            {
                IsAnalyzed = true,

                FaceAnalyzeRequest = request,
                FaceAnalyzeResponse = response
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public FaceAnalyzeRequest FaceAnalyzeRequest { get; internal set; } = new FaceAnalyzeRequest();
        public FaceAnalyzeResponse FaceAnalyzeResponse { get; internal set; }

        public string ApiRequestErrorMessage { get; set; }
        public string ApiRequestErrorContent { get; set; }
        public string OtherErrorMessage { get; set; }
        public string OtherErrorContent { get; set; }
    }
}
