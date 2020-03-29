using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class FaceAnalyzeResponse
    {
        public ImageInfo ImageInfo { get; set; } = new ImageInfo();

        public List<FaceAttributeType> FaceAttributes { get; set; } = new List<FaceAttributeType>();

        public List<FaceAnalyzeItem> FaceResult { get; set; } = new List<FaceAnalyzeItem>();
    }
}
