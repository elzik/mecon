using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex
{
    public class PlexHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-Plex-Client-Identifier", "mecon");
            request.Headers.Add("X-Plex-Token", "KPSTWHM3Y3_XYL1Vfjms");

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
