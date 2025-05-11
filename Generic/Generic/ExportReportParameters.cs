

namespace Adam.Generic.Generic
{
    public class ExportReportParameters
    {
        public int ReportType { get; set; }
        public string ReportPath { get; set; }
        public Dictionary<string, object> DataSources { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
