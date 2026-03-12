using System;
using System.Collections.Generic;

namespace TaskTray.Models
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public List<AppItem> Items { get; set; } = new List<AppItem>();
    }
}
