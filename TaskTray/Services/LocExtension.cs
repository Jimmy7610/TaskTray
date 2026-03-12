using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using TaskTray.Services;

namespace TaskTray.Services
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocExtension : MarkupExtension
    {
        [ConstructorArgument("key")]
        public string Key { get; set; }

        public LocExtension() { }

        public LocExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key)) return string.Empty;

            var binding = new Binding
            {
                Source = LanguageService.Instance,
                Path = new PropertyPath($"[{Key}]"),
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}
