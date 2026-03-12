using System;
using System.Collections.Generic;
using System.Globalization;

namespace TaskTray.Services
{
    public static class LanguageService
    {
        public static string CurrentLanguage { get; private set; } = "en";

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
                ["EmptyStateDesc"] = "Add apps by dragging them here or using the '+' button above."
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
                ["EmptyStateDesc"] = "Lägg till appar genom att dra dem hit eller använd '+'-knappen ovan."
            }
        };

        public static void Initialize()
        {
            string systemLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            string target = ConfigService.Data.Language;

            if (string.IsNullOrEmpty(target))
                target = Translations.ContainsKey(systemLang) ? systemLang : "en";

            CurrentLanguage = target;
        }

        public static string GetString(string key)
        {
            if (Translations.ContainsKey(CurrentLanguage) && Translations[CurrentLanguage].ContainsKey(key))
                return Translations[CurrentLanguage][key];

            // Fallback to English
            if (Translations["en"].ContainsKey(key))
                return Translations["en"][key];

            return $"[{key}]";
        }

        public static void SetLanguage(string lang)
        {
            if (Translations.ContainsKey(lang))
            {
                CurrentLanguage = lang;
                ConfigService.Data.Language = lang;
                ConfigService.Save();
            }
        }
    }
}
