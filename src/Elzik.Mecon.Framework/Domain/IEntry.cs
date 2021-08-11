namespace Elzik.Mecon.Framework.Domain
{
    public interface IEntry
    {
        string Type { get; }
        EntryKey Key { get; set; }
        string Title { get; set; }
    }
}