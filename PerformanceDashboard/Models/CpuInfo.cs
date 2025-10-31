namespace PerformanceDashboard.Models
{
    public class CpuInfo
    {
        public float UsagePercent { get; set; }
        public int CoreCount { get; set; }
        public int ThreadCount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
