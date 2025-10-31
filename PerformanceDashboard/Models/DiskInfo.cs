public class DiskInfo
{
    public string? DriveName { get; set; }
    public float TotalGB { get; set; }
    public float UsedGB { get; set; }
    public float FreeGB => TotalGB - UsedGB;
    public float UsagePercent => (UsedGB / TotalGB) * 100;
    public float ActivityPercent { get; set; } 
}
