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

        public void Save(string title, string url, string htmlContent)
        {
            string fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}.html";
            string filePath = Path.Combine(_folder, fileName);
            
            string styledHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>{title}</title>
    <style>
        body {{ font-family: 'Georgia', serif; max-width: 720px; margin: 40px auto; padding: 20px; background: #FAF8F5; color: #1A1A1A; line-height: 1.75; font-size: 19px; }}
        h1 {{ font-size: 36px; line-height: 1.2; margin-bottom: 16px; }}
        a {{ color: #6366F1; }}
        img {{ max-width: 100%; height: auto; border-radius: 8px; margin: 20px 0; }}
        .meta {{ color: #666; font-family: 'Segoe UI', sans-serif; font-size: 14px; margin-bottom: 32px; }}
    </style>
</head>
<body>
    <h1>{title}</h1>
    <div class='meta'>Kaynak: <a href='{url}'>{url}</a> · Kaydedildi: {DateTime.Now:dd MMMM yyyy HH:mm}</div>
    {htmlContent}
</body>
</html>";

            File.WriteAllText(filePath, styledHtml);
            
            Items.Add(new ReadLaterItem { Title = title, Url = url, FilePath = filePath });
            SaveIndex();
        }

        public void Delete(ReadLaterItem item)
        {
            if (File.Exists(item.FilePath)) File.Delete(item.FilePath);
            Items.Remove(item);
            SaveIndex();
        }

        private void SaveIndex()
        {
            try
            {
                File.WriteAllText(_indexPath, JsonSerializer.Serialize(Items));
            }
            catch { }
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