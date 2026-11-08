using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Win32;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public static class DialogWindows
    {
        private static Brush Bg => (Brush)Application.Current.Resources["BackgroundBrush"];
        private static Brush Surface => (Brush)Application.Current.Resources["SurfaceBrush"];
        private static Brush Fg => (Brush)Application.Current.Resources["ForegroundBrush"];
        private static Brush Border => (Brush)Application.Current.Resources["BorderBrush"];
        private static Brush Input => (Brush)Application.Current.Resources["InputBrush"];

        public static void ShowPasswords(PasswordService service)
        {
            var window = new Window
            {
                Title = "Şifre Yöneticisi", Width = 560, Height = 460,
                Background = Bg, Foreground = Fg,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow
            };
            var grid = new Grid { Margin = new Thickness(12) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var header = new TextBlock
            {
                Text = "Kayıtlı Şifreler (DPAPI ile şifreli)",
                Foreground = Fg, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 8)
            };
            Grid.SetRow(header, 0); grid.Children.Add(header);

            var list = new ListBox { Background = Surface, Foreground = Fg, BorderBrush = Border };
            Action refresh = () =>
            {
                list.Items.Clear();
                foreach (var e in service.Entries) list.Items.Add($"{e.Site}  |  {e.Username}");
            };
            refresh();
            Grid.SetRow(list, 1); grid.Children.Add(list);

            var bar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 0) };
            var addBtn = new Button { Content = "Ekle", Margin = new Thickness(0, 0, 6, 0), Padding = new Thickness(10, 4, 10, 4) };
            addBtn.Click += (s, e) =>
            {
                string? site = DialogHelper.Prompt("Site", "Site (örn. google.com)");
                if (string.IsNullOrWhiteSpace(site)) return;
                string? user = DialogHelper.Prompt("Kullanıcı", "Kullanıcı adı");
                if (string.IsNullOrWhiteSpace(user)) return;
                string? pass = DialogHelper.Prompt("Şifre", "Şifre");
                if (string.IsNullOrWhiteSpace(pass)) return;
                service.Save(site!, user!, pass!);
                refresh();
            };
            var delBtn = new Button { Content = "Sil", Padding = new Thickness(10, 4, 10, 4) };
            delBtn.Click += (s, e) =>
            {
                if (list.SelectedIndex < 0) return;
                var entry = service.Entries.ElementAtOrDefault(list.SelectedIndex);
                if (entry != null) { service.Delete(entry); refresh(); }
            };
            bar.Children.Add(addBtn); bar.Children.Add(delBtn);
            Grid.SetRow(bar, 2); grid.Children.Add(bar);

            window.Content = grid;
            window.ShowDialog();
        }

        public static void ShowDownloads(DownloadService service)
        {
            var window = new Window
            {
                Title = "İndirmeler", Width = 640, Height = 420,
                Background = Bg, Foreground = Fg,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow
            };
            var grid = new Grid { Margin = new Thickness(12) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var header = new TextBlock
            {
                Text = "İndirmeler", Foreground = Fg, FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 8)
            };
            Grid.SetRow(header, 0); grid.Children.Add(header);

            var list = new ListBox { Background = Surface, Foreground = Fg, BorderBrush = Border };
            var template = new DataTemplate(typeof(DownloadItem));
            var stack = new FrameworkElementFactory(typeof(StackPanel));
            stack.SetValue(StackPanel.MarginProperty, new Thickness(4));

            var name = new FrameworkElementFactory(typeof(TextBlock));
            name.SetBinding(TextBlock.TextProperty, new Binding("FileName"));
            name.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            name.SetValue(TextBlock.ForegroundProperty, Fg);
            stack.AppendChild(name);

            var status = new FrameworkElementFactory(typeof(TextBlock));
            status.SetBinding(TextBlock.TextProperty, new Binding("Status"));
            status.SetValue(TextBlock.ForegroundProperty, Fg);
            stack.AppendChild(status);

            var progress = new FrameworkElementFactory(typeof(ProgressBar));
            progress.SetValue(ProgressBar.HeightProperty, 8.0);
            progress.SetBinding(ProgressBar.ValueProperty, new Binding("Progress"));
            progress.SetValue(ProgressBar.MaximumProperty, 100.0);
            stack.AppendChild(progress);

            template.VisualTree = stack;
            list.ItemTemplate = template;
            list.ItemsSource = service.Downloads;
            Grid.SetRow(list, 1); grid.Children.Add(list);

            window.Content = grid;
            window.Show();
        }

        public static void ShowExtensions(ExtensionService service, Microsoft.Web.WebView2.Wpf.WebView2 webView, Func<System.Threading.Tasks.Task> onInstalled)
        {
            var window = new Window
            {
                Title = "Uzantı Yönetimi", Width = 720, Height = 600,
                Background = Bg, Foreground = Fg,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow
            };
            var root = new StackPanel { Margin = new Thickness(12) };

            var title1 = new TextBlock
            {
                Text = "Yerleşik Uzantılar (Assets/Extensions)",
                FontWeight = FontWeights.Bold, Foreground = Fg,
                Margin = new Thickness(0, 0, 0, 6)
            };
            root.Children.Add(title1);

            var builtIns = service.GetBuiltInExtensionNames();
            var builtInRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
            var builtInCombo = new ComboBox { Width = 220, Background = Input, Foreground = Fg };
            foreach (var name in builtIns) builtInCombo.Items.Add(name);
            if (builtIns.Count > 0) builtInCombo.SelectedIndex = 0;
            var builtInInstallBtn = new Button
            {
                Content = "Kur", Padding = new Thickness(14, 6, 14, 6),
                Margin = new Thickness(6, 0, 0, 0), IsEnabled = builtIns.Count > 0
            };
            builtInInstallBtn.Click += async (s, e) =>
            {
                if (builtInCombo.SelectedItem is not string name) return;
                try
                {
                    string id = await service.InstallBuiltInAsync(webView, name);
                    DialogHelper.Info("Uzantı", $"{name} kuruldu.\nID: {id}\nYeni sekmelerde etkin olacak.");
                    onInstalled?.Invoke();
                }
                catch (Exception ex) { DialogHelper.Error("Hata", ex.Message); }
            };
            builtInRow.Children.Add(builtInCombo);
            builtInRow.Children.Add(builtInInstallBtn);
            root.Children.Add(builtInRow);

            var title0 = new TextBlock
            {
                Text = "Klasör veya ZIP'ten Yükle",
                FontWeight = FontWeights.Bold, Foreground = Fg,
                Margin = new Thickness(0, 6, 0, 6)
            };
            root.Children.Add(title0);

            var btnRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
            var folderBtn = new Button { Content = "Klasörden Yükle", Padding = new Thickness(12, 6, 12, 6), Margin = new Thickness(0, 0, 6, 0) };
            folderBtn.Click += async (s, e) =>
            {
                var dlg = new OpenFolderDialog();
                if (dlg.ShowDialog() != true) return;
                try
                {
                    string id = await service.InstallFromFolderAsync(webView, dlg.FolderName);
                    DialogHelper.Info("Uzantı", "Kuruldu. ID: " + id);
                    onInstalled?.Invoke();
                }
                catch (Exception ex) { DialogHelper.Error("Hata", ex.Message); }
            };
            var zipBtn = new Button { Content = "ZIP'ten Yükle", Padding = new Thickness(12, 6, 12, 6) };
            zipBtn.Click += async (s, e) =>
            {
                var dlg = new OpenFileDialog { Filter = "ZIP|*.zip" };
                if (dlg.ShowDialog() != true) return;
                try
                {
                    string id = await service.InstallFromZipAsync(webView, dlg.FileName);
                    DialogHelper.Info("Uzantı", "Kuruldu. ID: " + id);
                    onInstalled?.Invoke();
                }
                catch (Exception ex) { DialogHelper.Error("Hata", ex.Message); }
            };
            btnRow.Children.Add(folderBtn);
            btnRow.Children.Add(zipBtn);
            root.Children.Add(btnRow);

            var title2 = new TextBlock
            {
                Text = "GitHub Deposu", FontWeight = FontWeights.Bold,
                Foreground = Fg, Margin = new Thickness(0, 6, 0, 6)
            };
            root.Children.Add(title2);

            var urlBox = new TextBox
            {
                Text = service.BaseUrl, Background = Input, Foreground = Fg,
                BorderBrush = Border, Padding = new Thickness(4, 4, 4, 4), Margin = new Thickness(0, 0, 0, 6)
            };
            root.Children.Add(urlBox);

            var list = new ListBox
            {
                Background = Surface, Foreground = Fg, BorderBrush = Border,
                Height = 200, Margin = new Thickness(0, 0, 0, 6)
            };
            root.Children.Add(list);

            ExtensionManifest[] manifests = Array.Empty<ExtensionManifest>();
            Func<System.Threading.Tasks.Task> refresh = async () =>
            {
                service.BaseUrl = urlBox.Text.Trim();
                manifests = await service.GetAvailableExtensionsAsync();
                list.Items.Clear();
                if (manifests.Length == 0)
                    list.Items.Add("(Uzantı bulunamadı)");
                else
                    foreach (var m in manifests)
                        list.Items.Add($"{m.Name} v{m.Version} — {m.Description}");
            };

            var ghRow = new StackPanel { Orientation = Orientation.Horizontal };
            var loadBtn = new Button { Content = "Listeyi Getir", Padding = new Thickness(12, 6, 12, 6), Margin = new Thickness(0, 0, 6, 0) };
            loadBtn.Click += async (s, e) => await refresh();
            var installBtn = new Button { Content = "Seçili Uzantıyı Kur", Padding = new Thickness(12, 6, 12, 6) };
            installBtn.Click += async (s, e) =>
            {
                if (list.SelectedIndex < 0 || manifests.Length == 0) return;
                try
                {
                    string id = await service.InstallFromGitHubAsync(webView, manifests[list.SelectedIndex]);
                    DialogHelper.Info("Uzantı", "Kuruldu. ID: " + id);
                    onInstalled?.Invoke();
                }
                catch (Exception ex) { DialogHelper.Error("Hata", ex.Message); }
            };
            ghRow.Children.Add(loadBtn);
            ghRow.Children.Add(installBtn);
            root.Children.Add(ghRow);

            window.Content = new ScrollViewer { Content = root, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            window.Show();
        }
    }
}