using PerformanceDashboard.Models;

namespace PerformanceDashboard.Services
{
    public static class ThemeManager
    {
        public static Theme? CurrentTheme { get; private set; }

        public static event Action<Theme>? ThemeChanged;

        public static void ApplyTheme(Theme theme)
        {
            CurrentTheme = theme;
            ThemeChanged?.Invoke(theme);
        }
        public static void InitDefaultTheme()
        {
            try
            {
                var theme = ThemeService.LoadTheme("Resources/themes/darkTheme.json");
                ApplyTheme(theme);
            }
            catch
            {
                CurrentTheme = new Theme
                {
                    BackgroundColor = "#282828",
                    PanelColor = "#3C3C3C",
                    TextColor = "#FFFFFF",
                    AccentColor = "#00BFFF"
                };
            }
        }
    }
}
