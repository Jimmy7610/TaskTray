using System;
using System.Collections.Generic;
using System.Globalization;

namespace TaskTray
{
    public static class LanguageManager
    {
        public static string CurrentLanguage { get; private set; } = "en";

        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new Dictionary<string, Dictionary<string, string>>
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
                ["Apps"] = "Applications",
                ["StartupHint"] = "Drag programs here to start organizing",
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
                ["SettingsTitle"] = "TaskTray Settings"
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
                ["SettingsTitle"] = "TaskTray Inställningar"
            }
        };

        public static void Initialize(string? preferredLanguage = null)
        {
            if (!string.IsNullOrEmpty(preferredLanguage) && Translations.ContainsKey(preferredLanguage))
            {
                CurrentLanguage = preferredLanguage;
            }
            else
            {
                // Detect from system
                var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                CurrentLanguage = Translations.ContainsKey(culture) ? culture : "en";
            }
        }

        public static void SetLanguage(string lang)
        {
            if (Translations.ContainsKey(lang))
            {
                CurrentLanguage = lang;
            }
        }

        public static string GetString(string key)
        {
            if (Translations[CurrentLanguage].TryGetValue(key, out var text))
                return text;
            
            // Fallback to English if key missing in current lang
            if (CurrentLanguage != "en" && Translations["en"].TryGetValue(key, out var engText))
                return engText;

            return key;
        }
    }
}
