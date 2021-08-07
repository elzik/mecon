namespace Elzik.Mecon.Service.Domain
{
    public interface IEntry
    {
        string Type { get; }
        string Key { get; set; }
        string Title { get; set; }
    }
}