using System;
using System.Collections.Generic;

namespace TaskTray.Models
{
    public class AppItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string? Arguments { get; set; }
        public string? IconBase64 { get; set; } // Cached base64 icon data
    }
}
