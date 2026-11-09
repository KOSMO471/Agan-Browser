using System.Windows;
using AganBrowser.Services;

namespace AganBrowser
{
    public partial class PersonalityWindow : Window
    {
        private readonly PersonalityService _service;

        public PersonalityWindow(PersonalityService service)
        {
            InitializeComponent();
            _service = service;
            TraitsList.ItemsSource = _service.Traits;

            if (_service.Traits.Count == 0)
            {
                MessageBox.Show("Kişilik listesi yüklenemedi. Lütfen 'personality.json' dosyasının projede doğru yerde olduğundan ve derleme sırasında gömüldüğünden emin olun.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _service.Save();
            Close();
        }
    }
}