using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
using Plex.ServerApi.PlexModels.Media;
using Xunit;
using MediaType = Elzik.Mecon.Framework.Domain.MediaType;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex
{
    public class PlexEntriesTests
    {
        private readonly IFixture _fixture;

        private readonly IPlexServerClient _mockPlexServerClient;
        private readonly IPlexLibraryClient _mockPlexLibraryClient;
        private readonly IPlexPlayHistory _mockPlexPlayHistory;

        private readonly MediaContainer _testVideos;
        private readonly LibraryContainer _testLibraries;
        private readonly Library _testMovieLibrary;
        private readonly OptionsWrapper<PlexOptions> _plexOptions;
        private readonly PlexEntries _plexEntries;
        private readonly MediaType[] _testMediaTypes;

        public PlexEntriesTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _testLibraries = _fixture.Create<LibraryContainer>();
            _testMovieLibrary = _fixture
                .Build<Library>()
                .With(library => library.Type, "movie")
                .Create();
            _testVideos = GetVideoMediaContainer();
            _plexOptions = new OptionsWrapper<PlexOptions>(_fixture.Build<PlexOptions>()
                .With(options => options.ItemsPerCall, _testVideos.Media.Count).Create());
            _mockPlexServerClient = Substitute.For<IPlexServerClient>();
            _mockPlexServerClient
                .GetLibrariesAsync(
                    Arg.Is(_plexOptions.Value.AuthToken), 
                    Arg.Is(_plexOptions.Value.BaseUrl))
                .Returns(_testLibraries);
            _mockPlexLibraryClient = Substitute.For<IPlexLibraryClient>();
            _mockPlexLibraryClient.LibrarySearch(
                Arg.Is(_plexOptions.Value.AuthToken),
                Arg.Is(_plexOptions.Value.BaseUrl),
                Arg.Is(string.Empty),
                Arg.Is(_testMovieLibrary.Key),
                Arg.Is(string.Empty),
                Arg.Is(SearchType.Movie),
                null,
                Arg.Is(0),
                Arg.Is(_testVideos.Media.Count)).Returns(_testVideos);
            _mockPlexLibraryClient.LibrarySearch(
                Arg.Is(_plexOptions.Value.AuthToken),
                Arg.Is(_plexOptions.Value.BaseUrl),
                Arg.Is(string.Empty),
                Arg.Is(_testMovieLibrary.Key),
                Arg.Is(string.Empty),
                Arg.Is(SearchType.Movie),
                null,
                Arg.Is(0),
                Arg.Is(_testVideos.Media.Count)).Returns(_testVideos);
            _mockPlexPlayHistory = Substitute.For<IPlexPlayHistory>();

            _testMediaTypes = new[] {MediaType.Movie, MediaType.TvShow};

            _plexEntries = new PlexEntries(_mockPlexServerClient, _mockPlexLibraryClient, _plexOptions, _mockPlexPlayHistory);
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(PlexEntries).GetConstructors());
        }

        [Fact]
        public void Constructor_WithoutPlexOptions_Throws()
        {
            // Arrange
            var nullPlexOptions = new OptionsWrapper<PlexOptions>(null);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() =>
                new PlexEntries(_mockPlexServerClient, _mockPlexLibraryClient, nullPlexOptions, _mockPlexPlayHistory));
            
            // Assert
            ex.Message.Should().Be("Value of plexOptions must not be null.");
        }

        [Fact]
        public async Task GetPlexEntries_WithoutVideoLibrary_ReturnsNoPlexEntries()
        {
            // Act
            var plexItems = await _plexEntries.GetPlexEntries(_testMediaTypes);

            // Assert
            plexItems.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPlexEntries_WithVideoLibrary_ReturnsPlexEntries()
        {
            // Arrange
            _testLibraries.Libraries.Add(_testMovieLibrary);

            // Act
            var plexItems = await _plexEntries.GetPlexEntries(_testMediaTypes);

            // Assert
            var plexItemList = plexItems.ToList();
            plexItemList.Should().HaveSameCount(_testVideos.Media);

            plexItemList.Should().Contain(entry =>
                entry.Key.ByteCount == 14920095015 &&
                entry.Key.Filename == "4 aventures de Reinette et Mirabelle 1987 1080p BluRay FLAC x264-EA.mkv" &&
                entry.ThumbnailUrl == _plexOptions.Value.BaseUrl
                + "/library/metadata/64143/thumb/1628558849?X-Plex-Token="
                + _plexOptions.Value.AuthToken &&
                entry.Title == "4 aventures de Reinette et Mirabelle" &&
                entry.Type == "PlexEntry");

            plexItemList.Should().Contain(entry =>
                entry.Key.ByteCount == 15825072616 &&
                entry.Key.Filename == "20th.Century.Women.2016.1080p.BluRay.DTS.x264-VietHD.mkv" &&
                entry.ThumbnailUrl == _plexOptions.Value.BaseUrl
                + "/library/metadata/64420/thumb/1623185035?X-Plex-Token="
                + _plexOptions.Value.AuthToken &&
                entry.Title == "20th Century Women" &&
                entry.Type == "PlexEntry");

            plexItemList.Should().Contain(entry =>
                entry.Key.ByteCount == 16712596293 &&
                entry.Key.Filename == "28.Weeks.Later.2007.1080p.BluRay.DTS.x264-IDE.mkv" &&
                entry.ThumbnailUrl == _plexOptions.Value.BaseUrl
                + "/library/metadata/64142/thumb/1628558849?X-Plex-Token="
                + _plexOptions.Value.AuthToken &&
                entry.Title == "28 Weeks Later" &&
                entry.Type == "PlexEntry");
        }

        [Fact]
        public async Task GetPlexEntries_WithEmptyLibrary_ReturnsNoEntries()
        {
            // Arrange
            _testVideos.Media = null;
            _testLibraries.Libraries.Add(_testMovieLibrary);

            // Act
            var plexItems = await _plexEntries.GetPlexEntries(_testMediaTypes);

            // Assert
            plexItems.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPlexEntries_WithNoPlayHistory_ReturnsNoWatchedByIds()
        {
            // Arrange
            _testLibraries.Libraries.Add(_testMovieLibrary);

            // Act
            var plexItems = await _plexEntries.GetPlexEntries(_testMediaTypes);

            // Assert
            plexItems.Sum(entry => entry.WatchedByAccounts.Sum()).Should().Be(0);
        }

        [Fact]
        public async Task GetPlexEntries_WithPlayHistory_ReturnsPlexEntriesWithWatchedByIds()
        {
            // Arrange
            _testLibraries.Libraries.Add(_testMovieLibrary);
            var testPlayHistory = CreateTestPlayHistory();

            _mockPlexPlayHistory.GetPlayHistory().Returns(testPlayHistory.PlayedEntries);

            // Act
            var plexItems = await _plexEntries.GetPlexEntries(_testMediaTypes);

            // Assert
            foreach (var plexEntry in plexItems)
            {
                plexEntry.WatchedByAccounts.Should().BeEquivalentTo(testPlayHistory.ExpectedWatchedIdsByKey[plexEntry.Key]);
            }
        }

        private class TestPlayHistory
        {
            public List<PlayedEntry> PlayedEntries { get; set; }
            public Dictionary<EntryKey, IEnumerable<int>> ExpectedWatchedIdsByKey { get; set; }
        }

        private TestPlayHistory CreateTestPlayHistory()
        {
            var playHistory = new List<PlayedEntry>();
            var expectedWatchedIdsByKey = new Dictionary<EntryKey, IEnumerable<int>>();

            foreach (var testVideo in _testVideos.Media)
            {
                var playedEntries = new List<PlayedEntry>();
                for (var i = 0; i < Random.Shared.Next(0, 10); i++)
                {
                    playedEntries.Add(new PlayedEntry()
                    {
                        AccountId = _fixture.Create<int>(),
                        LibraryKey = testVideo.Key
                    });
                }

                playHistory.AddRange(playedEntries);

                foreach (var mediaPart in testVideo.Media.SelectMany(medium => medium.Part))
                {
                    expectedWatchedIdsByKey.Add(
                        new EntryKey(Path.GetFileName(mediaPart.File), mediaPart.Size),
                        playedEntries.Select(entry => entry.AccountId));
                }
            }

            return new TestPlayHistory()
            {
                PlayedEntries = playHistory,
                ExpectedWatchedIdsByKey = expectedWatchedIdsByKey
            };
        }

        private static MediaContainer GetVideoMediaContainer()
        {
            var xmlSerialiser = new XmlSerializer(typeof(MediaContainer));

            using var userContainerStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex.TestData.TestVideoContainer.xml");
            if (userContainerStream == null)
            {
                throw new InvalidOperationException("TestUserContainer.xml embedded resource not found.");
            }

            return (MediaContainer)xmlSerialiser.Deserialize(userContainerStream);
        }
    }
}
