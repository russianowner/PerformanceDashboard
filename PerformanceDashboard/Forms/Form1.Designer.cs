using System.Windows.Forms;
using System.Drawing;

namespace PerformanceDashboard
{
    partial class Form1 : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Label labelCpu;
        private Label labelMemory;
        private Label labelDisk;
        private Label labelNetwork;
        private Label labelGpu;
        private ComboBox comboDiskSelector;
        private Button btnExport;
        private Button btnSettings;
        private Button btnRefresh;
        private Button btnCharts;
        private Button btnInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelCpu = new Label();
            this.labelMemory = new Label();
            this.labelDisk = new Label();
            this.labelNetwork = new Label();
            this.labelGpu = new Label();
            this.comboDiskSelector = new ComboBox();
            this.btnExport = new Button();
            this.btnSettings = new Button();
            this.btnRefresh = new Button();
            this.btnCharts = new Button();
            this.btnInfo = new Button();
            this.SuspendLayout();
            // 
            // labelCpu
            // 
            this.labelCpu.AutoSize = true;
            this.labelCpu.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.labelCpu.Location = new Point(30, 30);
            // 
            // labelMemory
            // 
            this.labelMemory.AutoSize = true;
            this.labelMemory.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.labelMemory.Location = new Point(30, 60);
            // 
            // labelDisk
            // 
            this.labelDisk.AutoSize = true;
            this.labelDisk.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.labelDisk.Location = new Point(30, 90);
            // 
            // comboDiskSelector
            // 
            this.comboDiskSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboDiskSelector.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.comboDiskSelector.Location = new Point(450, 90);
            this.comboDiskSelector.Size = new Size(100, 25);
            this.comboDiskSelector.SelectedIndexChanged += new System.EventHandler(this.ComboDiskSelector_SelectedIndexChanged);
            // 
            // labelNetwork
            // 
            this.labelNetwork.AutoSize = true;
            this.labelNetwork.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.labelNetwork.Location = new Point(30, 120);
            // 
            // labelGpu
            // 
            this.labelGpu.AutoSize = true;
            this.labelGpu.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.labelGpu.Location = new Point(30, 150);
            // 
            // btnExport
            // 
            this.btnExport.Location = new Point(30, 200);
            this.btnExport.Size = new Size(120, 30);
            this.btnExport.Text = "Экспорт CSV";
            this.btnExport.Click += BtnExport_Click;
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new Point(170, 200);
            this.btnSettings.Size = new Size(120, 30);
            this.btnSettings.Text = "Настройки";
            this.btnSettings.Click += (s, e) => new Forms.SettingsForm().ShowDialog();
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new Point(310, 200);
            this.btnRefresh.Size = new Size(120, 30);
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.Click += (s, e) => UpdateMetrics();
            // 
            // btnCharts
            // 
            this.btnCharts.Location = new Point(30, 240);
            this.btnCharts.Size = new Size(120, 30);
            this.btnCharts.Text = "Графики";
            this.btnCharts.Click += (s, e) => new Forms.ChartsForm().ShowDialog();
            // 
            // Form1
            // 
            this.ClientSize = new Size(600, 350);
            this.Controls.Add(this.labelCpu);
            this.Controls.Add(this.labelMemory);
            this.Controls.Add(this.labelDisk);
            this.Controls.Add(this.labelNetwork);
            this.Controls.Add(this.labelGpu);
            this.Controls.Add(this.comboDiskSelector);
            this.Controls.Add(this.btnExport);  
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCharts);
            this.Controls.Add(this.btnInfo);
            this.Name = "Form1";
            this.Text = "Мониторинг производительности";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
