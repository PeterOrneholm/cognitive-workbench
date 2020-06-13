using System.Collections.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient
{
    public class ReadResult
    {
        public int Page { get; set; }

        public string Language { get; set; }

        public double Angle { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Unit { get; set; }

        public IList<Line> Lines { get; set; }
    }
}
