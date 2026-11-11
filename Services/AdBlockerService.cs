using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AganBrowser.Services
{
    public class BlockedItem
    {
        public string Url { get; set; } = "";
        public string Domain { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;
    }

    public class AdBlockState
    {
        public bool IsEnabled { get; set; } = false;
    }

    public static class AdBlockerService
    {
        private static bool _isEnabled = false;
        public static bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                SaveState();
            }
        }

        public static int BlockedCount { get; private set; }
        public static List<BlockedItem> RecentBlocked { get; } = new();
        private const int MaxLogSize = 50;

        private static readonly HashSet<string> _whitelistDomains = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> _blockedDomains = new(StringComparer.OrdinalIgnoreCase);
        private static readonly List<Regex> _wildcardRules = new();
        private static readonly string _whitelistPath;
        private static readonly string _statePath;

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
            _statePath = Path.Combine(dir, "adblock_state.json");
            LoadWhitelist();
            LoadState();
        }

        public static void LoadRules()
        {
            _blockedDomains.Clear();
            _wildcardRules.Clear();

            string listsPath = Path.Combine(AppContext.BaseDirectory, "Assets", "AdblockLists");
            bool foundAnyList = false;

            if (Directory.Exists(listsPath))
            {
                foreach (var file in Directory.GetFiles(listsPath, "*.txt"))
                {
                    try
                    {
                        foreach (var line in File.ReadLines(file))
                        {
                            string rule = line.Trim();
                            if (string.IsNullOrEmpty(rule) || rule[0] == '!' || rule[0] == '[') continue;

                            if (rule.StartsWith("||") && rule.EndsWith("^") && !rule.Contains('$'))
                            {
                                string domain = rule.Substring(2, rule.Length - 3);
                                if (string.IsNullOrEmpty(domain)) continue;

                                if (domain.Contains('*'))
                                {
                                    string pattern = "(^|\\.)" + Regex.Escape(domain).Replace("\\*", ".*") + "$";
                                    _wildcardRules.Add(new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                                }
                                else
                                {
                                    _blockedDomains.Add(domain);
                                }
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
                    _blockedDomains.Add(domain);
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

                string host = uri.Host;
                string current = host;
                while (!string.IsNullOrEmpty(current))
                {
                    if (_blockedDomains.Contains(current))
                    {
                        RegisterBlock(url, uri.Host);
                        return true;
                    }

                    int dot = current.IndexOf('.');
                    if (dot < 0) break;
                    current = current.Substring(dot + 1);
                }

                if (_wildcardRules.Count > 0)
                {
                    foreach (var regex in _wildcardRules)
                    {
                        if (regex.IsMatch(host))
                        {
                            RegisterBlock(url, uri.Host);
                            return true;
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        private static void RegisterBlock(string url, string host)
        {
            BlockedCount++;
            RecentBlocked.Add(new BlockedItem { Url = url, Domain = host });
            if (RecentBlocked.Count > MaxLogSize) RecentBlocked.RemoveAt(0);
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
            return _whitelistDomains.Any(d =>
                domain.Equals(d, StringComparison.OrdinalIgnoreCase) ||
                domain.EndsWith("." + d, StringComparison.OrdinalIgnoreCase));
        }

        private static void SaveWhitelist()
        {
            try { File.WriteAllText(_whitelistPath, JsonSerializer.Serialize(_whitelistDomains)); } catch { }
        }

        private static void LoadWhitelist()
        {
            if (!File.Exists(_whitelistPath)) return;
            try
            {
                var list = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_whitelistPath));
                if (list != null) foreach (var d in list) _whitelistDomains.Add(d);
            }
            catch { }
        }

        private static void SaveState()
        {
            try { File.WriteAllText(_statePath, JsonSerializer.Serialize(new AdBlockState { IsEnabled = _isEnabled })); } catch { }
        }

        private static void LoadState()
        {
            if (!File.Exists(_statePath)) return;
            try
            {
                var state = JsonSerializer.Deserialize<AdBlockState>(File.ReadAllText(_statePath));
                if (state != null) _isEnabled = state.IsEnabled;
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

        public static void Reset()
        {
            BlockedCount = 0;
            RecentBlocked.Clear();
        }
    }
}
