using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using TaskTray.Models;
using TaskTray.Utilities;

namespace TaskTray
{
    public partial class ManagerForm : Form
    {
        private Category? _selectedCategory;
        private string _searchFilter = string.Empty;

        public ManagerForm()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = LanguageManager.GetString("AppTitle");
            DesignSystem.ApplyDarkTheme(this);
            
            this.DoubleBuffered = true;
            
            // Toolbar Events
            txtSearch.TextChanged += OnSearchChanged;
            txtSearch.PlaceholderText = LanguageManager.GetString("Search");
            
            btnAddProgram.Text = LanguageManager.GetString("AddProgram");
            btnAddProgram.Click += OnAddProgramClicked;
            
            btnAddCategory.Click += OnAddCategoryClicked;
            btnSettings.Click += (s, e) => ShowSettings();

            // Category List (Owner Draw)
            lstCategories.DrawItem += OnDrawCategoryItem;
            lstCategories.SelectedIndexChanged += OnCategoryChanged;
            lstCategories.MeasureItem += (s, e) => e.ItemHeight = 45;

            // Action Bar Events
            btnRenameCategory.Text = LanguageManager.GetString("RenameCategory");
            btnRenameCategory.Click += OnRenameCategoryClicked;
            
            btnDeleteCategory.Text = LanguageManager.GetString("Remove");
            btnDeleteCategory.Click += OnDeleteCategoryClicked;
            
            btnImport.Text = LanguageManager.GetString("Import");
            btnImport.Click += OnImportClicked;

            btnExport.Text = LanguageManager.GetString("Export");
            btnExport.Click += OnExportClicked;

            // Empty State Painting
            pnlEmptyState.Paint += OnPaintEmptyState;

            // Drag and Drop
            flowApps.AllowDrop = true;
            flowApps.DragEnter += OnDragEnter;
            flowApps.DragDrop += OnDragDrop;

            // Form closing behavior
            this.FormClosing += (s, e) => {
                if (e.CloseReason == CloseReason.UserClosing) {
                    e.Cancel = true;
                    this.Hide();
                }
            };
        }

