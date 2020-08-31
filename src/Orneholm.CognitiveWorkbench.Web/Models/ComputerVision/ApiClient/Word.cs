using System.Collections.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient
{
    public class Word
    {
        public IList<int> BoundingBox { get; set; }

        public string Text { get; set; }

        public double Confidence { get; set; }
    }
}