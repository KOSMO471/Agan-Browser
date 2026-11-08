using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class HistoryService
    {
        private readonly string _filePath;
        public List<HistoryItem> Items { get; private set; } = new();

        public HistoryService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "history.json");
            Load();
        }

        public void Add(string title, string url)
        {
            Items.Add(new HistoryItem { Title = title, Url = url, VisitedAt = DateTime.Now });
            if (Items.Count > 100)
            {
                Items = Items.OrderByDescending(x => x.VisitedAt).Take(100).OrderBy(x => x.VisitedAt).ToList();
            }
            Save();
        }

        public void Clear()
        {
            Items.Clear();
            Save();
        }

        public void Load()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                string json = File.ReadAllText(_filePath);
                Items = JsonSerializer.Deserialize<List<HistoryItem>>(json) ?? new List<HistoryItem>();
            }
            catch { Items = new List<HistoryItem>(); }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(Items);
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }
    }
}