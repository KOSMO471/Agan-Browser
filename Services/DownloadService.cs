using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Web.WebView2.Core;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class DownloadService
    {
        public ObservableCollection<DownloadItem> Downloads { get; } = new();

        public void Attach(CoreWebView2 core)
        {
            core.DownloadStarting += Core_DownloadStarting;
        }

        private void Core_DownloadStarting(object? sender, CoreWebView2DownloadStartingEventArgs e)
        {
            var op = e.DownloadOperation;
            var item = new DownloadItem
            {
                FileName = Path.GetFileName(e.ResultFilePath),
                FullPath = e.ResultFilePath,
                TotalBytes = (long)(op.TotalBytesToReceive ?? 0),
                Status = "İndiriliyor"
            };
            Downloads.Add(item);

            op.BytesReceivedChanged += (s, ev) =>
            {
                if (item.TotalBytes > 0)
                    item.Progress = (double)op.BytesReceived / (double)item.TotalBytes * 100.0;
            };

            op.StateChanged += (s, ev) =>
            {
                item.Status = op.State switch
                {
                    CoreWebView2DownloadState.Completed => "Tamamlandı",
                    CoreWebView2DownloadState.Interrupted => "Kesildi",
                    CoreWebView2DownloadState.InProgress => "İndiriliyor",
                    _ => "Bilinmiyor"
                };
            };
        }
    }
}
