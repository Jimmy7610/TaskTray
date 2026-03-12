using System;
using System.Collections.Generic;

namespace TaskTray.Models
{
    public class ConfigData
    {
        public string Language { get; set; } = "en"; // "en" or "sv"
        public bool AutoStart { get; set; } = false;
        public List<Category> Categories { get; set; } = new List<Category>();
        public double WindowWidth { get; set; } = 800;
        public double WindowHeight { get; set; } = 500;
        
        public static ConfigData CreateDefault()
        {
            return new ConfigData
            {
                Categories = new List<Category>
                {
                    new Category { Name = "Work" },
                    new Category { Name = "Games" },
                    new Category { Name = "Tools" }
                }
            };
        }
    }
}
