namespace PerformanceDashboard.Services
{
    public static class IconService
    {
        private static readonly string basePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icons");
        public static Image? GetIcon(string name)
        {
            try
            {
                string path = Path.Combine(basePath, name);
                return File.Exists(path) ? Image.FromFile(path) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
