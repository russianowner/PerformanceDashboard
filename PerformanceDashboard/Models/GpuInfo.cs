namespace PerformanceDashboard.Models
{
    public class GpuInfo
    {
        public string? Name { get; set; }
        public float UsagePercent { get; set; }
        public float MemoryUsedMB { get; set; }
        public float MemoryTotalMB { get; set; }
        public float TemperatureC { get; set; }
    }
}
