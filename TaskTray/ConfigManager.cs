using System;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Diagnostics;
using TaskTray.Models;

namespace TaskTray
{
    public static class ConfigManager
    {
        private static readonly string ConfigFileName = "tasktray_config.json";
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
        
        public static ConfigData Data { get; private set; } = new ConfigData();

        public static void Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath, Encoding.UTF8);
                    Data = JsonSerializer.Deserialize<ConfigData>(json) ?? ConfigData.CreateDefault();
                }
                else
                {
                    Data = ConfigData.CreateDefault();
                    Save();
                }
            }
            catch (Exception)
            {
                Data = ConfigData.CreateDefault();
            }
        }

        public static void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Data, options);
                File.WriteAllText(ConfigPath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save config: {ex.Message}");
            }
        }
    }
}
