using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaskTray.Services;

namespace TaskTray.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private bool _isAutoStartEnabled;
        private string _selectedLanguage;

        public ObservableCollection<string> Languages { get; } = new() { "en", "sv" };

        public SettingsViewModel()
        {
            // Initial state from config/system
            _isAutoStartEnabled = ConfigService.Data.AutoStart;
            _selectedLanguage = LanguageService.CurrentLanguage;

            SaveCommand = new RelayCommand(_ => Save());
        }

        public bool IsAutoStartEnabled
        {
            get => _isAutoStartEnabled;
            set => SetProperty(ref _isAutoStartEnabled, value);
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public ICommand SaveCommand { get; }

        private void Save()
        {
            // Update AutoStart
            if (ConfigService.Data.AutoStart != IsAutoStartEnabled)
            {
                ConfigService.Data.AutoStart = IsAutoStartEnabled;
                StartupService.SetEnabled(IsAutoStartEnabled);
            }

            // Update Language
            if (LanguageService.CurrentLanguage != SelectedLanguage)
            {
                LanguageService.SetLanguage(SelectedLanguage);
                TrayService.RefreshMenu();
                // Note: Full UI update might require window refresh or localized bindings
            }

            ConfigService.Save();
        }
    }
}
