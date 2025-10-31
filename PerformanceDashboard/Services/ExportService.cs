using System.Text;
using PerformanceDashboard.Models;

namespace PerformanceDashboard.Services
{
    public static class ExportService
    {
        public static void ExportToCsv(string filePath,
            CpuInfo cpu, MemoryInfo mem,
            List<DiskInfo> disks, List<NetworkInfo> nets,
            GpuInfo? gpu = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine("=== CPU ===");
            sb.AppendLine("UsagePercent,Cores,Threads,Timestamp");
            sb.AppendLine($"{cpu.UsagePercent:F2},{cpu.CoreCount},{cpu.ThreadCount},{cpu.Timestamp:O}");

            sb.AppendLine();
            sb.AppendLine("=== ОЗУ ===");
            sb.AppendLine("TotalMB,UsedMB,UsagePercent");
            sb.AppendLine($"{mem.TotalMB:F2},{mem.UsedMB:F2},{mem.UsagePercent:F2}");

            sb.AppendLine();
            sb.AppendLine("=== ДИСК ===");
            sb.AppendLine("Drive,TotalGB,UsedGB,UsagePercent");
            if (disks != null && disks.Count > 0)
            {
                foreach (var d in disks)
                {
                    sb.AppendLine($"{EscapeCsv(d.DriveName)},{d.TotalGB:F2},{d.UsedGB:F2},{d.UsagePercent:F2}");
                }
            }
            else
            {
                sb.AppendLine("Диск не найден");
            }
            sb.AppendLine();
            sb.AppendLine("=== Интернет ===");
            sb.AppendLine("AdapterName,DownloadKBps,UploadKBps");
            if (nets != null && nets.Count > 0)
            {
                foreach (var n in nets)
                    sb.AppendLine($"{EscapeCsv(n.AdapterName)},{n.DownloadKBps:F2},{n.UploadKBps:F2}");
            }
            else
            {
                sb.AppendLine("Нету интернет адаптера");
            }
            sb.AppendLine();
            sb.AppendLine("=== GPU ===");
            if (gpu != null)
            {
                sb.AppendLine("Name,UsagePercent,MemoryUsedMB,MemoryTotalMB");
                sb.AppendLine($"{EscapeCsv(gpu.Name)},{gpu.UsagePercent:F2},{gpu.MemoryUsedMB:F2},{gpu.MemoryTotalMB:F2}");
            }
            else
            {
                sb.AppendLine("GPU не найдено");
            }
            try
            {
                var dir = Path.GetDirectoryName(Path.GetFullPath(filePath));
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }
        private static string EscapeCsv(string? s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.Contains(',') || s.Contains('"') || s.Contains('\n'))
            {
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            }
            return s;
        }
    }
}
