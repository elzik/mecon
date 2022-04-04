using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.PlexModels.Library;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex
{
    public class PlexEntriesWithCachingTests : PlexEntriesTests
    {
        // The quality of these tests is reduced since IMemory cache using an out argument and an unmockable MemoryCacheEntryOptions
        private readonly IFixture _fixture;
        private readonly IPlexServerClient _mockPlexServerClient;
        private readonly IPlexLibraryClient _mockPlexLibraryClient;
        private readonly IMemoryCache _mockMemoryCache;
        private readonly PlexWithCachingOptions _options;
        private readonly MediaType[] _testMediaTypes;

        public PlexEntriesWithCachingTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _mockPlexServerClient = Substitute.For<IPlexServerClient>();
            _options = _fixture.Create<PlexWithCachingOptions>();
            _mockPlexServerClient
                .GetLibrariesAsync(
                    Arg.Is(_options.AuthToken),
                    Arg.Is(_options.BaseUrl))
                .Returns(_fixture.Create<LibraryContainer>());

            _mockPlexLibraryClient = Substitute.For<IPlexLibraryClient>();
            _mockMemoryCache = Substitute.For<IMemoryCache>();

            _testMediaTypes = new[] { MediaType.Movie, MediaType.TvShow };
        }

        [Fact]
        public void Constructor_NullMemoryCache_Throws()
        {
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new PlexEntriesWithCaching(_mockPlexServerClient, _mockPlexLibraryClient, null, 
                _fixture.Create<OptionsWrapper<PlexWithCachingOptions>>()));

            // Assert
            ex.ParamName.Should().Be("memoryCache");
        }

        [Fact]
        public void Constructor_CacheExpiryLessThanZero_Throws()
        {
            // Arrange
            _options.CacheExpiry = 0;
            var testInvalidCacheOptions= new OptionsWrapper<PlexWithCachingOptions>(_options);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => 
                new PlexEntriesWithCaching(_mockPlexServerClient, _mockPlexLibraryClient, 
                    _mockMemoryCache, testInvalidCacheOptions));

            // Assert
            ex.Message.Should().Be($"If options contains a {nameof(testInvalidCacheOptions.Value.CacheExpiry)} it " +
                                   "must be greater than zero. If no caching is desired, the " +
                                   $"{nameof(testInvalidCacheOptions.Value.CacheExpiry)} must be omitted.");
        }

        [Fact]
        public async Task GetPlexEntries_WithoutCacheExpiry_DoesNotFetchFromCache()
        {
            // Arrange
            _options.CacheExpiry = null;
            var testCacheOptionsWithoutCache = new OptionsWrapper<PlexWithCachingOptions>(_options);

            var plexEntriesWithCaching =
                new PlexEntriesWithCaching(_mockPlexServerClient, _mockPlexLibraryClient, 
                    _mockMemoryCache, testCacheOptionsWithoutCache);

            // Act
            await plexEntriesWithCaching.GetPlexEntries(_testMediaTypes);

            // Assert
            _mockMemoryCache.DidNotReceiveWithAnyArgs().TryGetValue(Arg.Any<object>(), out _);

        }

        [Fact]
        public async Task GetPlexEntries_WithCacheExpiry_FetchesFromCache()
        {
            // Arrange
            var testCacheOptions = new OptionsWrapper<PlexWithCachingOptions>(_options);

            var plexEntriesWithCaching =
                new PlexEntriesWithCaching(_mockPlexServerClient, _mockPlexLibraryClient, 
                    _mockMemoryCache, testCacheOptions);

            // Act
            await plexEntriesWithCaching.GetPlexEntries(_testMediaTypes);

            // Assert
            _mockMemoryCache.Received(1).TryGetValue(Arg.Is("PlexEntries"), out _);

        }

        [Fact]
        public async Task GetPlexEntries_WithCacheExpiry_SavesToCache()
        {
            // Arrange
            var testCacheOptions = new OptionsWrapper<PlexWithCachingOptions>(_options);

            var plexEntriesWithCaching =
                new PlexEntriesWithCaching(_mockPlexServerClient, _mockPlexLibraryClient, 
                    _mockMemoryCache, testCacheOptions);

            // Act
            await plexEntriesWithCaching.GetPlexEntries(_testMediaTypes);

            // Assert
            _mockMemoryCache.Received(1).Set(
                Arg.Is("PlexEntries"), 
                Arg.Any<PlexEntries>(), 
                Arg.Any<MemoryCacheEntryOptions>());

        }
    }
}
