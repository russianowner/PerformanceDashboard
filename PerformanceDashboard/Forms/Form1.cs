#nullable disable
using PerformanceDashboard.Models;
using PerformanceDashboard.Services;

namespace PerformanceDashboard
{
    public partial class Form1 : Form
    {
        private readonly SystemMonitorService monitorService = new();
        private Theme currentTheme;
        private System.Windows.Forms.Timer updateTimer;
        private int selectedDiskIndex = 0;

        public Form1()
        {
            InitializeComponent();
            ApplyIcons();
            ApplyWindowIcons();
            ThemeManager.InitDefaultTheme();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme(ThemeManager.CurrentTheme);
            PopulateDiskSelector();
            InitTimer();
        }
        private void ApplyWindowIcons()
        {
            var logo = IconService.GetIcon("app_logo.png");
            if (logo != null)
                this.Icon = Icon.FromHandle(((Bitmap)logo).GetHicon());
        }
        private void ApplyTheme(Theme theme)
        {
            try
            {
                if (theme == null) return;
                var back = theme.ToColor(theme.BackgroundColor);
                var text = theme.ToColor(theme.TextColor);
                var panel = theme.ToColor(theme.PanelColor);
                var accent = theme.ToColor(theme.AccentColor);
                this.BackColor = back;
                this.ForeColor = text;
                foreach (var label in new[] { labelCpu, labelMemory, labelDisk, labelNetwork, labelGpu })
                {
                    label.ForeColor = text;
                    label.BackColor = back;
                }
                foreach (var btn in new[] { btnExport, btnSettings, btnRefresh, btnCharts, btnInfo })
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.UseVisualStyleBackColor = false;
                    btn.BackColor = panel;
                    btn.ForeColor = text;
                    btn.FlatAppearance.BorderColor = accent;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(panel, 0.2f);
                    btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(panel, 0.2f);
                }
                comboDiskSelector.BackColor = panel;
                comboDiskSelector.ForeColor = text;
                comboDiskSelector.FlatStyle = FlatStyle.Flat;
                this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            }
            catch { }
        }
        private void ApplyIcons()
        {
            btnExport.Image = IconService.GetIcon("export.png");
            btnExport.ImageAlign = ContentAlignment.MiddleLeft;

            btnSettings.Image = IconService.GetIcon("settings.png");
            btnSettings.ImageAlign = ContentAlignment.MiddleLeft;

            btnRefresh.Image = IconService.GetIcon("refresh.png");
            btnRefresh.ImageAlign = ContentAlignment.MiddleLeft;

            btnCharts.Image = IconService.GetIcon("chart.png");
            btnCharts.ImageAlign = ContentAlignment.MiddleLeft;

            btnInfo.Image = IconService.GetIcon("info.png");
            btnInfo.ImageAlign = ContentAlignment.MiddleLeft;
        }
        private void InitTimer()
        {
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += (s, e) => UpdateMetrics();
            updateTimer.Start();
        }
        private void LoadTheme(string path)
        {
            try
            {
                currentTheme = ThemeService.LoadTheme(path);
                if (currentTheme == null) return;

                ApplyTheme(currentTheme);
            }
            catch { }
        }
        private void PopulateDiskSelector()
        {
            try
            {
                var disks = monitorService.GetDiskInfo();
                comboDiskSelector.Items.Clear();
                foreach (var d in disks)
                    comboDiskSelector.Items.Add(d.DriveName);
                if (comboDiskSelector.Items.Count > 0)
                    comboDiskSelector.SelectedIndex = 0;
            }
            catch { }
        }
        private void ComboDiskSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDiskIndex = comboDiskSelector.SelectedIndex;
        }
        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var cpu = monitorService.GetCpuInfo();
                var mem = monitorService.GetMemoryInfo();
                var disks = monitorService.GetDiskInfo();
                var nets = monitorService.GetNetworkInfo();
                var gpu = monitorService.GetGpuInfo();
                string path = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                ExportService.ExportToCsv(path, cpu, mem, disks, nets, gpu);

                MessageBox.Show($"Экспорт завершён!\nФайл: {path}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }
        private void UpdateMetrics()
        {
            try
            {
                var cpu = monitorService.GetCpuInfo();
                var mem = monitorService.GetMemoryInfo();
                var disks = monitorService.GetDiskInfo();
                var nets = monitorService.GetNetworkInfo();
                var gpu = monitorService.GetGpuInfo();
                labelCpu.Text = $"CPU: {cpu.UsagePercent:F1}%";
                labelMemory.Text = $"ОЗУ: {mem.UsagePercent:F1}% ({mem.UsedMB:F0}/{mem.TotalMB:F0} MB)";
                if (disks.Count > 0)
                {
                    var index = Math.Min(selectedDiskIndex, disks.Count - 1);
                    var d = disks[index];
                    labelDisk.Text = $"Диск {d.DriveName}: {d.UsedGB:F1}/{d.TotalGB:F1} GB ({d.UsagePercent:F1}%) Active: {d.ActivityPercent:F1}%";
                }
                else labelDisk.Text = "Диск: N/A";
                if (nets.Count > 0)
                {
                    var n = nets[0];
                    labelNetwork.Text = $"Интернет: ↓ {n.DownloadKBps:F1} / ↑ {n.UploadKBps:F1} KB/s";
                }
                else labelNetwork.Text = "Интернет: N/A";
                labelGpu.Text = gpu != null
                    ? $"GPU: {gpu.Name} ({gpu.UsagePercent:F1}%)"
                    : "GPU: N/A";
            }
            catch { }        
        }
    }
}
