using System.Collections.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient
{
    public class AnalyzeResult
    {
        public string Version { get; set; }

        public IList<ReadResult> ReadResults { get; set; }
    }
}
