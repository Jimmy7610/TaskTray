using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace TaskTray.Services
{
    public class LanguageService : INotifyPropertyChanged
    {
        private static LanguageService _instance = new();
        public static LanguageService Instance => _instance;

        private string _currentLanguage = "en";
        public string CurrentLanguage
        {
            get => _currentLanguage;
            private set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnPropertyChanged(null); // Notify everything changed
                }
            }
        }

        public string this[string key] => GetString(key);

        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            ["en"] = new Dictionary<string, string>
            {
                ["AppTitle"] = "TaskTray",
                ["OpenManager"] = "Open Manager",
                ["Settings"] = "Settings",
                ["Exit"] = "Exit",
                ["AddProgram"] = "Add Program",
                ["Remove"] = "Remove",
                ["Search"] = "Search apps...",
                ["Categories"] = "Categories",
                ["Apps"] = "Apps",
                ["StartupHint"] = "Drag apps here to start organizing",
                ["Language"] = "Language",
                ["AutoStart"] = "Run at Windows startup",
                ["Save"] = "Save",
                ["Cancel"] = "Cancel",
                ["ConfirmDelete"] = "Are you sure you want to remove this item?",
                ["Work"] = "Work",
                ["Games"] = "Games",
                ["Tools"] = "Tools",
                ["AddCategory"] = "New Category",
                ["DeleteCategory"] = "Delete Category",
                ["RenameCategory"] = "Rename",
                ["EnterCategoryName"] = "Enter category name:",
                ["SettingsTitle"] = "TaskTray Settings",
                ["Import"] = "Import",
                ["Export"] = "Export",
                ["EmptyStateTitle"] = "No apps here yet",
                ["EmptyStateDesc"] = "Add apps by dragging them here or using the '+' button above.",
                ["AlreadyRunning"] = "TaskTray is already running.",
                ["LaunchFailed"] = "Failed to launch: "
            },
            ["sv"] = new Dictionary<string, string>
            {
                ["AppTitle"] = "TaskTray",
                ["OpenManager"] = "Öppna hanteraren",
                ["Settings"] = "Inställningar",
                ["Exit"] = "Avsluta",
                ["AddProgram"] = "Lägg till program",
                ["Remove"] = "Ta bort",
                ["Search"] = "Sök appar...",
                ["Categories"] = "Kategorier",
                ["Apps"] = "Program",
                ["StartupHint"] = "Dra program hit för att börja organisera",
                ["Language"] = "Språk",
                ["AutoStart"] = "Kör vid Windows-uppstart",
                ["Save"] = "Spara",
                ["Cancel"] = "Avbryt",
                ["ConfirmDelete"] = "Är du säker på att du vill ta bort detta objekt?",
                ["Work"] = "Arbete",
                ["Games"] = "Spel",
                ["Tools"] = "Verktyg",
                ["AddCategory"] = "Ny kategori",
                ["DeleteCategory"] = "Ta bort kategori",
                ["RenameCategory"] = "Byt namn",
                ["EnterCategoryName"] = "Ange kategorinamn:",
                ["SettingsTitle"] = "TaskTray Inställningar",
                ["Import"] = "Importera",
                ["Export"] = "Exportera",
                ["EmptyStateTitle"] = "Inga appar här än",
                ["EmptyStateDesc"] = "Lägg till appar genom att dra dem hit eller använd '+'-knappen ovan.",
                ["AlreadyRunning"] = "TaskTray körs redan.",
                ["LaunchFailed"] = "Misslyckades att starta: "
            }
        };

        public static void Initialize()
        {
            string systemLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            string target = ConfigService.Data.Language;

            if (string.IsNullOrEmpty(target))
                target = Translations.ContainsKey(systemLang) ? systemLang : "en";

            _instance.CurrentLanguage = target;
        }

        public static string GetString(string key)
        {
            var current = _instance.CurrentLanguage;
            if (Translations.ContainsKey(current) && Translations[current].ContainsKey(key))
                return Translations[current][key];

            // Fallback to English
            if (Translations["en"].ContainsKey(key))
                return Translations["en"][key];

            return $"[{key}]";
        }

        public static void SetLanguage(string lang)
        {
            if (Translations.ContainsKey(lang))
            {
                _instance.CurrentLanguage = lang;
                ConfigService.Data.Language = lang;
                ConfigService.Save();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
