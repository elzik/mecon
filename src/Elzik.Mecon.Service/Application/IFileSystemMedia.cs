using System.Collections.Generic;
using Elzik.Mecon.Service.Domain;

namespace Elzik.Mecon.Service.Application
{
    public interface IFileSystemMedia
    {
        IEnumerable<MediaEntry> GetMediaEntries();
    }
}