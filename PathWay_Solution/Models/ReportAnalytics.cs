namespace PathWay_Solution.Models
{
    public class ReportAnalytics
    {
        public int ReportAnalyticsId { get; set; }
        public string ReportType { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }
    }

}
