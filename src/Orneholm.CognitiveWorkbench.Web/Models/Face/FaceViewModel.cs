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

        public static FaceViewModel Analyzed(FaceAnalyzeRequest faceAnalyzeRequest, FaceAnalyzeResponse faceAnalyzeResponse)
        {
            return new FaceViewModel
            {
                IsAnalyzed = true,

                FaceAnalyzeRequest = faceAnalyzeRequest,
                FaceAnalyzeResponse = faceAnalyzeResponse
            };
        }

        public bool IsAnalyzed { get; internal set; }

        public FaceAnalyzeRequest FaceAnalyzeRequest { get; internal set; } = new FaceAnalyzeRequest();
        public FaceAnalyzeResponse FaceAnalyzeResponse { get; internal set; }
    }
}
