using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class BookmarkService
    {
        private readonly string _filePath;
        public ObservableCollection<BookmarkItem> Bookmarks { get; } = new();

        public BookmarkService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "bookmarks.json");
            Load();
        }

        public void Add(BookmarkItem item)
        {
            Bookmarks.Add(item);
            Save();
        }

        public void Remove(BookmarkItem item)
        {
            Bookmarks.Remove(item);
            Save();
        }

        public void Load()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                string json = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<BookmarkItem>>(json);
                if (list != null)
                {
                    Bookmarks.Clear();
                    foreach (var item in list) Bookmarks.Add(item);
                }
            }
            catch { }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(new List<BookmarkItem>(Bookmarks));
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }
    }
}