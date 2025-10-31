#nullable disable
using PerformanceDashboard.Models;
using System.Diagnostics;
using System.Management;
using OpenHardwareMonitor.Hardware;

namespace PerformanceDashboard.Services
{
    public class SystemMonitorService
    {
        private readonly PerformanceCounter cpuCounter;
        private readonly PerformanceCounter ramCounter;
        private readonly List<PerformanceCounter> diskCounters = new();
        private readonly List<PerformanceCounter> netRecvCounters = new();
        private readonly List<PerformanceCounter> netSentCounters = new();

        private readonly Dictionary<string, float> lastNetRecv = new();
        private readonly Dictionary<string, float> lastNetSent = new();
        private DateTime lastNetSampleTime = DateTime.Now;
        private PerformanceCounter gpuUsageCounter;

        public SystemMonitorService()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                string name = drive.Name.Replace("\\", "");
                try
                {
                    diskCounters.Add(new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total"));
                }
                catch { }
            }
            try
            {
                foreach (var nic in new PerformanceCounterCategory("Network Interface").GetInstanceNames())
                {
                    if (nic.ToLower().Contains("loopback") || nic.ToLower().Contains("virtual"))
                        continue;

                    var recv = new PerformanceCounter("Network Interface", "Bytes Received/sec", nic);
                    var sent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", nic);

                    netRecvCounters.Add(recv);
                    netSentCounters.Add(sent);
                    lastNetRecv[nic] = recv.NextValue();
                    lastNetSent[nic] = sent.NextValue();
                }
            }
            catch { }
        }
        public CpuInfo GetCpuInfo()
        {
            return new CpuInfo
            {
                UsagePercent = cpuCounter.NextValue(),
                CoreCount = Environment.ProcessorCount,
                ThreadCount = Process.GetCurrentProcess().Threads.Count
            };
        }
        public MemoryInfo GetMemoryInfo()
        {
            var total = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024f / 1024f;
            var free = ramCounter.NextValue();
            return new MemoryInfo
            {
                TotalMB = total,
                UsedMB = total - free
            };
        }
        public List<DiskInfo> GetDiskInfo()
        {
            var result = new List<DiskInfo>();
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToList();
            for (int i = 0; i < drives.Count; i++)
            {
                var drive = drives[i];
                var used = (drive.TotalSize - drive.TotalFreeSpace) / 1024f / 1024f / 1024f;
                var total = drive.TotalSize / 1024f / 1024f / 1024f;
                float activity = 0;
                try
                {
                    if (i < diskCounters.Count)
                        activity = diskCounters[i].NextValue();
                }
                catch { }
                result.Add(new DiskInfo
                {
                    DriveName = drive.Name,
                    UsedGB = used,
                    TotalGB = total,
                    ActivityPercent = activity
                });
            }
            return result;
        }
        public List<NetworkInfo> GetNetworkInfo()
        {
            var list = new List<NetworkInfo>();
            var now = DateTime.Now;
            var timeDelta = (float)(now - lastNetSampleTime).TotalSeconds;
            lastNetSampleTime = now;
            for (int i = 0; i < netRecvCounters.Count; i++)
            {
                var name = netRecvCounters[i].InstanceName;
                var recv = netRecvCounters[i].NextValue();
                var sent = netSentCounters[i].NextValue();
                float recvRate = (recv - lastNetRecv[name]) / 1024f;
                float sentRate = (sent - lastNetSent[name]) / 1024f;
                lastNetRecv[name] = recv;
                lastNetSent[name] = sent;
                list.Add(new NetworkInfo
                {
                    AdapterName = name,
                    DownloadKBps = Math.Max(0, recvRate),
                    UploadKBps = Math.Max(0, sentRate)
                });
            }
            return list;
        }
        public GpuInfo GetGpuInfo()
        {
            var gpu = new GpuInfo();
            try
            {
                var computer = new OpenHardwareMonitor.Hardware.Computer
                {
                    IsGpuEnabled = true
                };
                computer.Open(false);
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.GpuNvidia ||
                        hardware.HardwareType == HardwareType.GpuAmd ||
                        hardware.HardwareType == HardwareType.GpuIntel)
                    {
                        hardware.Update();
                        gpu.Name = hardware.Name;
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Core"))
                                gpu.UsagePercent = sensor.Value ?? 0;
                            if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("GPU Core"))
                                gpu.TemperatureC = sensor.Value ?? 0;
                            if (sensor.SensorType == SensorType.SmallData && sensor.Name.Contains("Memory Used"))
                                gpu.MemoryUsedMB = sensor.Value ?? 0;
                            if (sensor.SensorType == SensorType.SmallData && sensor.Name.Contains("Memory Total"))
                                gpu.MemoryTotalMB = sensor.Value ?? 0;
                        }
                    }
                }
                computer.Close();
                if (gpu.UsagePercent <= 0)
                {
                    try
                    {
                        gpuUsageCounter ??= new PerformanceCounter(
                            "GPU Engine", "Utilization Percentage", "engtype_3D", true);
                        gpu.UsagePercent = gpuUsageCounter.NextValue();
                    }
                    catch
                    {
                        gpu.UsagePercent = 0;
                    }
                }
                if (string.IsNullOrWhiteSpace(gpu.Name))
                {
                    using var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
                    foreach (var obj in searcher.Get())
                    {
                        gpu.Name = obj["Name"]?.ToString();
                        gpu.MemoryTotalMB = Convert.ToSingle(obj["AdapterRAM"]) / 1024 / 1024;
                        break;
                    }
                }
                if (gpu.MemoryTotalMB > 0 && gpu.MemoryUsedMB == 0)
                    gpu.MemoryUsedMB = gpu.MemoryTotalMB * (gpu.UsagePercent / 100f);
            }
            catch (Exception ex)
            {
                gpu.Name ??= "Не найден GPU";
                gpu.UsagePercent = 0;
                gpu.MemoryUsedMB = 0;
                gpu.MemoryTotalMB = 0;
                gpu.TemperatureC = 0;
                LoggingService.LogError(ex);
            }
            return gpu;
        }
    }
}

