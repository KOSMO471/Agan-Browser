namespace AganBrowser.Models
{
    public class BookmarkItem
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public override string ToString() => Name;
    }
}