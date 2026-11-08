using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class ShortcutService
    {
        private readonly string _filePath;
        public ObservableCollection<ShortcutItem> Shortcuts { get; } = new();

        public ShortcutService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "shortcuts.json");
            Load();
        }

        public void Add(ShortcutItem item)
        {
            Shortcuts.Add(item);
            Save();
        }

        public void Remove(ShortcutItem item)
        {
            Shortcuts.Remove(item);
            Save();
        }

        private void Load()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                string json = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<ShortcutItem>>(json);
                if (list != null)
                    foreach (var s in list) Shortcuts.Add(s);
            }
            catch { }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(new List<ShortcutItem>(Shortcuts));
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }
    }
}