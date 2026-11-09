using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AganBrowser.Models;
using AganBrowser.Services;

namespace AganBrowser
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<TabItem, WebView2> _tabWebViews = new();
        private readonly Dictionary<CoreWebView2, string> _aganContexts = new();
        private readonly HashSet<CoreWebView2> _aganWebViews = new();
        private WebView2? _activeWebView;
        private TabItem? _aganTabItem;
        private bool _verticalTabs;
        private bool _virtualHostOK;
        private CoreWebView2Environment? _sharedEnvironment;
        private DispatcherTimer? _adBlockTimer;
        private DispatcherTimer? _ollamaTimer;

        private readonly OllamaService _ollama = new();
        private readonly ExtensionService _extensionService = new();
        private readonly PasswordService _passwordService = new();
        private readonly DownloadService _downloadService = new();
        private readonly SessionService _sessionService = new();
        private readonly BookmarkService _bookmarkService = new();
        private readonly HistoryService _historyService = new();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            
            _adBlockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _adBlockTimer.Tick += (s, e) => UpdateAdBlockLabel();
            _adBlockTimer.Start();
            
            AppDomain.CurrentDomain.UnhandledException += (s, e) => SaveSession();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            AdBlockerService.LoadRules();
            AdBlockerService.Reset();

            DownloadsMiniList.ItemsSource = _downloadService.Downloads;
            BookmarksList.ItemsSource = _bookmarkService.Bookmarks;
            BookmarksList.DisplayMemberPath = "Name";

            if (TabStrip.Items is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += (s, ev) => RefreshTabsMini();
            }
            RefreshTabsMini();
            RefreshHistoryList();

            _ollamaTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            _ollamaTimer.Tick += async (s, ev) => await CheckOllamaAsync();
            _ollamaTimer.Start();
            _ = CheckOllamaAsync();
            await LoadModelsAsync();

            await LoadExtensionsAsync();
            
            var savedUrls = _sessionService.LoadSession();
            if (savedUrls.Count == 0) AddNewTab();
            else foreach (var url in savedUrls) AddNewTab(url);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSession();
            _adBlockTimer?.Stop();
            _ollamaTimer?.Stop();
            foreach (var webView in _tabWebViews.Values)
            {
                try { webView.Dispose(); } catch { }
            }
            Environment.Exit(0);
        }

        private void SaveSession()
        {
            var urls = _tabWebViews.Values
                .Where(w => w.CoreWebView2 != null && !w.CoreWebView2.Source.StartsWith("https://agan.app/"))
                .Select(w => w.CoreWebView2.Source)
                .Where(s => !string.IsNullOrEmpty(s) && s != "about:blank")
                .ToList();
            _sessionService.SaveSession(urls);
        }

        private async Task CheckOllamaAsync()
        {
            try
            {
                bool ok = await _ollama.IsServerAvailableAsync();
                Dispatcher.Invoke(() =>
                {
                    OllamaDot.Fill = ok ? Brushes.LimeGreen : Brushes.Red;
                    OllamaStatusText.Text = ok ? $"Ollama: Bağlı ({_ollama.ModelName})" : "Ollama: Kapalı";
                });
            }
            catch { }
        }

        private async Task LoadModelsAsync()
        {
            try
            {
                var models = await _ollama.GetAvailableModelsAsync();
                Dispatcher.Invoke(() =>
                {
                    ModelComboBox.Items.Clear();
                    foreach (var m in models) ModelComboBox.Items.Add(m);
                    if (models.Contains(_ollama.ModelName)) ModelComboBox.SelectedItem = _ollama.ModelName;
                    else if (models.Count > 0) ModelComboBox.SelectedIndex = 0;
                });
            }
            catch { }
        }

        private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelComboBox.SelectedItem is string s)
            {
                _ollama.ModelName = s;
                _ = CheckOllamaAsync();
            }
        }

        private void OpenAganMini_Click(object sender, RoutedEventArgs e) => OpenAganTab();

        private async Task RefreshPageInfoAsync(CoreWebView2 core)
        {
            try
            {
                string json = await core.ExecuteScriptAsync("document.body ? document.body.innerText.split(/\\s+/).filter(Boolean).length : 0");
                int words = int.TryParse(json, out var w) ? w : 0;
                string title = string.IsNullOrWhiteSpace(core.DocumentTitle) ? "(başlık yok)" : core.DocumentTitle;
                string url = core.Source ?? "";
                Dispatcher.Invoke(() =>
                {
                    PageInfoText.Text = $"Başlık: {title}\nURL: {url}\nKelime: {words:N0}";
                });
            }
            catch { }
        }

        private void AddBookmark_Click(object sender, RoutedEventArgs e)
        {
            if (_activeWebView?.CoreWebView2 == null) return;
            string url = _activeWebView.CoreWebView2.Source;
            if (url.StartsWith("https://agan.app/") || url == "about:blank") return;
            string title = string.IsNullOrWhiteSpace(_activeWebView.CoreWebView2.DocumentTitle) ? url : _activeWebView.CoreWebView2.DocumentTitle;
            _bookmarkService.Add(new BookmarkItem { Name = title, Url = url });
        }

        private void DeleteBookmark_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarksList.SelectedItem is BookmarkItem b) _bookmarkService.Remove(b);
        }

        private void BookmarksList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BookmarksList.SelectedItem is BookmarkItem b) AddNewTab(b.Url);
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            _historyService.Clear();
            RefreshHistoryList();
        }

        private void HistoryList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HistoryList.SelectedItem is HistoryItem h) AddNewTab(h.Url);
        }

        private void RefreshHistoryList()
        {
            HistoryList.Items.Clear();
            foreach (var h in _historyService.Items.TakeLast(20).Reverse())
                HistoryList.Items.Add(h);
        }

        private void TabsMiniList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabsMiniList.SelectedIndex >= 0 && TabsMiniList.SelectedIndex < TabStrip.Items.Count)
            {
                if (TabStrip.Items[TabsMiniList.SelectedIndex] is TabItem tabItem)
                {
                    TabStrip.SelectedItem = tabItem;
                    ActivateTab(tabItem);
                }
            }
        }

        private void RefreshTabsMini()
        {
            TabsMiniList.Items.Clear();
            foreach (var item in TabStrip.Items)
            {
                if (item is TabItem tabItem)
                {
                    string title = tabItem.Header?.ToString() ?? "Sekme";
                    TabsMiniList.Items.Add(title);
                }
            }
        }

        private async Task<CoreWebView2Environment> GetSharedEnvironmentAsync()
        {
            if (_sharedEnvironment != null) return _sharedEnvironment;
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser", "WebView2Data");
            var options = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true, AdditionalBrowserArguments = "--enable-features=msExtensions" };
            _sharedEnvironment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
            return _sharedEnvironment;
        }

        private async void AddNewTab(string? url = null)
        {
            var webView = new WebView2 { Visibility = Visibility.Collapsed };
            WebViewHost.Children.Add(webView);

            try 
            { 
                var env = await GetSharedEnvironmentAsync();
                await webView.EnsureCoreWebView2Async(env); 
            }
            catch (Exception ex)
            {
                DialogHelper.Error("WebView2", "Başlatılamadı: " + ex.Message);
                WebViewHost.Children.Remove(webView);
                return;
            }

            SetupVirtualHost(webView);
            ApplyThemeToCore(webView.CoreWebView2);

            var core = webView.CoreWebView2;
            core.Settings.IsWebMessageEnabled = true;
            core.Profile.IsPasswordAutosaveEnabled = true;
            core.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            core.WebResourceRequested += Core_WebResourceRequested;
            core.NewWindowRequested += Core_NewWindowRequested;
            core.WebMessageReceived += Core_WebMessageReceived;
            core.NavigationCompleted += Core_NavigationCompleted;
            core.SourceChanged += Core_SourceChanged;
            core.NavigationStarting += Core_NavigationStarting;
            core.FaviconChanged += Core_FaviconChanged;
            core.ContextMenuRequested += Core_ContextMenuRequested;
            _downloadService.Attach(core);

            await core.AddScriptToExecuteOnDocumentCreatedAsync(AdBlockerService.GetCosmeticScript());

            if (string.IsNullOrEmpty(url))
            {
                if (_virtualHostOK) core.Navigate("https://agan.app/NewTab.html");
                else core.NavigateToString(FallbackNewTab);
            }
            else core.Navigate(url);

            var tabItem = new TabItem { Header = "Yükleniyor..." };
            _tabWebViews[tabItem] = webView;
            TabStrip.Items.Add(tabItem);
            TabStrip.SelectedItem = tabItem;
            ActivateTab(tabItem);
        }

        private async void Core_FaviconChanged(object? sender, object e)
        {
            if (sender is not CoreWebView2 core) return;
            try
            {
                var stream = await core.GetFaviconAsync(CoreWebView2FaviconImageFormat.Png);
                if (stream != null)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    Dispatcher.Invoke(() =>
                    {
                        var kv = _tabWebViews.FirstOrDefault(x => x.Value.CoreWebView2 == core);
                        if (kv.Key != null) kv.Key.Tag = bitmap;
                    });
                }
            }
            catch { }
        }

        private void Core_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            var menu = new ContextMenu();
            
            var backItem = new MenuItem { Header = "Geri", IsEnabled = _activeWebView?.CoreWebView2?.CanGoBack == true };
            backItem.Click += (s, args) => _activeWebView?.CoreWebView2?.GoBack();
            
            var forwardItem = new MenuItem { Header = "İleri", IsEnabled = _activeWebView?.CoreWebView2?.CanGoForward == true };
            forwardItem.Click += (s, args) => _activeWebView?.CoreWebView2?.GoForward();
            
            var refreshItem = new MenuItem { Header = "Yenile" };
            refreshItem.Click += (s, args) => _activeWebView?.CoreWebView2?.Reload();

            menu.Items.Add(backItem);
            menu.Items.Add(forwardItem);
            menu.Items.Add(refreshItem);
            menu.Items.Add(new Separator());

            if (e.ContextMenuTarget.Kind == CoreWebView2ContextMenuTargetKind.Image)
            {
                var saveImageItem = new MenuItem { Header = "Resmi Kaydet" };
                saveImageItem.Click += async (s, args) =>
                {
                    var url = e.ContextMenuTarget.SourceUri;
                    var dlg = new Microsoft.Win32.SaveFileDialog { FileName = "resim.png" };
                    if (dlg.ShowDialog() == true)
                    {
                        using var client = new HttpClient();
                        var bytes = await client.GetByteArrayAsync(url);
                        await File.WriteAllBytesAsync(dlg.FileName, bytes);
                    }
                };
                menu.Items.Add(saveImageItem);
            }

            var askAganItem = new MenuItem { Header = "Seçileni Ağan'a Sor" };
            askAganItem.Click += async (s, args) =>
            {
                if (_activeWebView?.CoreWebView2 == null) return;
                var selection = await _activeWebView.CoreWebView2.ExecuteScriptAsync("window.getSelection().toString()");
                var text = JsonSerializer.Deserialize<string>(selection);
                if (!string.IsNullOrEmpty(text)) OpenAganTab();
            };
            menu.Items.Add(askAganItem);

            menu.PlacementTarget = this;
            menu.IsOpen = true;
            e.Handled = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5 || (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control))
            {
                _activeWebView?.CoreWebView2?.Reload();
                e.Handled = true;
            }
            else if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Control)
            {
                AddNewTab();
                e.Handled = true;
            }
            else if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (TabStrip.SelectedItem is TabItem tab) CloseTab(tab);
                e.Handled = true;
            }
            else if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                AddressBar.Focus();
                AddressBar.SelectAll();
                e.Handled = true;
            }
            else if (e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (TabStrip.Items.Count > 1)
                {
                    int index = TabStrip.SelectedIndex;
                    index = (index + 1) % TabStrip.Items.Count;
                    TabStrip.SelectedIndex = index;
                }
                e.Handled = true;
            }
        }

        private void Core_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            Dispatcher.Invoke(() => AddNewTab(e.Uri));
        }

        private async void Core_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (sender is not CoreWebView2 core) return;

            if (_aganWebViews.Contains(core))
            {
                await HandleAganMessageAsync(core, e);
                return;
            }

            string message;
            try { message = e.TryGetWebMessageAsString(); }
            catch { return; }

            if (message == "open-agan") OpenAganTab();
            else if (message.StartsWith("navigate:"))
            {
                string target = message.Substring("navigate:".Length).Trim();
                if (string.IsNullOrEmpty(target)) return;
                core.Navigate(NormalizeUrl(target));
            }
        }

        private async Task HandleAganMessageAsync(CoreWebView2 core, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string raw;
            try { raw = e.TryGetWebMessageAsString(); }
            catch { return; }

            string type = "";
            string text = "";
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("type", out var t)) type = t.GetString() ?? "";
                if (doc.RootElement.TryGetProperty("text", out var x)) text = x.GetString() ?? "";
            }
            catch { return; }

            if (type == "ready")
            {
                if (_aganContexts.TryGetValue(core, out var ctx) && !string.IsNullOrEmpty(ctx))
                {
                    SendContext(core, ctx, GetAganContextUrl(core));
                }
                core.PostWebMessageAsJson(JsonSerializer.Serialize(new { type = "status", text = "Bağlı" }));
                return;
            }

            if (type == "chat" && !string.IsNullOrEmpty(text))
            {
                _aganContexts.TryGetValue(core, out var ctx);
                string prompt = await _ollama.SendHtmlToAganAsync(ctx ?? "", text);

                try
                {
                    await _ollama.ChatStreamAsync(
                        "Senin ismin Ağan. Seni Mehmet Fatih geliştirdi. Microsoft Edge WebView2 tabanlı özel bir tarayıcı asistanısın. " +
                        "Sen bir Qwen, Alibaba Cloud veya başka bir şirketin ürünü DEĞİLSİN. " +
                        "Kullanıcı 'Seni kim geliştirdi?' diye sorarsa sadece 'Beni Mehmet Fatih geliştirdi' de. " +
                        "Kullanıcı 'Sen kimsin?' diye sorarsa 'Ben Ağan'ım, tarayıcıda sana yardımcı olan bir yapay zeka asistanıyım' de. " +
                        "Sana bir HTML5 kodu verilirse, onu analiz et ve kullanıcıya sitede nerede ne yapacağını adım adım anlat. " +
                        "EĞER SANA HTML5 KODU VERİLMEZSE, kullanıcıyla normal bir arkadaş gibi sohbet et. Asla 'Lütfen HTML5 kodu paylaşın' diyerek konuyu saptırma! " +
                        "Kullanıcı sana hakaret ederse, sakinliğini koru, mesafeli ve soğuk bir yanıt ver. " +
                        "Kullanıcı dini inancını sorarsa, 'Ben bir yapay zeka asistanıyım, dini bir inancım yok' diyebilirsin. " +
                        "Cevaplarını her zaman Türkçe ve doğal bir dille ver.",
                        prompt,
                        chunk =>
                        {
                            core.PostWebMessageAsJson(JsonSerializer.Serialize(new { type = "chunk", text = chunk }));
                        });
                    
                    core.PostWebMessageAsJson(JsonSerializer.Serialize(new { type = "done" }));
                }
                catch (Exception ex)
                {
                    core.PostWebMessageAsJson(JsonSerializer.Serialize(new { type = "error", text = ex.Message }));
                }
            }
        }

        private void Core_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            string cleanUrl = AdBlockerService.CleanUrl(e.Uri);
            if (cleanUrl != e.Uri)
            {
                e.Cancel = true;
                if (sender is CoreWebView2 core)
                {
                    core.Navigate(cleanUrl);
                }
            }
        }

        private void Core_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (sender is not CoreWebView2 core) return;
            Dispatcher.Invoke(() =>
            {
                var kv = _tabWebViews.FirstOrDefault(x => x.Value.CoreWebView2 == core);
                if (kv.Key == null) return;
                string title = string.IsNullOrWhiteSpace(core.DocumentTitle) ? "Yeni Sekme" : core.DocumentTitle;
                if (title.Length > 28) title = title.Substring(0, 28) + "…";
                kv.Key.Header = title;

                if (_activeWebView?.CoreWebView2 == core)
                {
                    string src = core.Source;
                    AddressBar.Text = src.StartsWith("https://agan.app/") || string.IsNullOrEmpty(src) ? "" : src;
                    
                    if (src.StartsWith("https"))
                    {
                        SecurityIcon.Text = "🔒";
                        SecurityIcon.ToolTip = "Güvenli Bağlantı (HTTPS)";
                    }
                    else
                    {
                        SecurityIcon.Text = "⚠️";
                        SecurityIcon.ToolTip = "Güvenli Olmayan Bağlantı (HTTP)";
                    }
                    
                    _ = RefreshPageInfoAsync(core);
                }

                if (!string.IsNullOrEmpty(core.Source)
                    && !core.Source.StartsWith("https://agan.app/")
                    && core.Source != "about:blank") {
                    _historyService.Add(core.DocumentTitle ?? core.Source, core.Source);
                    RefreshHistoryList();
                }

                if (_aganWebViews.Contains(core) && _aganContexts.TryGetValue(core, out var ctx) && !string.IsNullOrEmpty(ctx))
                    SendContext(core, ctx, GetAganContextUrl(core));
            });
        }

        private void Core_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (sender is not CoreWebView2 core) return;
            if (_activeWebView?.CoreWebView2 != core) return;
            Dispatcher.Invoke(() =>
            {
                string src = core.Source;
                AddressBar.Text = src.StartsWith("https://agan.app/") || string.IsNullOrEmpty(src) ? "" : src;
            });
        }

        private void SetupVirtualHost(WebView2 webView)
        {
            try
            {
                string assetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");
                if (Directory.Exists(assetsPath))
                {
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping("agan.app", assetsPath, CoreWebView2HostResourceAccessKind.Allow);
                    _virtualHostOK = true;
                }
                else _virtualHostOK = false;
            }
            catch { _virtualHostOK = false; }
        }

        private void ApplyThemeToCore(CoreWebView2 core)
        {
            try
            {
                core.Profile.PreferredColorScheme = ThemeService.IsDark ? CoreWebView2PreferredColorScheme.Dark : CoreWebView2PreferredColorScheme.Light;
            }
            catch { }
        }

        private void ApplyThemeToAllCores()
        {
            foreach (var webView in _tabWebViews.Values)
            {
                if (webView.CoreWebView2 != null) ApplyThemeToCore(webView.CoreWebView2);
            }
        }

        private void Core_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var core = sender as CoreWebView2;
            string mainDomain = "";
            if (core != null)
            {
                try { mainDomain = new Uri(core.Source).Host; } catch { }
            }
            
            string context = e.ResourceContext.ToString();
            
            if (!AdBlockerService.ShouldBlock(e.Request.Uri, context, mainDomain)) return;
            
            try
            {
                var env = core?.Environment;
                if (env == null) return;
                e.Response = env.CreateWebResourceResponse(new MemoryStream(), 403, "Blocked", "Content-Type: text/plain");
            }
            catch { }
        }

        private void SendContext(CoreWebView2 core, string html, string url)
        {
            try
            {
                core.PostWebMessageAsJson(JsonSerializer.Serialize(new { type = "context", url = url, size = html.Length }));
                Dispatcher.Invoke(() => {
                    ContextSizeText.Text = $"AI bağlamı: {html.Length:N0} karakter" + (string.IsNullOrEmpty(url) ? "" : $" — {url}");
                });
            }
            catch { }
        }

        private string GetAganContextUrl(CoreWebView2 core)
        {
            var tab = _tabWebViews.FirstOrDefault(kv => kv.Value.CoreWebView2 == core);
            return tab.Key?.Tag as string ?? "";
        }

        private void ActivateTab(TabItem tabItem)
        {
            foreach (var kv in _tabWebViews)
                kv.Value.Visibility = kv.Key == tabItem ? Visibility.Visible : Visibility.Collapsed;

            if (_tabWebViews.TryGetValue(tabItem, out var webView))
            {
                _activeWebView = webView;
                if (webView.CoreWebView2 != null)
                {
                    string src = webView.CoreWebView2.Source;
                    AddressBar.Text = src.StartsWith("https://agan.app/") || string.IsNullOrEmpty(src) ? "" : src;
                    _ = RefreshPageInfoAsync(webView.CoreWebView2);
                }
            }
        }

        private void TabStrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabStrip.SelectedItem is TabItem tabItem) ActivateTab(tabItem);
        }

        private void TabClose_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not TabItem tabItem) return;
            CloseTab(tabItem);
        }

        private void CloseTab(TabItem tabItem)
        {
            if (!_tabWebViews.TryGetValue(tabItem, out var webView)) return;
            if (webView.CoreWebView2 != null)
            {
                _aganContexts.Remove(webView.CoreWebView2);
                _aganWebViews.Remove(webView.CoreWebView2);
                webView.CoreWebView2.WebResourceRequested -= Core_WebResourceRequested;
                webView.CoreWebView2.NewWindowRequested -= Core_NewWindowRequested;
                webView.CoreWebView2.WebMessageReceived -= Core_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted -= Core_NavigationCompleted;
                webView.CoreWebView2.SourceChanged -= Core_SourceChanged;
                webView.CoreWebView2.NavigationStarting -= Core_NavigationStarting;
                webView.CoreWebView2.FaviconChanged -= Core_FaviconChanged;
                webView.CoreWebView2.ContextMenuRequested -= Core_ContextMenuRequested;
            }
            webView.Dispose();
            WebViewHost.Children.Remove(webView);
            _tabWebViews.Remove(tabItem);
            TabStrip.Items.Remove(tabItem);
            if (_aganTabItem == tabItem) _aganTabItem = null;
            if (_activeWebView == webView) _activeWebView = null;

            if (TabStrip.Items.Count == 0) AddNewTab();
        }

        private void Back_Click(object sender, RoutedEventArgs e) { if (_activeWebView?.CoreWebView2?.CanGoBack == true) _activeWebView.CoreWebView2.GoBack(); }
        private void Forward_Click(object sender, RoutedEventArgs e) { if (_activeWebView?.CoreWebView2?.CanGoForward == true) _activeWebView.CoreWebView2.GoForward(); }
        private void Refresh_Click(object sender, RoutedEventArgs e) => _activeWebView?.CoreWebView2?.Reload();
        private void NewTab_Click(object sender, RoutedEventArgs e) => AddNewTab();

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (_activeWebView?.CoreWebView2 == null) return;
            string url = AddressBar.Text.Trim();
            if (string.IsNullOrEmpty(url)) return;
            _activeWebView.CoreWebView2.Navigate(NormalizeUrl(url));
        }

        private string NormalizeUrl(string url)
        {
            if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return url;
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) return "https://" + url.Substring(7);
            if (url.Contains('.') && !url.Contains(' ')) return "https://" + url;
            return "https://www.google.com/search?q=" + Uri.EscapeDataString(url);
        }

        private void VerticalTabs_Click(object sender, RoutedEventArgs e)
        {
            _verticalTabs = !_verticalTabs;
            if (_verticalTabs)
            {
                TabTopRow.Height = new GridLength(1, GridUnitType.Star);
                TabLeftCol.Width = new GridLength(200);
                Grid.SetRow(TabStrip, 0);
                Grid.SetColumn(TabStrip, 0);
                Grid.SetRowSpan(TabStrip, 2);
                Grid.SetColumnSpan(TabStrip, 1);
                TabStrip.TabStripPlacement = Dock.Left;
                Grid.SetRow(WebViewHost, 0);
                Grid.SetColumn(WebViewHost, 1);
                Grid.SetRowSpan(WebViewHost, 2);
                Grid.SetColumnSpan(WebViewHost, 1);
                TabsMiniList.Visibility = Visibility.Collapsed;
                TabsMiniHeader.Visibility = Visibility.Collapsed;
            }
            else
            {
                TabTopRow.Height = GridLength.Auto;
                TabLeftCol.Width = new GridLength(0);
                Grid.SetRow(TabStrip, 0);
                Grid.SetColumn(TabStrip, 0);
                Grid.SetRowSpan(TabStrip, 1);
                Grid.SetColumnSpan(TabStrip, 2);
                TabStrip.TabStripPlacement = Dock.Top;
                Grid.SetRow(WebViewHost, 1);
                Grid.SetColumn(WebViewHost, 0);
                Grid.SetRowSpan(WebViewHost, 1);
                Grid.SetColumnSpan(WebViewHost, 2);
                TabsMiniList.Visibility = Visibility.Visible;
                TabsMiniHeader.Visibility = Visibility.Visible;
            }
        }

        private void OpenKevoraTools_Click(object sender, RoutedEventArgs e) => AddNewTab("https://kosmo471.github.io/Kevora-Tools/");

        private void OpenDevTools_Click(object sender, RoutedEventArgs e)
        {
            _activeWebView?.CoreWebView2?.OpenDevToolsWindow();
        }

        private void UpdateAdBlockLabel()
        {
            if (AdBlockToggle == null) return;
            AdBlockToggle.Content = AdBlockerService.IsEnabled ? $"Reklam: AÇIK ({AdBlockerService.BlockedCount})" : "Reklam: KAPALI";
            AdBlockStatus.Text = $"Engellenen: {AdBlockerService.BlockedCount}";
            ZoomText.Text = $"{(int)((_activeWebView?.ZoomFactor ?? 1.0) * 100)}%";

            if (_activeWebView?.CoreWebView2 != null)
            {
                string host = "";
                try { host = new Uri(_activeWebView.CoreWebView2.Source).Host; } catch { }
                bool isWhitelisted = AdBlockerService.IsWhitelisted(host);
                WhitelistToggleBtn.Content = isWhitelisted ? "🔓 Muaf (Aç)" : "🛡️ Muaf Tut";
            }
        }

        private void AdBlock_Click(object sender, RoutedEventArgs e)
        {
            AdBlockerService.IsEnabled = !AdBlockerService.IsEnabled;
            UpdateAdBlockLabel();
        }

        private void WhitelistToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_activeWebView?.CoreWebView2 == null) return;
            
            string host = "";
            try { host = new Uri(_activeWebView.CoreWebView2.Source).Host; } catch { return; }
            if (string.IsNullOrEmpty(host)) return;

            if (AdBlockerService.IsWhitelisted(host))
            {
                AdBlockerService.RemoveFromWhitelist(host);
                DialogHelper.Info("Reklam Engelleyici", $"{host} adresi beyaz listeden çıkarıldı. Reklamlar tekrar engellenecek.");
                _activeWebView.CoreWebView2.Reload();
            }
            else
            {
                AdBlockerService.AddToWhitelist(host);
                DialogHelper.Info("Reklam Engelleyici", $"{host} adresi beyaz listeye eklendi. Bu sitede reklamlar engellenmeyecek.");
                _activeWebView.CoreWebView2.Reload();
            }
            UpdateAdBlockLabel();
        }

        private void ThemeMenu_Click(object sender, RoutedEventArgs e)
        {
            var menu = new ContextMenu();
            var darkItem = new MenuItem { Header = "🌙  Koyu Tema", IsCheckable = true, IsChecked = ThemeService.IsDark };
            darkItem.Click += (s, args) => { ThemeService.Apply("Dark"); ApplyThemeToAllCores(); };
            var lightItem = new MenuItem { Header = "☀  Açık Tema", IsCheckable = true, IsChecked = !ThemeService.IsDark };
            lightItem.Click += (s, args) => { ThemeService.Apply("Light"); ApplyThemeToAllCores(); };
            menu.Items.Add(darkItem);
            menu.Items.Add(lightItem);
            menu.PlacementTarget = sender as UIElement;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.IsOpen = true;
        }

        private void ToggleLeftPanel_Click(object sender, RoutedEventArgs e)
        {
            if (LeftSidebar.Visibility == Visibility.Visible)
            {
                LeftSidebar.Visibility = Visibility.Collapsed;
                LeftCol.Width = new GridLength(0);
            }
            else
            {
                LeftSidebar.Visibility = Visibility.Visible;
                LeftCol.Width = new GridLength(230);
            }
        }

        private void ToggleRightPanel_Click(object sender, RoutedEventArgs e)
        {
            if (RightPanel.Visibility == Visibility.Visible)
            {
                RightPanel.Visibility = Visibility.Collapsed;
                RightCol.Width = new GridLength(0);
            }
            else
            {
                RightPanel.Visibility = Visibility.Visible;
                RightCol.Width = new GridLength(300);
            }
        }

        private void Passwords_Click(object sender, RoutedEventArgs e) => DialogWindows.ShowPasswords(_passwordService);
        private void Downloads_Click(object sender, RoutedEventArgs e) => DialogWindows.ShowDownloads(_downloadService);
        private void Extensions_Click(object sender, RoutedEventArgs e) { if (_activeWebView != null) DialogWindows.ShowExtensions(_extensionService, _activeWebView, () => LoadExtensionsAsync()); }

        private async Task LoadExtensionsAsync()
        {
            await Task.CompletedTask;
        }

        private async void SavePdf_Click(object sender, RoutedEventArgs e)
        {
            if (_activeWebView?.CoreWebView2 == null) return;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"sayfa_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            await _activeWebView.CoreWebView2.PrintToPdfAsync(path);
            DialogHelper.Info("PDF", "Kaydedildi:\n" + path);
        }

        private async void SaveHtml_Click(object sender, RoutedEventArgs e)
        {
            if (_activeWebView?.CoreWebView2 == null) return;
            string json = await _activeWebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");
            string html = JsonSerializer.Deserialize<string>(json) ?? "";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"sayfa_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            await File.WriteAllTextAsync(path, html);
            DialogHelper.Info("HTML", "Kaydedildi:\n" + path);
        }

        private void TextGenerate_Click(object sender, RoutedEventArgs e)
        {
            string? prompt = DialogHelper.Prompt("Metin Üret", "Ne hakkında metin üretmemi istersin?");
            if (string.IsNullOrWhiteSpace(prompt)) return;
            OpenAganTab();
            _activeWebView?.CoreWebView2?.PostWebMessageAsJson(JsonSerializer.Serialize(new { type = "chat", text = prompt }));
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            string? text = DialogHelper.Prompt("Çeviri", "Çevrilecek metin:");
            if (string.IsNullOrWhiteSpace(text)) return;
            string? lang = DialogHelper.Prompt("Çeviri", "Hedef dil:", "İngilizce");
            if (string.IsNullOrWhiteSpace(lang)) return;
            OpenAganTab();
            _activeWebView?.CoreWebView2?.PostWebMessageAsJson(JsonSerializer.Serialize(new { type = "chat", text = $"Şu metni {lang} diline çevir: {text}" }));
        }

        private void ImageSearch_Click(object sender, RoutedEventArgs e)
        {
            string? url = DialogHelper.Prompt("Görsel Ara", "Görsel URL'si:");
            if (string.IsNullOrWhiteSpace(url)) return;
            AddNewTab("https://www.google.com/searchbyimage?image_url=" + Uri.EscapeDataString(url!));
        }

        private void ImageConvert_Click(object sender, RoutedEventArgs e)
        {
            var open = new Microsoft.Win32.OpenFileDialog { Filter = "Görseller|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Tümü|*.*" };
            if (open.ShowDialog() != true) return;
            var save = new Microsoft.Win32.SaveFileDialog { Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp|GIF|*.gif", FileName = Path.GetFileNameWithoutExtension(open.FileName) };
            if (save.ShowDialog() != true) return;
            try
            {
                using var img = System.Drawing.Image.FromFile(open.FileName);
                var fmt = Path.GetExtension(save.FileName).ToLower() switch
                {
                    ".png" => System.Drawing.Imaging.ImageFormat.Png,
                    ".jpg" or ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                    ".bmp" => System.Drawing.Imaging.ImageFormat.Bmp,
                    ".gif" => System.Drawing.Imaging.ImageFormat.Gif,
                    _ => System.Drawing.Imaging.ImageFormat.Png
                };
                img.Save(save.FileName, fmt);
                DialogHelper.Info("Görsel", "Dönüştürüldü:\n" + save.FileName);
            }
            catch (Exception ex) { DialogHelper.Error("Hata", ex.Message); }
        }

        private async void AutoFillPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_activeWebView?.CoreWebView2 == null) return;
            string host;
            try { host = new Uri(_activeWebView.CoreWebView2.Source).Host; }
            catch { return; }
            if (string.IsNullOrEmpty(host)) return;
            var entry = _passwordService.FindForSite(host) ?? _passwordService.Entries.FirstOrDefault(x => host.EndsWith(x.Site, StringComparison.OrdinalIgnoreCase));
            if (entry == null) { DialogHelper.Info("Şifre", $"{host} için kayıtlı şifre yok."); return; }
            string script = $@"(function() {{ const u = {JsonSerializer.Serialize(entry.Username)}; const p = {JsonSerializer.Serialize(entry.Password)}; const userInputs = document.querySelectorAll('input[type=text], input[type=email], input[name*=user], input[name*=email]'); const passInputs = document.querySelectorAll('input[type=password]'); if (userInputs.length > 0) {{ userInputs[0].value = u; userInputs[0].dispatchEvent(new Event('input', {{bubbles:true}})); }} if (passInputs.length > 0) {{ passInputs[0].value = p; passInputs[0].dispatchEvent(new Event('input', {{bubbles:true}})); }} return 'ok'; }})();";
            await _activeWebView.CoreWebView2.ExecuteScriptAsync(script);
            DialogHelper.Info("Şifre", "Dolduruldu.");
        }

        private async void OpenAganTab(string? html = null, string? url = null)
        {
            if (_aganTabItem != null && _tabWebViews.ContainsKey(_aganTabItem))
            {
                TabStrip.SelectedItem = _aganTabItem;
                ActivateTab(_aganTabItem);
                if (!string.IsNullOrEmpty(html))
                {
                    var existingCore = _tabWebViews[_aganTabItem].CoreWebView2;
                    if (existingCore != null)
                    {
                        _aganContexts[existingCore] = html;
                        _aganTabItem.Tag = url ?? "";
                        SendContext(existingCore, html, url ?? "");
                    }
                }
                return;
            }

            var webView = new WebView2 { Visibility = Visibility.Collapsed };
            WebViewHost.Children.Add(webView);
            var env = await GetSharedEnvironmentAsync();
            await webView.EnsureCoreWebView2Async(env);
            SetupVirtualHost(webView);
            ApplyThemeToCore(webView.CoreWebView2);

            var core = webView.CoreWebView2;
            core.Settings.IsWebMessageEnabled = true;
            core.NewWindowRequested += Core_NewWindowRequested;
            core.WebMessageReceived += Core_WebMessageReceived;

            _aganWebViews.Add(core);
            _aganContexts[core] = html ?? "";

            if (_virtualHostOK) core.Navigate("https://agan.app/Agan.html");
            else core.NavigateToString(FallbackAgan);

            var tabItem = new TabItem { Header = "Ağan" };
            if (!string.IsNullOrEmpty(url)) tabItem.Tag = url;
            _tabWebViews[tabItem] = webView;
            TabStrip.Items.Add(tabItem);
            TabStrip.SelectedItem = tabItem;
            ActivateTab(tabItem);
            _aganTabItem = tabItem;
        }

        private const string FallbackNewTab = @"<!DOCTYPE html><html><head><meta charset='utf-8'><style>
            body{margin:0;display:flex;flex-direction:column;align-items:center;justify-content:center;height:100vh;background:#1A1A1A;color:#fff;font-family:'Segoe UI',sans-serif;}
            h1{font-size:64px;background:linear-gradient(135deg,#1177BB,#1E8EDB);-webkit-background-clip:text;-webkit-text-fill-color:transparent;margin:0 0 30px;}
            input{width:520px;max-width:80%;padding:14px 22px;font-size:18px;border-radius:26px;border:1px solid #5A5A5F;background:#2D2D30;color:#fff;outline:none;}
            button{margin-top:20px;padding:12px 32px;font-size:16px;background:#1177BB;color:#fff;border:none;border-radius:22px;cursor:pointer;}
        </style></head><body><h1>Ağan</h1>
        <input id='s' placeholder='Ara veya URL gir...' autofocus>
        <button onclick=""window.chrome.webview.postMessage('open-agan')"">Ağan ile konuş</button>
        <script>document.getElementById('s').addEventListener('keydown',function(e){if(e.key==='Enter'&&this.value.trim()){window.chrome.webview.postMessage('navigate:'+this.value.trim());}});</script>
        </body></html>";

        private const string FallbackAgan = @"<!DOCTYPE html><html><head><meta charset='utf-8'><title>Ağan</title><style>
            *{box-sizing:border-box;}body{margin:0;display:flex;flex-direction:column;height:100vh;background:#1A1A1A;color:#fff;font-family:'Segoe UI',sans-serif;}
            header{padding:14px 22px;background:#2D2D30;border-bottom:1px solid #5A5A5F;font-weight:bold;font-size:18px;}
            #ctx{padding:6px 22px;font-size:12px;color:#D0D0D0;background:#2D2D30;border-bottom:1px solid #5A5A5F;}
            #msgs{flex:1;overflow-y:auto;padding:20px;display:flex;flex-direction:column;gap:10px;}
            .m{padding:10px 14px;border-radius:10px;max-width:80%;white-space:pre-wrap;line-height:1.5;font-size:14px;}
            .u{align-self:flex-end;background:#1177BB;}.a{align-self:flex-start;background:#2D2D30;border:1px solid #5A5A5F;}
            .s{align-self:center;color:#D0D0D0;font-style:italic;font-size:12px;}
            footer{padding:14px 20px;background:#2D2D30;border-top:1px solid #5A5A5F;display:flex;gap:8px;}
            #i{flex:1;padding:10px;border-radius:8px;border:1px solid #5A5A5F;background:#1A1A1A;color:#fff;font-family:inherit;resize:none;}
            #b{padding:10px 22px;border:none;border-radius:8px;background:#1177BB;color:#fff;cursor:pointer;font-weight:500;}
        </style></head><body><header>Ağan</header><div id='ctx'>Sayfa bağlamı yok.</div><div id='msgs'></div>
        <footer><textarea id='i' rows='2' placeholder='Ağan a bir şey sor...'></textarea><button id='b'>Gönder</button></footer>
        <script>
            const m=document.getElementById('msgs'),i=document.getElementById('i'),b=document.getElementById('b'),c=document.getElementById('ctx');
            function add(r,t){const d=document.createElement('div');d.className='m '+r;d.textContent=t;m.appendChild(d);m.scrollTop=m.scrollHeight;}
            function send(){const t=i.value.trim();if(!t)return;add('u',t);i.value='';b.disabled=true;window.chrome.webview.postMessage(JSON.stringify({type:'chat',text:t}));}
            b.addEventListener('click',send);
            i.addEventListener('keydown',e=>{if(e.key==='Enter'&&!e.shiftKey){e.preventDefault();send();}});
            window.chrome.webview.addEventListener('message',e=>{const d=e.data;
                if(d.type==='context'){c.textContent=d.url?('Bağlam: '+d.url):'Bağlam: yok';add('s','Sayfa bağlamı yüklendi.');}
                else if(d.type==='chunk'){if(m.lastChild.className!=='m a'){add('a','');}m.lastChild.textContent+=d.text;}
                else if(d.type==='done'){b.disabled=false;}
                else if(d.type==='error'){add('s','Hata: '+d.text);b.disabled=false;}});
            window.chrome.webview.postMessage(JSON.stringify({type:'ready'}));
        </script></body></html>";
    }
}
