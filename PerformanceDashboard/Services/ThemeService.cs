using System.Text.Json;
using PerformanceDashboard.Models;

namespace PerformanceDashboard.Services
{
    public static class ThemeService
    {
        private static Theme? _currentTheme;
        public static Theme LoadTheme(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"{filePath}");
            string json = File.ReadAllText(filePath);
            var theme = JsonSerializer.Deserialize<Theme>(json);
            if (theme == null)
                throw new Exception($"{filePath}");
            _currentTheme = theme;
            return theme;
        }
        public static Theme? CurrentTheme => _currentTheme;
    }
}
