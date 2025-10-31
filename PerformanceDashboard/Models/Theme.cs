namespace PerformanceDashboard.Models
{
    public class Theme
    {
        public string? Name { get; set; }
        public string? BackgroundColor { get; set; }
        public string? PanelColor { get; set; }
        public string? TextColor { get; set; }
        public string? AccentColor { get; set; }
        public string? ChartBackground { get; set; }
        public string? ChartLineColor { get; set; }
        public string? GoodColor { get; set; }
        public string? WarningColor { get; set; }
        public string? CriticalColor { get; set; }
        public Color ToColor(string hex) => ColorTranslator.FromHtml(hex);
    }
}
