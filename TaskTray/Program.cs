using System;
using System.Threading;
using System.Windows.Forms;

namespace TaskTray
{
    internal static class Program
    {
        private static Mutex? _mutex;

        [STAThread]
        static void Main()
        {
            const string mutexName = "Global\\TaskTray_SingleInstance_Mutex";
            _mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew)
            {
                MessageBox.Show("TaskTray is already running.", "TaskTray", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize foundation
            ConfigManager.Load();
            LanguageManager.Initialize(ConfigManager.Data.Language);

            var managerForm = new ManagerForm();
            var trayManager = new TrayManager(managerForm);

            Application.Run();
            
            _mutex.ReleaseMutex();
        }
    }
}