        private void OnImportClicked(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = System.IO.File.ReadAllText(ofd.FileName, System.Text.Encoding.UTF8);
                        var imported = System.Text.Json.JsonSerializer.Deserialize<ConfigData>(json);
                        if (imported != null)
                        {
                            ConfigManager.Data.Categories = imported.Categories;
                            ConfigManager.Save();
                            LoadData();
                            TrayRefreshRequested?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show($"Import failed: {ex.Message}"); }
                }
            }
        }

        private void OnExportClicked(object? sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON Files (*.json)|*.json";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                        string json = System.Text.Json.JsonSerializer.Serialize(ConfigManager.Data, options);
                        System.IO.File.WriteAllText(sfd.FileName, json, System.Text.Encoding.UTF8);
                    }
                    catch (Exception ex) { MessageBox.Show($"Export failed: {ex.Message}"); }
                }
            }
        }

        private void LoadData()
        {
            lstCategories.Items.Clear();
            foreach (var cat in ConfigManager.Data.Categories)
            {
                lstCategories.Items.Add(cat.Name);
            }

            if (lstCategories.Items.Count > 0)
                lstCategories.SelectedIndex = 0;
            else
                _selectedCategory = null;
            
            UpdateAppList();
        }

        private void OnDrawCategoryItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Rectangle rect = e.Bounds;
            rect.Inflate(-8, -4); // More padding for list items

            Color bgColor = isSelected ? DesignSystem.AccentColor : Color.Transparent;
            Color textColor = isSelected ? Color.Black : DesignSystem.TextColor;

            if (isSelected)
            {
                DesignSystem.DrawRoundedRectangle(g, rect, 8, bgColor);
            }
            else if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                DesignSystem.DrawRoundedRectangle(g, rect, 8, DesignSystem.SurfaceElevatedColor);
            }

            string text = lstCategories.Items[e.Index].ToString() ?? "";
            TextRenderer.DrawText(g, text, DesignSystem.HeaderFont, rect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding);
        }

        private void OnPaintEmptyState(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = pnlEmptyState.ClientRectangle;

            // Draw a subtle "plus" icon in a circle
            int circleSize = 60;
            Rectangle circleRect = new Rectangle((rect.Width - circleSize) / 2, (rect.Height / 2) - 80, circleSize, circleSize);
            using (SolidBrush brush = new SolidBrush(DesignSystem.SurfaceElevatedColor))
            {
                g.FillEllipse(brush, circleRect);
            }
            using (Pen pen = new Pen(DesignSystem.AccentColor, 3))
            {
                int margin = 18;
                g.DrawLine(pen, circleRect.X + margin, circleRect.Y + circleSize / 2, circleRect.Right - margin, circleRect.Y + circleSize / 2);
                g.DrawLine(pen, circleRect.X + circleSize / 2, circleRect.Y + margin, circleRect.X + circleSize / 2, circleRect.Bottom - margin);
            }

            // Draw text
            string title = LanguageManager.GetString("EmptyStateTitle");
            string desc = LanguageManager.GetString("EmptyStateDesc");

            Rectangle titleRect = new Rectangle(0, circleRect.Bottom + 20, rect.Width, 30);
            Rectangle descRect = new Rectangle(60, titleRect.Bottom + 10, rect.Width - 120, 80);

            TextRenderer.DrawText(g, title, DesignSystem.TitleFont, titleRect, DesignSystem.TextColor, TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, desc, DesignSystem.MainFont, descRect, DesignSystem.TextDimColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
        }

        private void UpdateAppList()
        {
            flowApps.Controls.Clear();
            
            if (_selectedCategory == null)
            {
                pnlEmptyState.Visible = true;
                flowApps.Visible = false;
                return;
            }

            var items = _selectedCategory.Items.AsEnumerable();
            if (!string.IsNullOrEmpty(_searchFilter))
            {
                items = items.Where(i => i.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!items.Any())
            {
                pnlEmptyState.Visible = true;
                flowApps.Visible = false;
            }
            else
            {
                pnlEmptyState.Visible = false;
                flowApps.Visible = true;
                foreach (var app in items)
                {
                    flowApps.Controls.Add(CreateAppRow(app));
                }
            }
        }

        private Control CreateAppRow(AppItem app)
        {
            Panel row = new Panel
            {
                Size = new Size(flowApps.Width - 50, 65),
                BackColor = DesignSystem.SurfaceElevatedColor,
                Margin = new Padding(0, 0, 0, 8),
                Padding = new Padding(10),
                Cursor = Cursors.Hand
            };

            // Custom border and corner logic could be added here if we want more flair
            // For now, let's keep it clean with properties.

            PictureBox pic = new PictureBox
            {
                Size = new Size(32, 32),
                Location = new Point(15, 16),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            if (!string.IsNullOrEmpty(app.IconBase64))
            {
                try {
                    byte[] bytes = Convert.FromBase64String(app.IconBase64);
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        pic.Image = Image.FromStream(ms);
                    }
                } catch { pic.Image = SystemIcons.Application.ToBitmap(); }
            }
            else { pic.Image = SystemIcons.Application.ToBitmap(); }

            Label lblName = new Label
            {
                Text = app.Name,
                Font = DesignSystem.HeaderFont,
                ForeColor = DesignSystem.TextColor,
                Location = new Point(60, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblPath = new Label
            {
                Text = app.Path,
                Font = DesignSystem.SmallFont,
                ForeColor = DesignSystem.TextDimColor,
                Location = new Point(60, 34),
                AutoSize = false,
                Size = new Size(row.Width - 100, 18),
                AutoEllipsis = true,
                BackColor = Color.Transparent
            };

            row.Controls.Add(pic);
            row.Controls.Add(lblName);
            row.Controls.Add(lblPath);

            // Hover effects
            row.MouseEnter += (s, e) => row.BackColor = DesignSystem.HoverColor;
            row.MouseLeave += (s, e) => row.BackColor = DesignSystem.SurfaceElevatedColor;

            foreach (Control c in row.Controls)
            {
                c.Click += (s, e) => LaunchApp(app);
                c.MouseEnter += (s, e) => row.BackColor = DesignSystem.HoverColor;
            }
            row.Click += (s, e) => LaunchApp(app);

            // Context menu
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem removeBtn = new ToolStripMenuItem(LanguageManager.GetString("Remove"));
            removeBtn.Click += (s, e) => RemoveApp(app);
            menu.Items.Add(removeBtn);
            row.ContextMenuStrip = menu;

            return row;
        }

        private void LaunchApp(AppItem app)
        {
            try { Process.Start(new ProcessStartInfo(app.Path) { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void RemoveApp(AppItem app)
        {
            if (_selectedCategory != null)
            {
                if (MessageBox.Show(LanguageManager.GetString("ConfirmDelete"), LanguageManager.GetString("Remove"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _selectedCategory.Items.Remove(app);
                    ConfigManager.Save();
                    UpdateAppList();
                    TrayRefreshRequested?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        public void ShowSettings()
        {
            // For now, let's just show a simple settings dialog or reuse ManagerForm
            // Given the task, I will implement a settings mode or dialog soon.
            MessageBox.Show("Settings Mode Coming Soon", "TaskTray");
        }

        private void OnCategoryChanged(object? sender, EventArgs e)
        {
            if (lstCategories.SelectedIndex >= 0 && lstCategories.SelectedIndex < ConfigManager.Data.Categories.Count)
            {
                _selectedCategory = ConfigManager.Data.Categories[lstCategories.SelectedIndex];
            }
            else
            {
                _selectedCategory = null;
            }
            UpdateAppList();
        }

        private void OnSearchChanged(object? sender, EventArgs e)
        {
            _searchFilter = txtSearch.Text;
            UpdateAppList();
        }

        private void OnAddProgramClicked(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    AddAppToCategory(ofd.FileName);
                }
            }
        }

        private void OnAddCategoryClicked(object? sender, EventArgs e)
        {
            string name = PromptForValue(LanguageManager.GetString("EnterCategoryName"), LanguageManager.GetString("AddCategory"));
            if (!string.IsNullOrWhiteSpace(name))
            {
                ConfigManager.Data.Categories.Add(new Category { Name = name });
                ConfigManager.Save();
                LoadData();
                TrayRefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnRenameCategoryClicked(object? sender, EventArgs e)
        {
            if (_selectedCategory == null) return;
            string name = PromptForValue(LanguageManager.GetString("EnterCategoryName"), LanguageManager.GetString("RenameCategory"), _selectedCategory.Name);
            if (!string.IsNullOrWhiteSpace(name))
            {
                _selectedCategory.Name = name;
                ConfigManager.Save();
                int idx = lstCategories.SelectedIndex;
                LoadData();
                lstCategories.SelectedIndex = idx;
                TrayRefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnDeleteCategoryClicked(object? sender, EventArgs e)
        {
            if (_selectedCategory == null) return;
            if (MessageBox.Show(LanguageManager.GetString("ConfirmDelete"), LanguageManager.GetString("DeleteCategory"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ConfigManager.Data.Categories.Remove(_selectedCategory);
                ConfigManager.Save();
                LoadData();
                TrayRefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private string PromptForValue(string prompt, string title, string defaultValue = "")
        {
            using (var form = new Form())
            {
                form.Text = title;
                form.Size = new Size(350, 160);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.BackColor = DesignSystem.BackgroundColor;
                form.ForeColor = DesignSystem.TextColor;

                var lbl = new Label { Text = prompt, Location = new Point(20, 15), Size = new Size(310, 20) };
                var txt = new TextBox { Text = defaultValue, Location = new Point(20, 45), Size = new Size(295, 25), BackColor = DesignSystem.SurfaceColor, ForeColor = DesignSystem.TextColor, BorderStyle = BorderStyle.FixedSingle };
                var btn = new Button { Text = "OK", Location = new Point(235, 85), Size = new Size(80, 30), DialogResult = DialogResult.OK };

                DesignSystem.ApplyDarkTheme(form);
                form.Controls.AddRange(new Control[] { lbl, txt, btn });
                form.AcceptButton = btn;

                return form.ShowDialog() == DialogResult.OK ? txt.Text : string.Empty;
            }
        }

        private void OnDragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data!.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void OnDragDrop(object? sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data!.GetData(DataFormats.FileDrop)!;
            foreach (string file in files)
            {
                AddAppToCategory(file);
            }
        }

        private void AddAppToCategory(string path)
        {
            if (_selectedCategory == null) return;

            string resolvedPath = ShellUtils.ResolveShortcut(path);
            var app = new AppItem
            {
                Name = ShellUtils.GetFileName(path),
                Path = resolvedPath,
                IconBase64 = ShellUtils.GetIconBase64(resolvedPath)
            };

            _selectedCategory.Items.Add(app);
            ConfigManager.Save();
            UpdateAppList();
            TrayRefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        public static event EventHandler? TrayRefreshRequested;
    }
}
