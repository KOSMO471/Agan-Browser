using System.Windows;

namespace AganBrowser
{
    public partial class App : Application
    {
        public App()
        {
            Services.ThemeService.Apply("Dark");
        }
    }
}