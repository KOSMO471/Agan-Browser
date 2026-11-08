using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class PasswordService
    {
        private readonly string _filePath;
        private List<PasswordEntry> _entries = new();

        public PasswordService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "passwords.dat");
            Load();
        }

        public IReadOnlyList<PasswordEntry> Entries => _entries;

        public void Save(string site, string username, string password)
        {
            var existing = _entries.FirstOrDefault(e =>
                e.Site.Equals(site, StringComparison.OrdinalIgnoreCase) && e.Username == username);
            if (existing != null)
            {
                existing.Password = password;
            }
            else
            {
                _entries.Add(new PasswordEntry { Site = site, Username = username, Password = password });
            }
            Persist();
        }

        public void Delete(PasswordEntry entry)
        {
            _entries.Remove(entry);
            Persist();
        }

        public PasswordEntry? FindForSite(string site)
        {
            return _entries.FirstOrDefault(e =>
                e.Site.Equals(site, StringComparison.OrdinalIgnoreCase));
        }

        private void Load()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                byte[] encrypted = File.ReadAllBytes(_filePath);
                byte[] plain = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                string json = Encoding.UTF8.GetString(plain);
                _entries = JsonSerializer.Deserialize<List<PasswordEntry>>(json) ?? new();
            }
            catch { _entries = new(); }
        }

        private void Persist()
        {
            try
            {
                string json = JsonSerializer.Serialize(_entries);
                byte[] plain = Encoding.UTF8.GetBytes(json);
                byte[] encrypted = ProtectedData.Protect(plain, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(_filePath, encrypted);
            }
            catch { }
        }
    }
}