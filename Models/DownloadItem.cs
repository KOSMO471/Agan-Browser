using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AganBrowser.Models
{
    public class DownloadItem : INotifyPropertyChanged
    {
        public string FileName { get; set; } = "";
        public string FullPath { get; set; } = "";
        public long TotalBytes { get; set; }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(); }
        }

        private string _status = "";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}