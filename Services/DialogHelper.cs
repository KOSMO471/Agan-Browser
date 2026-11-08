using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AganBrowser.Services
{
    public static class DialogHelper
    {
        public static string? Prompt(string title, string message, string defaultValue = "")
        {
            var window = new Window
            {
                Title = title,
                Width = 420,
                Height = 190,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                Background = (Brush)Application.Current.Resources["SurfaceBrush"]
            };
            var panel = new StackPanel { Margin = new Thickness(16) };
            panel.Children.Add(new TextBlock
            {
                Text = message,
                Margin = new Thickness(0, 0, 0, 8),
                Foreground = (Brush)Application.Current.Resources["ForegroundBrush"]
            });
            var textBox = new TextBox
            {
                Text = defaultValue,
                Background = (Brush)Application.Current.Resources["InputBrush"],
                Foreground = (Brush)Application.Current.Resources["ForegroundBrush"],
                BorderBrush = (Brush)Application.Current.Resources["BorderBrush"],
                Padding = new Thickness(4, 2, 4, 2)
            };
            panel.Children.Add(textBox);
            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 12, 0, 0)
            };
            string? result = null;
            var ok = new Button { Content = "Tamam", Width = 70, IsDefault = true, Margin = new Thickness(0, 0, 6, 0) };
            ok.Click += (s, e) => { result = textBox.Text; window.DialogResult = true; };
            var cancel = new Button { Content = "İptal", Width = 70, IsCancel = true };
            buttons.Children.Add(ok);
            buttons.Children.Add(cancel);
            panel.Children.Add(buttons);
            window.Content = panel;
            window.ShowDialog();
            return result;
        }

        public static void Info(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Error(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}