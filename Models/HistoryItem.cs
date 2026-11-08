using System;

namespace AganBrowser.Models
{
    public class HistoryItem
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime VisitedAt { get; set; } = DateTime.Now;
        public override string ToString() => $"{Title} — {Url}";
    }
}