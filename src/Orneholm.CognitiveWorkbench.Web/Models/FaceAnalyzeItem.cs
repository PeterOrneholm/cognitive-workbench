using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Orneholm.CognitiveWorkbench.Web.Models
{
    public class FaceAnalyzeItem
    {
        public DetectedFace DetectedFace { get; set; }

        public List<IdentificationCandidate> IdentificationCandidates { get; set; }
    }

    public class IdentificationCandidate
    {
        public IdentifyCandidate IdentifyCandidate { get; set; }

        public Person Person { get; set; }
    }
}
