using System;
using System.Collections.Generic;

namespace AganBrowser.Services
{
    public static class AdBlockerService
    {
        public static bool IsEnabled { get; set; } = true;
        public static int BlockedCount { get; private set; }

        private static readonly HashSet<string> BlockedDomains = new(StringComparer.OrdinalIgnoreCase)
        {
            "doubleclick.net", "googlesyndication.com", "googleadservices.com", "google-analytics.com",
            "googletagmanager.com", "googletagservices.com", "adservice.google.com", "pagead2.googlesyndication.com",
            "partner.googleadservices.com", "ads.yahoo.com", "adserver.yahoo.com", "analytics.yahoo.com",
            "facebook.net", "connect.facebook.net", "scorecardresearch.com", "outbrain.com", "taboola.com",
            "criteo.com", "criteo.net", "quantserve.com", "quantcount.com", "adnxs.com", "rubiconproject.com",
            "pubmatic.com", "amazon-adsystem.com", "advertising.com", "zedo.com", "moatads.com",
            "adsafeprotected.com", "imasdk.googleapis.com", "adroll.com", "mgid.com", "revcontent.com",
            "sharethrough.com", "triplelift.com", "smartadserver.com", "yieldmo.com", "casalemedia.com",
            "indexww.com", "sovrn.com", "openx.net", "contextweb.com", "bidswitch.net", "adform.net",
            "adsrvr.org", "tremorhub.com", "teads.tv", "spotxchange.com", "spotx.tv", "1rx.io",
            "krxd.net", "rlcdn.com", "bluekai.com", "demdex.net", "omtrdc.net", "everesttech.net",
            "hotjar.com", "fullstory.com", "inspectlet.com", "mouseflow.com", "crazyegg.com", "clarity.ms",
            "adcolony.com", "applovin.com", "unityads.unity3d.com", "vungle.com", "chartboost.com",
            "propellerads.com", "popads.net", "exoclick.com", "juicyads.com", "trafficjunky.net"
        };

        private static readonly string[] BlockedPathPatterns =
        {
            "/ads/", "/adserver/", "/adservice/", "/adframe/", "/adframe.",
            "/banner/", "/banners/", "/popup/", "/popunder/",
            "/tracking/", "/tracker/", "/analytics/", "/pixel/", "/beacon/",
            "/collect?", "/collect/", "/pagead/", "/ad_status.", "/adx/",
            "googleads.g.doubleclick.net", "/telemetry/", "/metrics/",
            "/sponsor/", "/sponsored/", "/promoted/", "/advert/", "/advertisement/"
        };

        public static bool ShouldBlock(string url)
        {
            if (!IsEnabled || string.IsNullOrEmpty(url)) return false;
            try
            {
                var uri = new Uri(url);
                if (uri.Scheme != "http" && uri.Scheme != "https") return false;

                string host = uri.Host;
                foreach (var domain in BlockedDomains)
                {
                    if (host.Equals(domain, StringComparison.OrdinalIgnoreCase) ||
                        host.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase))
                    {
                        BlockedCount++;
                        return true;
                    }
                }

                string path = uri.AbsolutePath.ToLowerInvariant();
                string query = uri.Query.ToLowerInvariant();
                foreach (var pattern in BlockedPathPatterns)
                {
                    if (path.Contains(pattern) || query.Contains(pattern))
                    {
                        BlockedCount++;
                        return true;
                    }
                }
                return false;
            }
            catch { return false; }
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