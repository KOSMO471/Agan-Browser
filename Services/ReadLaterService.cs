using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AganBrowser.Services
{
    public class ReadLaterItem
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public string FilePath { get; set; } = "";
        public DateTime SavedAt { get; set; } = DateTime.Now;
    }

    public class ReadLaterService
    {
        private readonly string _folder;
        private readonly string _indexPath;
        public List<ReadLaterItem> Items { get; } = new();

        public ReadLaterService()
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            _folder = Path.Combine(appData, "ReadLater");
            _indexPath = Path.Combine(_folder, "index.json");
            Directory.CreateDirectory(_folder);
            Load();
        }

        public string BuildPdfPath()
        {
            string fileName = $"{DateTime.Now:yyyyMMdd_HHmmss_fff}.pdf";
            return Path.Combine(_folder, fileName);
        }

        public ReadLaterItem Save(string title, string url, string pdfPath)
        {
            var item = new ReadLaterItem { Title = title, Url = url, FilePath = pdfPath };
            Items.Add(item);
            SaveIndex();
            return item;
        }

        private void SaveIndex()
        {
            try { File.WriteAllText(_indexPath, JsonSerializer.Serialize(Items)); } catch { }
        }

        private void Load()
        {
            if (!File.Exists(_indexPath)) return;
            try
            {
                var loaded = JsonSerializer.Deserialize<List<ReadLaterItem>>(File.ReadAllText(_indexPath));
                if (loaded != null)
                {
                    foreach (var item in loaded)
                        if (File.Exists(item.FilePath)) Items.Add(item);
                }
            }
            catch { }
        }
    }
}