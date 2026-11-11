using System.Windows;

namespace AganBrowser
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Services.ThemeService.Initialize();
        }
    }
}
