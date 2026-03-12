using System;
using System.Threading;
using System.Windows;
using TaskTray.Services;

namespace TaskTray
{
    public partial class App : Application
    {
        private static Mutex? _mutex;
        private const string AppGuid = "TaskTray-WPF-7CAA0382-0BF6-4FA3-A08E-5FC668A5F987";

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, AppGuid, out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("TaskTray is already running.");
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);

            // Initialize Services
            ConfigService.Load();
            LanguageService.Initialize();

            // Setup Tray (Handled by TrayService)
            TrayService.Initialize();

            // Note: MainWindow is not set in App.xaml so it doesn't open automatically
        }

        protected override void OnExit(ExitEventArgs e)
        {
            TrayService.Shutdown();
            _mutex?.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
