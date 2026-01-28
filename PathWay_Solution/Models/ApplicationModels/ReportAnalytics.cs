using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class ReportAnalytics
    {
        [Key]
        public int ReportAnalyticsId { get; set; }
        public string ReportType { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }
    }

}
