using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.Plex;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{
    public class OutputOperations : IOutputOperations
    {
        private readonly IPlexUsers _plexUsers;

        public OutputOperations(IPlexUsers plexUsers)
        {
            _plexUsers = plexUsers ?? throw new ArgumentNullException(nameof(plexUsers));
        }

        public async Task<IMediaEntryCollection> PerformOutputFilters(IMediaEntryCollection entries,
            ReconciliationOptions options)
        {
            if (options.MissingFromLibrary)
            {
                entries = entries.WhereNotInPlex();
            }

            if (options.PresentInLibrary)
            {
                entries = entries.WhereInPlex();
            }

            if (options.WatchedByUsers != null && options.WatchedByUsers.Any())
            {
                var expandedWatchedByUserTitles = await GetExpandedWatchedByUserTitles(options.WatchedByUsers);

                var watchedByAccountIds = await _plexUsers.GetAccountIds(expandedWatchedByUserTitles);

                entries = entries.WhereWatchedByAccounts(watchedByAccountIds);
            }

            return entries;
        }

        private async Task<IEnumerable<string>> GetExpandedWatchedByUserTitles(IEnumerable<string> watchedByUserTitles)
        {
            var watchedByUserTitlesList = watchedByUserTitles.ToList();
            
            if (watchedByUserTitlesList.Any(IsExclamationMark))
            {
                watchedByUserTitlesList.AddRange((await _plexUsers.GetPlexUsers()).Select(user => user.UserTitle));
                watchedByUserTitlesList.RemoveAll(IsExclamationMark);
            }

            return watchedByUserTitlesList;
        }

        private static bool IsExclamationMark(string s)
        {
            return s.Equals("!", StringComparison.InvariantCulture);
        }
    }
}
