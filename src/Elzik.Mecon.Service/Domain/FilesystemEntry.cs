namespace Elzik.Mecon.Service.Domain
{
    public class FilesystemEntry : IEntry
    {
        public string Type => nameof(FilesystemEntry);
        public EntryKey Key { get; set; }
        public string Title { get; set; }
        public string FileSystemPath { get; set; }
    }
}