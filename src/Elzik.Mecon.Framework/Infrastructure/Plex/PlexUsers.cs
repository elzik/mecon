using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.Extensions.Options;
using Plex.ServerApi.Clients.Interfaces;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexUsers : IPlexUsers
    {
        private readonly IPlexAccountClient _plexAccountClient;
        private readonly PlexOptions _plexOptions;

        public PlexUsers(IPlexAccountClient plexAccountClient, IOptions<PlexOptions> plexOptions)
        {
            _plexAccountClient = plexAccountClient ?? throw new ArgumentNullException(nameof(plexAccountClient));

            if (plexOptions == null)
            {
                throw new ArgumentNullException(nameof(plexOptions));
            }
            _plexOptions = plexOptions.Value ??
                           throw new InvalidOperationException($"Value of {nameof(plexOptions)} must not be null.");
        }

        public async Task<IEnumerable<PlexUser>> GetPlexUsers()
        {
            var homeUserContainer = await _plexAccountClient.GetHomeUsersAsync(_plexOptions.AuthToken);
            var homeUsers = homeUserContainer.Users.Select(user => new PlexUser()
            {
                AccountId = user.Id,
                UserTitle = user.Title
            }).ToList();

            var friends = await _plexAccountClient.GetFriendsAsync(_plexOptions.AuthToken);
            var friendUsers = friends.Select(user => new PlexUser()
            {
                AccountId = user.Id,
                UserTitle = user.Title
            });

            var allUsers = homeUsers.UnionBy(friendUsers, user => user.AccountId)
                .OrderBy(user => user.UserTitle);

            return allUsers;
        }
    }
}
