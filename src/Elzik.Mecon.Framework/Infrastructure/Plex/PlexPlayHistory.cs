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
        private readonly IPlexAccountClient _plexAccountClient;
        private readonly IPlexServerClient _plexServerClient;
        private readonly PlexOptions _plexOptions;

        public PlexPlayHistory(IPlexServerClient plexServerClient, IPlexAccountClient plexAccountClient, IOptions<PlexOptions> plexOptions)
        {
            _plexServerClient = plexServerClient ?? throw new ArgumentNullException(nameof(plexServerClient));
            _plexAccountClient = plexAccountClient ?? throw new ArgumentNullException(nameof(plexAccountClient));

            if (plexOptions == null)
            {
                throw new ArgumentNullException(nameof(plexOptions));
            }
            _plexOptions = plexOptions.Value ??
                           throw new InvalidOperationException($"Value of {nameof(plexOptions)} must not be null.");
        }

        public async Task<IEnumerable<PlayedEntry>> GetPlayHistory()
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

            await FixAdminIds(mediaItems);

            return mediaItems;
        }

        /// <summary>
        /// The Plex end point '/status/sessions/history/all' seems to return an ID of 1 for all home admin users rather than their given ID.
        /// This seems incorrect and makes it difficult to reconcile users with played media.
        /// A Plex employee has agreed that it would have made more sense to return the correct ID so it seems reasonable to correct this.
        /// https://forums.plex.tv/t/api-users-play-history/796964
        /// </summary>
        /// <param name="entries">Played entries where the home admin user is incorrectly identified with an ID of 1.</param>
        /// <returns>Played entries where the home admin user is correctly identified with the ID supplied.</returns>
        private async Task FixAdminIds(IEnumerable<PlayedEntry> entries)
        {
            var homeAdminId = await GetHomeAdminId();

            foreach (var entry in entries.Where(entry => entry.AccountId == 1))
            {
                entry.AccountId = homeAdminId;
            }
        }

        private async Task<int> GetHomeAdminId()
        {
            var homeUserContainer = await _plexAccountClient.GetHomeUsersAsync(_plexOptions.AuthToken);
            var homeAdminUsers = homeUserContainer.Users.Where(user => user.IsAdmin).ToList();
            if (!homeAdminUsers.Any())
            {
                throw new InvalidOperationException("No home admin users were found.");
            }

            if (homeAdminUsers.Count > 1)
            {
                throw new InvalidOperationException("More than one home admin users were found.");
            }

            var homeAdminId = homeAdminUsers.Single().Id;
            return homeAdminId;
        }
    }
}
