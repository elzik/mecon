using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Plex.ServerApi.Clients.Interfaces;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexEntriesWithCaching : PlexEntries
    {
        private readonly IMemoryCache _memoryCache;
        private MemoryCacheEntryOptions _memoryCacheEntryOptions;
        private readonly PlexWithCachingOptions _plexWithCachingOptions;

        public PlexEntriesWithCaching(IPlexServerClient plexServerClient, IPlexLibraryClient plexLibraryClient, IMemoryCache memoryCache, IOptions<PlexWithCachingOptions> plexOptions) 
            : base(plexServerClient, plexLibraryClient, plexOptions)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            ValidateOptions(plexOptions);
            _plexWithCachingOptions = plexOptions.Value;
            SetMemoryCacheOptions(plexOptions);
        }

        public override async Task<IEnumerable<PlexEntry>> GetPlexEntries(IEnumerable<MediaType> mediaTypesFilter)
        {
            if (!_plexWithCachingOptions.CacheExpiry.HasValue || !_memoryCache.TryGetValue("PlexEntries", out List<PlexEntry> plexEntries))
            {
                plexEntries = (await base.GetPlexEntries(mediaTypesFilter)).ToList();

                if (_plexWithCachingOptions.CacheExpiry.HasValue)
                {
                    _memoryCache.Set("PlexEntries", plexEntries, _memoryCacheEntryOptions);
                }
            }

            return plexEntries;
        }

        private static void ValidateOptions(IOptions<PlexWithCachingOptions> options)
        {
            if (options.Value.CacheExpiry is <= 0)
            {
                throw new InvalidOperationException($"If {nameof(options)} contains a {nameof(options.Value.CacheExpiry)} it " +
                                                    $"must be greater than zero. If no caching is desired, the {nameof(options.Value.CacheExpiry)} " +
                                                    "must be omitted.");
            }
        }

        private void SetMemoryCacheOptions(IOptions<PlexWithCachingOptions> plexOptions)
        {
            if (plexOptions.Value.CacheExpiry.HasValue)
            {
                _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds((int)plexOptions.Value.CacheExpiry));
            }
        }
    }
}
