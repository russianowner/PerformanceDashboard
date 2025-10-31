namespace PerformanceDashboard.Models
{
    public class MemoryInfo
    {
        public float TotalMB { get; set; }
        public float UsedMB { get; set; }
        public float FreeMB => TotalMB - UsedMB;
        public float UsagePercent => (UsedMB / TotalMB) * 100;
    }
}
