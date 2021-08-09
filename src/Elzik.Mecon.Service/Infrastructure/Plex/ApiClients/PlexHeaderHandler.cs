using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Service.Infrastructure.Plex.ApiClients
{
    public class PlexHeaderHandler : DelegatingHandler
    {
        private readonly string _token;

        public PlexHeaderHandler(IOptions<PlexOptions> options)
        {
            ValidateOptions(options);

            _token = options.Value.AuthToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-Plex-Client-Identifier", "mecon");
            request.Headers.Add("X-Plex-Token", _token);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateOptions(IOptions<PlexOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Value == null)
            {
                throw new InvalidOperationException($"{nameof(options)} must not be null.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.AuthToken))
            {
                throw new InvalidOperationException($"{nameof(options)} must contain an {nameof(options.Value.AuthToken)}.");
            }
        }
    }
}
