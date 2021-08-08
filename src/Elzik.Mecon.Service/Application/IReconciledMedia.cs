using System.Collections.Generic;
using System.Threading.Tasks;
using Elzik.Mecon.Service.Domain;

namespace Elzik.Mecon.Service.Application
{
    public interface IReconciledMedia
    {
        Task<IEnumerable<MediaEntry>> GetMediaEntries(string mediaPath);
    }
}