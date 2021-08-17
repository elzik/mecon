using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients
{
    public class PlexHeaderHandler : DelegatingHandler
    {
        private readonly PlexOptions _plexOptions;

        public PlexHeaderHandler(IOptions<PlexOptions> options)
        {
            ValidateOptions(options);

            _plexOptions = options.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-Plex-Client-Identifier", "mecon");
            request.Headers.Add("X-Plex-Token", _plexOptions.AuthToken);

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
                throw new InvalidOperationException($"Value of {nameof(options)} must not be null.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.AuthToken))
            {
                throw new InvalidOperationException($"{nameof(PlexOptions)} must contain an {nameof(options.Value.AuthToken)}.");
            }
        }
    }
}
