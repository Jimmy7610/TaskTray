using System;
using System.IO;

namespace TaskTray.Services
{
    public static class StartupService
    {
        private static readonly string StartupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "TaskTray.lnk");

        public static void SetEnabled(bool enabled)
        {
            try
            {
                if (enabled)
                {
                    Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
                    if (shellType == null) return;

                    dynamic? shell = Activator.CreateInstance(shellType);
                    if (shell == null) return;

                    dynamic shortcut = shell.CreateShortcut(StartupPath);
                    shortcut.TargetPath = Environment.ProcessPath;
                    shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    shortcut.Description = "Launch TaskTray on startup";
                    shortcut.Save();
                }
                else
                {
                    if (System.IO.File.Exists(StartupPath))
                        System.IO.File.Delete(StartupPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set auto-start: {ex.Message}");
            }
        }
    }
}
