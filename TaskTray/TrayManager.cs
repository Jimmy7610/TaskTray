using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using TaskTray.Models;
using System.Collections.Generic;

namespace TaskTray
{
    public class TrayManager : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ManagerForm _managerForm;
        private readonly ContextMenuStrip _contextMenu;

        public TrayManager(ManagerForm managerForm)
        {
            _managerForm = managerForm;
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Replace with custom icon if available
                Text = LanguageManager.GetString("AppTitle"),
                Visible = true
            };

            _notifyIcon.MouseClick += OnTrayIconClick;

            _contextMenu = new ContextMenuStrip();
            RefreshMenu();
            _notifyIcon.ContextMenuStrip = _contextMenu;

            ManagerForm.TrayRefreshRequested += (s, e) => RefreshMenu();
        }

        public void RefreshMenu()
        {
            _contextMenu.Items.Clear();

            // Right-click menu items
            var openManagerItem = new ToolStripMenuItem(LanguageManager.GetString("OpenManager"));
            openManagerItem.Click += (s, e) => _managerForm.ShowForm();
            _contextMenu.Items.Add(openManagerItem);

            var settingsItem = new ToolStripMenuItem(LanguageManager.GetString("Settings"));
            settingsItem.Click += (s, e) => _managerForm.ShowSettings();
            _contextMenu.Items.Add(settingsItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            var exitItem = new ToolStripMenuItem(LanguageManager.GetString("Exit"));
            exitItem.Click += (s, e) => Application.Exit();
            _contextMenu.Items.Add(exitItem);
        }

        private void OnTrayIconClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowLaunchMenu();
            }
        }

        private void ShowLaunchMenu()
        {
            var menu = new ContextMenuStrip();
            
            if (ConfigManager.Data.Categories.Count == 0)
            {
                var empty = new ToolStripMenuItem(LanguageManager.GetString("StartupHint"));
                empty.Enabled = false;
                menu.Items.Add(empty);
            }
            else
            {
                foreach (var category in ConfigManager.Data.Categories)
                {
                    if (category.Items.Count == 0) continue;

                    var catItem = new ToolStripMenuItem(category.Name);
                    foreach (var app in category.Items)
                    {
                        var appItem = new ToolStripMenuItem(app.Name);
                        
                        // Set icon if available
                        if (!string.IsNullOrEmpty(app.IconBase64))
                        {
                            try {
                                byte[] imageBytes = Convert.FromBase64String(app.IconBase64);
                                using (var ms = new System.IO.MemoryStream(imageBytes))
                                {
                                    appItem.Image = Image.FromStream(ms);
                                }
                            } catch { /* ignore invalid icon data */ }
                        }

                        appItem.Click += (s, e) => LaunchApp(app);
                        catItem.DropDownItems.Add(appItem);
                    }
                    menu.Items.Add(catItem);
                }
            }

            // If only one category or just a few apps, flatten? 
            // For now, keep it categorized as requested.

            menu.Show(Cursor.Position);
        }

        private void LaunchApp(AppItem app)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = app.Path,
                    Arguments = app.Arguments ?? string.Empty,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetString("AppTitle")}: Failed to launch {app.Name}\n{ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Dispose()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _contextMenu.Dispose();
        }
    }
}
