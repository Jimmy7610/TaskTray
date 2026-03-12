using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TaskTray.Views;

namespace TaskTray.Services
{
    public static class TrayService
    {
        private static NotifyIcon? _notifyIcon;
        private static MainWindow? _mainWindow;

        public static void Initialize()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = LanguageService.GetString("AppTitle")
            };

            _notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ShowLauncherMenu();
                }
            };

            RefreshMenu();
        }

        public static void RefreshMenu()
        {
            if (_notifyIcon == null) return;

            var menu = new ContextMenuStrip();
            
            // Core Menu
            var openItem = new ToolStripMenuItem(LanguageService.GetString("OpenManager"), null, (s, e) => ShowManager());
            openItem.Font = new Font(openItem.Font, FontStyle.Bold);
            menu.Items.Add(openItem);
            
            menu.Items.Add(new ToolStripSeparator());

            // Exit
            menu.Items.Add(new ToolStripMenuItem(LanguageService.GetString("Exit"), null, (s, e) => System.Windows.Application.Current.Shutdown()));

            _notifyIcon.ContextMenuStrip = menu;
        }

        private static void ShowLauncherMenu()
        {
            if (_notifyIcon == null) return;

            var menu = new ContextMenuStrip();
            foreach (var category in ConfigService.Data.Categories)
            {
                if (category.Items.Count == 0) continue;

                var catItem = new ToolStripMenuItem(category.Name);
                foreach (var app in category.Items)
                {
                    var appItem = new ToolStripMenuItem(app.Name, null, (s, e) => LaunchApp(app.Path));
                    catItem.DropDownItems.Add(appItem);
                }
                menu.Items.Add(catItem);
            }

            if (menu.Items.Count == 0)
            {
                menu.Items.Add(new ToolStripMenuItem(LanguageService.GetString("StartupHint")) { Enabled = false });
            }

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(LanguageService.GetString("OpenManager"), null, (s, e) => ShowManager()));

            // Position and show menu near the tray area
            Point pos = Control.MousePosition;
            menu.Show(pos);
        }

        private static void ShowManager()
        {
            if (_mainWindow == null)
            {
                _mainWindow = new MainWindow();
                _mainWindow.Closed += (s, e) => _mainWindow = null;
            }
            
            _mainWindow.Show();
            _mainWindow.Activate();
            _mainWindow.Focus();
        }

        private static void LaunchApp(string path)
        {
            try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show($"Failed to launch: {ex.Message}"); }
        }

        public static void Shutdown()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
        }
    }
}
