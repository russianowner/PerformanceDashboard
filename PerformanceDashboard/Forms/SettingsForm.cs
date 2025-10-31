#nullable disable
using PerformanceDashboard.Models;
using PerformanceDashboard.Services;

namespace PerformanceDashboard.Forms
{
    public partial class SettingsForm : Form
    {
        public event Action<Theme> ThemeChanged;

        public SettingsForm()
        {
            InitializeComponent();
            LoadThemes();
            Icon = Icon.FromHandle(((Bitmap)IconService.GetIcon("settings.png")).GetHicon());
        }
        private void LoadThemes()
        {
            try
            {
                var files = Directory.GetFiles("Resources/themes", "*.json");
                listBoxThemes.Items.Clear();
                foreach (var file in files)
                    listBoxThemes.Items.Add(file);
            }
            catch { }
        }
        public void ApplyTheme(Theme theme)
        {
            try
            {
                this.BackColor = theme.ToColor(theme.BackgroundColor);
                this.ForeColor = theme.ToColor(theme.TextColor);
                labelHeader.ForeColor = theme.ToColor(theme.AccentColor);
                listBoxThemes.BackColor = theme.ToColor(theme.PanelColor);
                listBoxThemes.ForeColor = theme.ToColor(theme.TextColor);
                listBoxThemes.BorderStyle = BorderStyle.FixedSingle;
                foreach (var btn in new[] { btnApply, btnCancel })
                {
                    btn.BackColor = theme.ToColor(theme.PanelColor);
                    btn.ForeColor = theme.ToColor(theme.TextColor);

                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = theme.ToColor(theme.AccentColor);
                    btn.FlatAppearance.MouseOverBackColor = theme.ToColor(theme.AccentColor);
                    btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(theme.ToColor(theme.AccentColor));
                }
                this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            }
            catch { }
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (listBoxThemes.SelectedItem is string selectedTheme)
            {
                try
                {
                    var theme = ThemeService.LoadTheme(selectedTheme);
                    ThemeManager.ApplyTheme(theme); 

                    MessageBox.Show($"Тема '{Path.GetFileNameWithoutExtension(selectedTheme)}' применена");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите тему");
            }
        }
        private void btnCancel_Click(object sender, EventArgs e) => this.Close();
    }
}
