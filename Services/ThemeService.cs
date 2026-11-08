using System;
using System.Linq;
using System.Windows;

namespace AganBrowser.Services
{
    public static class ThemeService
    {
        public static bool IsDark { get; private set; } = true;

        public static void Apply(string theme)
        {
            var app = Application.Current;
            if (app == null) return;
            var dict = new ResourceDictionary
            {
                Source = new Uri($"Themes/{theme}Theme.xaml", UriKind.Relative)
            };
            var existing = app.Resources.MergedDictionaries
                .Where(d => d.Source != null && d.Source.OriginalString.Contains("Theme.xaml"))
                .ToList();
            foreach (var d in existing) app.Resources.MergedDictionaries.Remove(d);
            app.Resources.MergedDictionaries.Add(dict);
            IsDark = theme == "Dark";
        }

        public static void Toggle() => Apply(IsDark ? "Light" : "Dark");
    }
}