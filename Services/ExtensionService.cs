using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class ExtensionService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public string BaseUrl { get; set; } = "https://your-username.github.io/extensions-repo/";

        public List<string> GetBuiltInExtensionNames()
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Extensions");
            if (!Directory.Exists(baseDir)) return new List<string>();
            return Directory.GetDirectories(baseDir)
                .Where(d => File.Exists(Path.Combine(d, "manifest.json")))
                .Select(d => Path.GetFileName(d)!)
                .ToList();
        }

        public async Task<string> InstallBuiltInAsync(WebView2 webView, string extensionName)
        {
            string root = Path.Combine(AppContext.BaseDirectory, "Assets", "Extensions", extensionName);
            if (!Directory.Exists(root))
                throw new DirectoryNotFoundException("Klasör bulunamadı: " + root);
            if (!File.Exists(Path.Combine(root, "manifest.json")))
                throw new FileNotFoundException("manifest.json bulunamadı: " + root);

            await webView.EnsureCoreWebView2Async();
            var ext = await webView.CoreWebView2.Profile.AddBrowserExtensionAsync(root);
            return ext.Id;
        }

        public async Task<ExtensionManifest[]> GetAvailableExtensionsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<ExtensionManifest[]>($"{BaseUrl}extensions.json");
                return result ?? Array.Empty<ExtensionManifest>();
            }
            catch { return Array.Empty<ExtensionManifest>(); }
        }

        public async Task<string> InstallFromFolderAsync(WebView2 webView, string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);
            if (!File.Exists(Path.Combine(folderPath, "manifest.json")))
                throw new FileNotFoundException("manifest.json bulunamadı: " + folderPath);

            await webView.EnsureCoreWebView2Async();
            var ext = await webView.CoreWebView2.Profile.AddBrowserExtensionAsync(folderPath);
            return ext.Id;
        }

        public async Task<string> InstallFromZipAsync(WebView2 webView, string zipPath)
        {
            if (!File.Exists(zipPath)) throw new FileNotFoundException(zipPath);

            string extractDir = Path.Combine(Path.GetTempPath(), "AganExtensions", Guid.NewGuid().ToString());
            Directory.CreateDirectory(extractDir);
            ZipFile.ExtractToDirectory(zipPath, extractDir);

            string root = extractDir;
            var entries = Directory.GetFileSystemEntries(extractDir);
            if (entries.Length == 1 && Directory.Exists(entries[0]))
                root = entries[0];

            if (!File.Exists(Path.Combine(root, "manifest.json")))
                throw new FileNotFoundException("manifest.json bulunamadı.");

            await webView.EnsureCoreWebView2Async();
            var ext = await webView.CoreWebView2.Profile.AddBrowserExtensionAsync(root);
            return ext.Id;
        }

        public async Task<string> InstallFromGitHubAsync(WebView2 webView, ExtensionManifest manifest)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "AganExtensions", manifest.Name);
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);

            byte[] bytes = await _httpClient.GetByteArrayAsync(manifest.DownloadUrl);
            string zipPath = Path.Combine(Path.GetTempPath(), $"{manifest.Name}.zip");
            await File.WriteAllBytesAsync(zipPath, bytes);
            ZipFile.ExtractToDirectory(zipPath, tempDir);
            File.Delete(zipPath);

            var entries = Directory.GetFileSystemEntries(tempDir);
            string root = tempDir;
            if (entries.Length == 1 && Directory.Exists(entries[0]))
                root = entries[0];

            await webView.EnsureCoreWebView2Async();
            var ext = await webView.CoreWebView2.Profile.AddBrowserExtensionAsync(root);
            return ext.Id;
        }
    }
}