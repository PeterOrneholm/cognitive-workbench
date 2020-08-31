using System;
using System.ComponentModel;

namespace Orneholm.CognitiveWorkbench.Web.Models.ComputerVision.ApiClient
{
    public class ReadOperationResult
    {
        public ReadOperationStatus Status { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime LastUpdatedDateTime { get; set; }

        public AnalyzeResult AnalyzeResult { get; set; }
    }

    public enum ReadOperationStatus
    {
        [Description("notStarted")]
        NotStarted = 0,
        [Description("running")]
        Running = 1,
        [Description("failed")]
        Failed = 2,
        [Description("succeeded")]
        Succeeded = 3,
    }
}