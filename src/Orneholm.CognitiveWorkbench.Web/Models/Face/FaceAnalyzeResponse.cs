using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Orneholm.CognitiveWorkbench.Web.Models.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.Face
{
    public class FaceAnalyzeResponse
    {
        public ImageInfo ImageInfo { get; set; } = new ImageInfo();

        public List<FaceAttributeType> FaceAttributes { get; set; } = new List<FaceAttributeType>();

        public List<FaceAnalyzeItem> FaceResult { get; set; } = new List<FaceAnalyzeItem>();

        public string ApiRequestErrorMessage { get; set; }
        public string ApiRequestErrorContent { get; set; }
        public string OtherErrorMessage { get; set; }
        public string OtherErrorContent { get; set; }
    }
}
