using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex
{
    public class PlexEntriesWithCachingTests : PlexEntriesTests
    {
        // The quality of these tests is reduced since IMemory cache using an out argument and an unmockable MemoryCacheEntryOptions
        private readonly IFixture _fixture;
        private readonly IPlexLibraryClient _mockPlexLibraryClient;
        private readonly IMemoryCache _mockMemoryCache;

        public PlexEntriesWithCachingTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _mockPlexLibraryClient = Substitute.For<IPlexLibraryClient>();
            _mockPlexLibraryClient.GetLibraries().Returns(_fixture.Create<LibraryContainer>());
            _mockMemoryCache = Substitute.For<IMemoryCache>();
        }

        [Fact]
        public void Constructor_NullMemoryCache_Throws()
        {
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new PlexEntriesWithCaching(_mockPlexLibraryClient, null, 
                _fixture.Create<OptionsWrapper<PlexWithCachingOptions>>()));

            // Assert
            ex.ParamName.Should().Be("memoryCache");
        }

        [Fact]
        public void Constructor_CacheExpiryLessThanZero_Throws()
        {
            // Arrange
            var options = _fixture
                .Build<PlexWithCachingOptions>()
                .With(cachingOptions => cachingOptions.CacheExpiry, 0)
                .Create();
            var testInvalidCacheOptions= new OptionsWrapper<PlexWithCachingOptions>(options);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => 
                new PlexEntriesWithCaching(_mockPlexLibraryClient, _mockMemoryCache, testInvalidCacheOptions));

            // Assert
            ex.Message.Should().Be($"If options contains a {nameof(testInvalidCacheOptions.Value.CacheExpiry)} it " +
                                   $"must be greater than zero. If no caching is desired, the {nameof(testInvalidCacheOptions.Value.CacheExpiry)} " +
                                   "must be omitted.");
        }

        [Fact]
        public async Task GetPlexEntries_WithoutCacheExpiry_DoesNotFetchFromCache()
        {
            // Arrange
            var options = _fixture
                .Build<PlexWithCachingOptions>()
                .Without(cachingOptions => cachingOptions.CacheExpiry)
                .Create();
            var testCacheOptionsWithoutCache = new OptionsWrapper<PlexWithCachingOptions>(options);

            var plexEntriesWithCaching =
                new PlexEntriesWithCaching(_mockPlexLibraryClient, _mockMemoryCache, testCacheOptionsWithoutCache);

            // Act
            await plexEntriesWithCaching.GetPlexEntries();

            // Assert
            _mockMemoryCache.DidNotReceiveWithAnyArgs().TryGetValue(Arg.Any<object>(), out _);

        }

        [Fact]
        public async Task GetPlexEntries_WithCacheExpiry_FetchesFromCache()
        {
            // Arrange
            var options = _fixture.Create<PlexWithCachingOptions>();
            var testCacheOptions = new OptionsWrapper<PlexWithCachingOptions>(options);

            var plexEntriesWithCaching =
                new PlexEntriesWithCaching(_mockPlexLibraryClient, _mockMemoryCache, testCacheOptions);

            // Act
            await plexEntriesWithCaching.GetPlexEntries();

            // Assert
            _mockMemoryCache.Received(1).TryGetValue(Arg.Is("PlexEntries"), out _);

        }

        [Fact]
        public async Task GetPlexEntries_WithCacheExpiry_SavesToCache()
        {
            // Arrange
            var options = _fixture.Create<PlexWithCachingOptions>();
            var testCacheOptions = new OptionsWrapper<PlexWithCachingOptions>(options);

            var plexEntriesWithCaching =
                new PlexEntriesWithCaching(_mockPlexLibraryClient, _mockMemoryCache, testCacheOptions);

            // Act
            await plexEntriesWithCaching.GetPlexEntries();

            // Assert
            _mockMemoryCache.Received(1).Set(
                Arg.Is("PlexEntries"), 
                Arg.Any<PlexEntries>(), 
                Arg.Any<MemoryCacheEntryOptions>());

        }
    }
}
