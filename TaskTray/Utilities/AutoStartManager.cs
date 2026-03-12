using System;
using System.IO;
using System.Windows.Forms;

namespace TaskTray.Utilities
{
    public static class AutoStartManager
    {
        private static readonly string StartupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        private static readonly string ShortcutPath = Path.Combine(StartupFolderPath, "TaskTray.lnk");

        public static bool IsEnabled()
        {
            return File.Exists(ShortcutPath);
        }

        public static void SetEnabled(bool enable)
        {
            try
            {
                if (enable)
                {
                    if (!File.Exists(ShortcutPath))
                    {
                        CreateShortcut();
                    }
                }
                else
                {
                    if (File.Exists(ShortcutPath))
                    {
                        File.Delete(ShortcutPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update auto-start: {ex.Message}", "TaskTray", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CreateShortcut()
        {
            try
            {
                Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType != null)
                {
                    dynamic shell = Activator.CreateInstance(shellType)!;
                    dynamic shortcut = shell.CreateShortcut(ShortcutPath);
                    shortcut.TargetPath = Application.ExecutablePath;
                    shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    shortcut.Description = "TaskTray Startup Shortcut";
                    shortcut.Save();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not create shortcut: {ex.Message}");
            }
        }
    }
}
