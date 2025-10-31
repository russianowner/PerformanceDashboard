using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace PerformanceDashboard.Forms
{
    partial class ChartsForm
    {
        private Chart chartMain;

        private void InitializeComponent()
        {
            this.chartMain = new Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).BeginInit();
            this.SuspendLayout();
            // chartMain
            this.chartMain.Dock = DockStyle.Fill;
            this.chartMain.BackColor = Color.FromArgb(30, 30, 30);
            this.chartMain.BorderlineColor = Color.Gray;
            this.chartMain.BorderlineWidth = 1;
            this.chartMain.BorderlineDashStyle = ChartDashStyle.Solid;
            this.chartMain.Palette = ChartColorPalette.None;
            // ChartsForm
            this.ClientSize = new Size(900, 600);
            this.Controls.Add(this.chartMain);
            this.Text = "Системные графики";
            this.BackColor = Color.FromArgb(25, 25, 25);
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
