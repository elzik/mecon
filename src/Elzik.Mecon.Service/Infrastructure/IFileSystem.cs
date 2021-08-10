using System.Collections.Generic;

namespace Elzik.Mecon.Service.Infrastructure
{
    public interface IFileSystem
    {
        IEnumerable<string> GetMedia(string directoryPath, params string[] fileExtensions);
    }
}
