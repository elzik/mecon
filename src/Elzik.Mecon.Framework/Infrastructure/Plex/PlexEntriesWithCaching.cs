using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexEntriesWithCaching : PlexEntries
    {
        private readonly IMemoryCache _memoryCache;
        private MemoryCacheEntryOptions _memoryCacheEntryOptions;
        private readonly PlexOptionsWithCaching _plexOptions;

        public PlexEntriesWithCaching(IPlexLibraryClient plexLibraryClient, IMemoryCache memoryCache, IOptions<PlexOptionsWithCaching> plexOptions) 
            : base(plexLibraryClient, plexOptions)
        {
            _memoryCache = memoryCache;

            ValidateOptions(plexOptions);
            _plexOptions = plexOptions.Value;
            SetMemoryCacheOptions(plexOptions);
        }

        public override async Task<IEnumerable<PlexEntry>> GetPlexEntries()
        {
            if (!_plexOptions.CacheExpiry.HasValue || !_memoryCache.TryGetValue("PlexEntries", out List<PlexEntry> plexEntries))
            {
                plexEntries = (await base.GetPlexEntries()).ToList();

                if (_plexOptions.CacheExpiry.HasValue)
                {
                    _memoryCache.Set("PlexEntries", plexEntries, _memoryCacheEntryOptions);
                }
            }

            return plexEntries;
        }

        private static void ValidateOptions(IOptions<PlexOptionsWithCaching> options)
        {
            if (options.Value.CacheExpiry is <= 0)
            {
                throw new InvalidOperationException($"If {nameof(options)} contains a {nameof(options.Value.CacheExpiry)} it " +
                                                    $"must be greater than zero. If no caching is desired, the {nameof(options.Value.CacheExpiry)} " +
                                                    $"must be omitted.");
            }
        }

        private void SetMemoryCacheOptions(IOptions<PlexOptionsWithCaching> plexOptions)
        {
            if (plexOptions.Value.CacheExpiry.HasValue)
            {
                _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds((int)plexOptions.Value.CacheExpiry));
            }
        }
    }
}
