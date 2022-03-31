using System.Collections.Generic;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Framework.Application
{
    public interface IReconciledMedia
    {
        Task<IEnumerable<MediaEntry>> GetMediaEntries(string folderDefinitionName);

        Task<IEnumerable<MediaEntry>> GetMediaEntries(string folderPath, IEnumerable<string> supportedFileExtensions, bool recurse);
    }
}