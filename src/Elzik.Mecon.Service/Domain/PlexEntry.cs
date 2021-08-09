namespace Elzik.Mecon.Service.Domain
{
    public class PlexEntry : IEntry
    {
        public string Type => nameof(PlexEntry);
        public EntryKey Key { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
