using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AganBrowser.Services
{
    public static class ThemeService
    {
        public static bool IsDark { get; private set; } = true;
        public static string CurrentThemeName { get; private set; } = "Dark";

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
            
            CurrentThemeName = theme;
            IsDark = theme != "Light" && theme != "Sepia";
        }

        public static void Toggle() => Apply(IsDark ? "Light" : "Dark");

        public static Dictionary<string, string> GetCurrentColors()
        {
            var colors = new Dictionary<string, string>();
            var app = Application.Current;
            if (app == null) return colors;

            string GetHex(string key)
            {
                if (app.Resources[key] is SolidColorBrush brush)
                {
                    var c = brush.Color;
                    return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                }
                return "#000000";
            }

            colors["--bg"] = GetHex("BackgroundBrush");
            colors["--surface"] = GetHex("SurfaceBrush");
            colors["--text"] = GetHex("ForegroundBrush");
            colors["--muted"] = GetHex("MutedForegroundBrush");
            colors["--accent"] = GetHex("AccentBrush");
            colors["--border"] = GetHex("BorderBrush");
            
            return colors;
        }
    }
}
