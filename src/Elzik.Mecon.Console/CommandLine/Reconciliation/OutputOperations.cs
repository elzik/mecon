using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.Plex;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{
    internal class OutputOperations : IOutputOperations
    {
        private readonly IPlexUsers _plexUsers;

        public OutputOperations(IPlexUsers plexUsers)
        {
            _plexUsers = plexUsers ?? throw new ArgumentNullException(nameof(plexUsers));
        }

        public async Task<IEnumerable<MediaEntry>> PerformOutputFilters(IEnumerable<MediaEntry> entries,
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

            if (options.WatchedByUsers != null)
            {
                // TODO: Ignore spaces on the command line for the -w option.
                // TODO: Ignore case when finding user titles.
                // TODO: Validate that each supplied user title has a matching Plex user.
                var plexUsers = (await _plexUsers.GetPlexUsers()).ToList();

                var watchedByUsers = options.WatchedByUsers.Count() == 1 && options.WatchedByUsers.Single() == "!"
                    ? plexUsers.Select(user => user.UserTitle)
                    : options.WatchedByUsers;

                var watchedByAccountIds = watchedByUsers.Select(watchedByUserTitle => 
                    plexUsers.Single(user => user.UserTitle == watchedByUserTitle).AccountId);

                entries = entries.WhereWatchedByAccounts(watchedByAccountIds);
            }

            return entries;
        }
    }
}
