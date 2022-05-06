using System.Collections.Generic;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Framework.Application
{
    public interface IReconciledMedia
    {
        Task<IEnumerable<MediaEntry>> GetMediaEntries(string directoryDefinitionName);

        Task<IEnumerable<MediaEntry>> GetMediaEntries(
            string directoryPath, 
            IEnumerable<string> supportedFileExtensions, 
            bool recurse, 
            IEnumerable<MediaType> mediaTypes,
            string directoryFilterRegexPattern);
    }
}