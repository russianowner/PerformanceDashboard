namespace PerformanceDashboard.Services
{
    public static class LoggingService
    {
        private static readonly string logDir = "Logs";

        static LoggingService()
        {
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
        }
        public static void Log(string message)
        {
            string file = Path.Combine(logDir, $"{DateTime.Now:yyyy-MM-dd}.log");
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            File.AppendAllText(file, line + Environment.NewLine);
        }
        public static void LogError(Exception ex)
        {
            Log($"{ex.Message}\n{ex.StackTrace}");
        }
    }
}
