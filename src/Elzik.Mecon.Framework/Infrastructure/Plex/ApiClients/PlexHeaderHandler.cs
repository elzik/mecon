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
            if (string.IsNullOrWhiteSpace(_plexOptions.AuthToken))
            {
                throw new InvalidOperationException($"{nameof(_plexOptions)} must contain an {nameof(_plexOptions.AuthToken)}.");
            }

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
                throw new InvalidOperationException($"{nameof(options)} must not be null.");
            }
        }
    }
}
