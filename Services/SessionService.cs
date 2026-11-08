using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AganBrowser.Services
{
    public class SessionService
    {
        private readonly string _filePath;

        public SessionService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "session.json");
        }

        public void SaveSession(List<string> urls)
        {
            try
            {
                string json = JsonSerializer.Serialize(urls);
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }

        public List<string> LoadSession()
        {
            try
            {
                if (!File.Exists(_filePath)) return new List<string>();
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch { return new List<string>(); }
        }
    }
}