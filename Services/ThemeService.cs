using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace AganBrowser.Services
{
    public class ThemeSettings
    {
        public string Theme { get; set; } = "Dark";
    }

    public static class ThemeService
    {
        public static bool IsDark { get; private set; } = true;
        public static string CurrentThemeName { get; private set; } = "Dark";

        private static readonly string _settingsPath;

        static ThemeService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _settingsPath = Path.Combine(dir, "theme.json");
        }

        public static void Initialize()
        {
            string saved = LoadSavedTheme() ?? "Dark";
            Apply(saved, save: false);
        }

        public static void Apply(string theme, bool save = true)
        {
            var app = Application.Current;
            if (app == null) return;

            try
            {
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

                if (save) SaveTheme(theme);
            }
            catch { }
        }

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

        private static string? LoadSavedTheme()
        {
            try
            {
                if (!File.Exists(_settingsPath)) return null;
                var settings = JsonSerializer.Deserialize<ThemeSettings>(File.ReadAllText(_settingsPath));
                return settings?.Theme;
            }
            catch { return null; }
        }

        private static void SaveTheme(string theme)
        {
            try
            {
                File.WriteAllText(_settingsPath, JsonSerializer.Serialize(new ThemeSettings { Theme = theme }));
            }
            catch { }
        }
    }
}
