using System.Collections.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient
{
    public class Line
    {
        public IList<int> BoundingBox { get; set; }

        public string Text { get; set; }

        public IList<Word> Words { get; set; }

        public string Language { get; set; }
    }
}