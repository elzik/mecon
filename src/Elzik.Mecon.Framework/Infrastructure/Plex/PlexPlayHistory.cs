using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Plex.ServerApi.Clients.Interfaces;
using Elzik.Mecon.Framework.Domain.Plex;
using Microsoft.Extensions.Options;
using Plex.ServerApi.PlexModels.Server.History;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexPlayHistory : IPlexPlayHistory
    {
        private readonly IPlexServerClient _plexServerClient;
        private readonly PlexOptions _plexOptions;

        public PlexPlayHistory(IPlexServerClient plexServerClient, IOptions<PlexOptions> plexOptions)
        {
            _plexServerClient = plexServerClient ?? throw new ArgumentNullException(nameof(plexServerClient));

            if (plexOptions == null)
            {
                throw new ArgumentNullException(nameof(plexOptions));
            }
            _plexOptions = plexOptions.Value ??
                           throw new InvalidOperationException($"Value of {nameof(plexOptions)} must not be null.");
        }

        public async Task<List<PlayedEntry>> GetPlayHistory()
        {
            var mediaItems = new List<PlayedEntry>();
            var consumptionCount = 0;
            HistoryMediaContainer mediaContainer;
            do
            {
                mediaContainer = await _plexServerClient.GetPlayHistory(_plexOptions.AuthToken, _plexOptions.BaseUrl, consumptionCount, _plexOptions.ItemsPerCall);

                if (mediaContainer.HistoryMetadata == null)
                {
                    break;
                }

                var itemsInLibrary = mediaContainer.HistoryMetadata.Where(metadata => metadata.Key != null);
                mediaItems.AddRange(itemsInLibrary.Select(metadata => new PlayedEntry()
                {
                    AccountId = metadata.AccountId,
                    LibraryKey = metadata.Key
                }));

                consumptionCount += mediaContainer.Size;

            } while (mediaContainer.Size > 0);

            return mediaItems;
        }
    }
}
