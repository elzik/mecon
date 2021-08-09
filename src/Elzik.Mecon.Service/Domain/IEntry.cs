namespace Elzik.Mecon.Service.Domain
{
    public interface IEntry
    {
        string Type { get; }
        EntryKey Key { get; set; }
        string Title { get; set; }
    }
}