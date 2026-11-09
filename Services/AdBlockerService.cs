using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AganBrowser.Services
{
    public static class AdBlockerService
    {
        public static bool IsEnabled { get; set; } = true;
        public static int BlockedCount { get; private set; }

        private static readonly HashSet<string> _whitelistDomains = new(StringComparer.OrdinalIgnoreCase);
        private static readonly List<Regex> _blockedRules = new();
        private static readonly string _whitelistPath;

        private static readonly string[] _trackingParams = 
        { 
            "utm_source", "utm_medium", "utm_campaign", "utm_term", "utm_content", 
            "fbclid", "gclid", "yclid", "msclkid", "mc_eid", "igshid", "ref_src" 
        };

        private static readonly HashSet<string> _fallbackDomains = new(StringComparer.OrdinalIgnoreCase)
        {
            "doubleclick.net", "googlesyndication.com", "googleadservices.com", "google-analytics.com",
            "googletagmanager.com", "facebook.net", "scorecardresearch.com", "outbrain.com", "taboola.com",
            "criteo.com", "adnxs.com", "pubmatic.com", "amazon-adsystem.com", "zedo.com", "moatads.com"
        };

        static AdBlockerService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _whitelistPath = Path.Combine(dir, "adblock_allowlist.json");
            LoadWhitelist();
        }

        public static void LoadRules()
        {
            _blockedRules.Clear();
            string listsPath = Path.Combine(AppContext.BaseDirectory, "Assets", "AdblockLists");
            bool foundAnyList = false;

            if (Directory.Exists(listsPath))
            {
                var files = Directory.GetFiles(listsPath, "*.txt");
                foreach (var file in files)
                {
                    try
                    {
                        var lines = File.ReadAllLines(file);
                        foreach (var line in lines)
                        {
                            string rule = line.Trim();
                            if (string.IsNullOrWhiteSpace(rule) || rule.StartsWith("!") || rule.StartsWith("[")) continue;
                            
                            if (rule.StartsWith("||") && rule.EndsWith("^"))
                            {
                                string domain = rule.Substring(2, rule.Length - 3).Replace(".", "\\.");
                                if (domain.Contains("*")) domain = domain.Replace("*", ".*");
                                _blockedRules.Add(new Regex($"(^|\\.){domain}$", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                            }
                        }
                        foundAnyList = true;
                    }
                    catch { }
                }
            }

            if (!foundAnyList)
            {
                foreach (var domain in _fallbackDomains)
                {
                    string safeDomain = domain.Replace(".", "\\.");
                    _blockedRules.Add(new Regex($"(^|\\.){safeDomain}$", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                }
            }
        }

        public static bool ShouldBlock(string url, string resourceContext, string mainDomain)
        {
            if (!IsEnabled || string.IsNullOrEmpty(url)) return false;

            try
            {
                var uri = new Uri(url);
                if (uri.Scheme != "http" && uri.Scheme != "https") return false;

                if (IsWhitelisted(uri.Host)) return false;

                if (!string.IsNullOrEmpty(mainDomain) && !uri.Host.EndsWith(mainDomain, StringComparison.OrdinalIgnoreCase))
                {
                    if (resourceContext.Equals("Script", StringComparison.OrdinalIgnoreCase) || 
                        resourceContext.Equals("Iframe", StringComparison.OrdinalIgnoreCase))
                    {
                        BlockedCount++;
                        return true;
                    }
                }

                string host = uri.Host;
                foreach (var regex in _blockedRules)
                {
                    if (regex.IsMatch(host))
                    {
                        BlockedCount++;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static string CleanUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            
            try
            {
                var uri = new Uri(url);
                if (string.IsNullOrEmpty(uri.Query)) return url;

                var queryParams = uri.Query.TrimStart('?').Split('&');
                var newParams = new List<string>();
                bool changed = false;

                foreach (var param in queryParams)
                {
                    if (string.IsNullOrWhiteSpace(param)) continue;
                    
                    var parts = param.Split('=');
                    string key = parts[0];

                    if (_trackingParams.Contains(key, StringComparer.OrdinalIgnoreCase))
                    {
                        changed = true;
                        continue;
                    }
                    newParams.Add(param);
                }

                if (changed)
                {
                    var builder = new UriBuilder(uri);
                    builder.Query = string.Join("&", newParams);
                    return builder.ToString();
                }
            }
            catch { }
            return url;
        }

        public static void AddToWhitelist(string domain)
        {
            if (string.IsNullOrEmpty(domain)) return;
            _whitelistDomains.Add(domain);
            SaveWhitelist();
        }

        public static void RemoveFromWhitelist(string domain)
        {
            if (string.IsNullOrEmpty(domain)) return;
            _whitelistDomains.Remove(domain);
            SaveWhitelist();
        }

        public static bool IsWhitelisted(string domain)
        {
            if (string.IsNullOrEmpty(domain)) return false;
            return _whitelistDomains.Any(d => domain.EndsWith(d, StringComparison.OrdinalIgnoreCase));
        }

        private static void SaveWhitelist()
        {
            try
            {
                string json = JsonSerializer.Serialize(_whitelistDomains);
                File.WriteAllText(_whitelistPath, json);
            }
            catch { }
        }

        private static void LoadWhitelist()
        {
            if (!File.Exists(_whitelistPath)) return;
            try
            {
                string json = File.ReadAllText(_whitelistPath);
                var list = JsonSerializer.Deserialize<List<string>>(json);
                if (list != null)
                {
                    foreach (var d in list) _whitelistDomains.Add(d);
                }
            }
            catch { }
        }

        public static string GetCosmeticScript()
        {
            return @"
                (function() {
                    if (document.getElementById('agan-adblock-style')) return;
                    const style = document.createElement('style');
                    style.id = 'agan-adblock-style';
                    style.textContent = `
                        .ad, .ads, .ad-container, .ad-wrapper, .ad-slot, .ad-banner, .ad-box,
                        .adsbygoogle, .sponsored, .promoted, .advert, .advertisement,
                        #ad, #ads, #ad-container, #google_ads, #ad-slot, #ad-banner, #ad-box,
                        [id^='ad-'], [class^='ad-'], [id*='google_ads'], [class*='sponsored'],
                        [id*='banner'], [class*='banner-ad'], [class*='advert'],
                        iframe[src*='doubleclick'], iframe[src*='googleads'], iframe[src*='adsystem'],
                        ins.adsbygoogle, div[id^='div-gpt-ad'], div[data-ad-slot],
                        .ytp-ad-overlay-container, .ytp-ad-progress-list, #masthead-ad {
                            display: none !important;
                            width: 0 !important;
                            height: 0 !important;
                            opacity: 0 !important;
                            pointer-events: none !important;
                        }
                    `;
                    document.documentElement.appendChild(style);
                })();
            ";
        }

        public static void Reset() => BlockedCount = 0;
    }
}
