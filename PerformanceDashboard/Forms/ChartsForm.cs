#nullable disable
using PerformanceDashboard.Models;
using PerformanceDashboard.Services;
using System.Windows.Forms.DataVisualization.Charting;


namespace PerformanceDashboard.Forms
{
    public partial class ChartsForm : Form
    {
        private readonly SystemMonitorService monitorService = new();
        private readonly System.Windows.Forms.Timer updateTimer;
        private readonly Dictionary<string, Series> seriesDict = new();
        private readonly Dictionary<string, Label> valueLabels = new();
        private TableLayoutPanel layout;
        private int tick = 0;

        public ChartsForm()
        {
            InitializeComponent();
            InitializeLayout();
            InitializeCharts();
            InitializeValueLabels();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme(ThemeManager.CurrentTheme);
            updateTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            updateTimer.Tick += UpdateCharts!;
            updateTimer.Start();
        }
        private void ApplyTheme(Theme theme)
        {
            if (theme == null) return;
            this.BackColor = theme.ToColor(theme.BackgroundColor);
            layout.BackColor = theme.ToColor(theme.BackgroundColor);
            foreach (var lbl in valueLabels.Values)
                lbl.ForeColor = theme.ToColor(theme.TextColor);
            chartMain.BackColor = theme.ToColor(theme.ChartBackground ?? theme.PanelColor);
            chartMain.ChartAreas[0].BackColor = theme.ToColor(theme.ChartBackground ?? theme.PanelColor);
            chartMain.ChartAreas[0].AxisX.LabelStyle.ForeColor = theme.ToColor(theme.TextColor);
            chartMain.ChartAreas[0].AxisY.LabelStyle.ForeColor = theme.ToColor(theme.TextColor);
        }
        private void InitializeLayout()
        {
            layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.Controls.Clear(); 
            layout.Controls.Add(chartMain, 1, 0);
            this.Controls.Add(layout);
        }
        private void InitializeCharts()
        {
            chartMain.ChartAreas.Clear();
            chartMain.Series.Clear();
            chartMain.Legends.Clear();
            var area = new ChartArea("MainArea");
            area.AxisX.Title = "Время (сек)";
            area.AxisY.Title = "Проценты / Скорость";
            area.AxisX.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.BackColor = Color.FromArgb(40, 40, 40);
            chartMain.ChartAreas.Add(area);
            chartMain.BackColor = Color.FromArgb(30, 30, 30);
            chartMain.ForeColor = Color.White;
            AddSeries("CPU", Color.Lime);
            AddSeries("Memory", Color.DodgerBlue);
            AddSeries("Disk", Color.Orange);
            AddSeries("Network", Color.Yellow);
            AddSeries("GPU", Color.MediumPurple);
        }
        private void AddSeries(string name, Color color)
        {
            var s = new Series(name)
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                Color = color
            };
            chartMain.Series.Add(s);
            seriesDict[name] = s;
        }
        private void InitializeValueLabels()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 25, 25),
                Padding = new Padding(10)
            };
            layout.Controls.Add(panel, 0, 0);
            string[] names = { "CPU", "Memory", "Disk", "Network", "GPU" };
            Color[] colors = { Color.Lime, Color.DodgerBlue, Color.Orange, Color.Yellow, Color.MediumPurple };
            int y = 20;
            for (int i = 0; i < names.Length; i++)
            {
                var lbl = new Label
                {
                    Text = $"{names[i]}: 0%",
                    ForeColor = colors[i],
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, y)
                };
                y += 35;
                panel.Controls.Add(lbl);
                valueLabels[names[i]] = lbl;
            }
        }
        private void UpdateCharts(object sender, EventArgs e)
        {
            tick++;
            try
            {
                var cpu = monitorService.GetCpuInfo();
                var mem = monitorService.GetMemoryInfo();
                var disks = monitorService.GetDiskInfo();
                var nets = monitorService.GetNetworkInfo();
                var gpu = monitorService.GetGpuInfo();
                AddPoint("CPU", cpu.UsagePercent);
                AddPoint("Memory", mem.UsagePercent);
                if (disks.Count > 0)
                    AddPoint("Disk", disks[0].UsagePercent);
                if (nets.Count > 0)
                    AddPoint("Network", (nets[0].DownloadKBps + nets[0].UploadKBps) / 50.0);
                if (gpu != null)
                    AddPoint("GPU", gpu.UsagePercent);
                foreach (var s in seriesDict.Values)
                {
                    if (s.Points.Count > 60)
                        s.Points.RemoveAt(0);
                }
                chartMain.ChartAreas[0].AxisX.Minimum = Math.Max(0, tick - 60);
                chartMain.ChartAreas[0].AxisX.Maximum = tick;
                valueLabels["CPU"].Text = $"CPU: {cpu.UsagePercent:F1}%";
                valueLabels["Memory"].Text = $"ОЗУ: {mem.UsagePercent:F1}%";
                valueLabels["Disk"].Text = $"Диск: {disks.FirstOrDefault()?.UsagePercent:F1}%";
                valueLabels["Network"].Text = $"Интернет: ↓{nets.FirstOrDefault()?.DownloadKBps:F0} / ↑{nets.FirstOrDefault()?.UploadKBps:F0} KB/s";
                valueLabels["GPU"].Text = $"GPU: {gpu?.UsagePercent:F1}%";
                chartMain.Invalidate();
            }
            catch { }
        }
        private void AddPoint(string name, double value)
        {
            if (seriesDict.ContainsKey(name))
                seriesDict[name].Points.AddXY(tick, value);
        }
    }
}
