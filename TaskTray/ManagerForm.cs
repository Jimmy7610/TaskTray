using System;
using System.Collections.Generic;
using System.Drawing;
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
            
            // Setup events
            lstCategories.SelectedIndexChanged += OnCategoryChanged;
            txtSearch.TextChanged += OnSearchChanged;
            btnAddProgram.Click += OnAddProgramClicked;
            
            btnAddCategory.Click += OnAddCategoryClicked;
            btnRenameCategory.Click += OnRenameCategoryClicked;
            btnDeleteCategory.Click += OnDeleteCategoryClicked;

            chkAutoStart.CheckedChanged += OnAutoStartChanged;
            cmbLanguage.SelectedIndexChanged += OnLanguageChanged;

            // Drag and drop
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

            // Initialize settings UI
            chkAutoStart.Checked = AutoStartManager.IsEnabled();
            cmbLanguage.Items.Clear();
            cmbLanguage.Items.AddRange(new string[] { "English", "Svenska" });
            cmbLanguage.SelectedIndex = ConfigManager.Data.Language == "sv" ? 1 : 0;
            
            tabMain.SelectedIndexChanged += (s, e) => {
                if (tabMain.SelectedTab == tabSettings)
                {
                    chkAutoStart.Checked = AutoStartManager.IsEnabled();
                }
            };
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

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            string newLang = cmbLanguage.SelectedIndex == 1 ? "sv" : "en";
            if (ConfigManager.Data.Language != newLang)
            {
                ConfigManager.Data.Language = newLang;
                LanguageManager.SetLanguage(newLang);
                ConfigManager.Save();
                // Request restart or re-apply strings? 
                // For simplicity, let's re-apply.
                SetupUI();
                LoadData();
                RefreshTray();
            }
        }

        private void OnAutoStartChanged(object? sender, EventArgs e)
        {
            AutoStartManager.SetEnabled(chkAutoStart.Checked);
            ConfigManager.Data.AutoStart = chkAutoStart.Checked;
            ConfigManager.Save();
        }

        private void RefreshTray()
        {
            TrayRefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        public static event EventHandler? TrayRefreshRequested;

        public void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            tabMain.SelectedTab = tabLauncher;
        }

        public void ShowSettings()
        {
            ShowForm();
            tabMain.SelectedTab = tabSettings;
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

        private void OnAddCategoryClicked(object? sender, EventArgs e)
        {
            string name = PromptForValue(LanguageManager.GetString("EnterCategoryName"), LanguageManager.GetString("AddCategory"));
            if (!string.IsNullOrWhiteSpace(name))
            {
                ConfigManager.Data.Categories.Add(new Category { Name = name });
                ConfigManager.Save();
                LoadData();
                RefreshTray();
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
                RefreshTray();
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
                RefreshTray();
            }
        }

        private string PromptForValue(string prompt, string title, string defaultValue = "")
        {
            using (var form = new Form())
            {
                form.Text = title;
                form.Size = new Size(300, 150);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.BackColor = DesignSystem.BackColor;
                form.ForeColor = DesignSystem.TextColor;

                var lbl = new Label { Text = prompt, Location = new Point(10, 10), Size = new Size(280, 20) };
                var txt = new TextBox { Text = defaultValue, Location = new Point(10, 40), Size = new Size(260, 25), BackColor = DesignSystem.SurfaceColor, ForeColor = DesignSystem.TextColor, BorderStyle = BorderStyle.FixedSingle };
                var btn = new Button { Text = "OK", Location = new Point(190, 80), Size = new Size(80, 25), DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = DesignSystem.SurfaceColor };

                form.Controls.AddRange(new Control[] { lbl, txt, btn });
                form.AcceptButton = btn;

                return form.ShowDialog() == DialogResult.OK ? txt.Text : string.Empty;
            }
        }

        private void OnSearchChanged(object? sender, EventArgs e)
        {
            _searchFilter = txtSearch.Text;
            UpdateAppList();
        }

        private void UpdateAppList()
        {
            flowApps.Controls.Clear();
            
            if (_selectedCategory == null)
            {
                if (ConfigManager.Data.Categories.Count == 0)
                {
                    flowApps.Controls.Add(new Label { Text = LanguageManager.GetString("StartupHint"), AutoSize = true, ForeColor = DesignSystem.TextDimColor, Margin = new Padding(20) });
                }
                return;
            }

            var items = _selectedCategory.Items.AsEnumerable();
            if (!string.IsNullOrEmpty(_searchFilter))
            {
                items = items.Where(i => i.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var app in items)
            {
                flowApps.Controls.Add(CreateAppCard(app));
            }

            if (!items.Any() && string.IsNullOrEmpty(_searchFilter))
            {
                flowApps.Controls.Add(new Label { Text = LanguageManager.GetString("StartupHint"), AutoSize = true, ForeColor = DesignSystem.TextDimColor, Margin = new Padding(20) });
            }
        }

        private Control CreateAppCard(AppItem app)
        {
            var panel = new Panel
            {
                Size = new Size(150, 60),
                BackColor = DesignSystem.SurfaceColor,
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };

            var picIcon = new PictureBox
            {
                Size = new Size(32, 32),
                Location = new Point(10, 14),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            if (!string.IsNullOrEmpty(app.IconBase64))
            {
                try {
                    byte[] bytes = Convert.FromBase64String(app.IconBase64);
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        picIcon.Image = Image.FromStream(ms);
                    }
                } catch { picIcon.Image = SystemIcons.Application.ToBitmap(); }
            }
            else { picIcon.Image = SystemIcons.Application.ToBitmap(); }

            var lblName = new Label
            {
                Text = app.Name,
                Location = new Point(50, 10),
                Size = new Size(90, 40),
                ForeColor = DesignSystem.TextColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(picIcon);
            panel.Controls.Add(lblName);
            
            panel.Click += (s, e) => LaunchApp(app);
            picIcon.Click += (s, e) => LaunchApp(app);
            lblName.Click += (s, e) => LaunchApp(app);

            // Context menu for removal
            var menu = new ContextMenuStrip();
            var removeBtn = new ToolStripMenuItem(LanguageManager.GetString("Remove"));
            removeBtn.Click += (s, e) => RemoveApp(app);
            menu.Items.Add(removeBtn);
            panel.ContextMenuStrip = menu;

            return panel;
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
                _selectedCategory.Items.Remove(app);
                ConfigManager.Save();
                UpdateAppList();
            }
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
            RefreshTray();
        }
    }
}
