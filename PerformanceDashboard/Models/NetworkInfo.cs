namespace PerformanceDashboard.Models
{
    public class NetworkInfo
    {
        public float UploadKBps { get; set; }
        public float DownloadKBps { get; set; }
        public string? AdapterName { get; set; }
    }
}
