using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex.TestData;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
using Plex.ServerApi.PlexModels.Media;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex
{
    public class PlexEntriesTests
    {
        private readonly IFixture _fixture;
        private readonly IPlexServerClient _mockPlexServerClient;
        private readonly IPlexLibraryClient _mockPlexLibraryClient;
        private readonly MediaContainer _testVideos;
        private readonly LibraryContainer _testLibraries;
        private readonly Library _testMovieLibrary;
        private readonly OptionsWrapper<PlexOptions> _plexOptions;

        private readonly PlexEntries _plexEntries;

        public PlexEntriesTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _testLibraries = _fixture.Create<LibraryContainer>();
            _testMovieLibrary = _fixture
                .Build<Library>()
                .With(library => library.Type, "movie")
                .Create();
            _testVideos = TestMediaContainers.GetVideoMediaContainer();
            _plexOptions = new OptionsWrapper<PlexOptions>(_fixture.Create<PlexOptions>());
            _mockPlexServerClient = Substitute.For<IPlexServerClient>();
            _mockPlexServerClient
                .GetLibrariesAsync(
                    Arg.Is(_plexOptions.Value.AuthToken), 
                    Arg.Is(_plexOptions.Value.BaseUrl))
                .Returns(_testLibraries);
            _mockPlexLibraryClient = Substitute.For<IPlexLibraryClient>();
            _mockPlexLibraryClient.GetLibrarySize(Arg.Is(_plexOptions.Value.AuthToken),
                Arg.Is(_plexOptions.Value.BaseUrl),
                Arg.Is(_testMovieLibrary.Key)).Returns(_testVideos.Media.Count);
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

            _plexEntries = new PlexEntries(_mockPlexServerClient, _mockPlexLibraryClient, _plexOptions);
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
                new PlexEntries(_mockPlexServerClient, _mockPlexLibraryClient, nullPlexOptions));
            
            // Assert
            ex.Message.Should().Be("Value of plexOptions must not be null.");
        }

        [Fact]
        public async Task GetPlexEntries_WithoutVideoLibrary_ReturnsNoPlexEntries()
        {
            // Act
            var plexItems = await _plexEntries.GetPlexEntries();

            // Assert
            plexItems.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPlexEntries_WithVideoLibrary_ReturnsPlexEntries()
        {
            // Arrange
            _testLibraries.Libraries.Add(_testMovieLibrary);

            // Act
            var plexItems = await _plexEntries.GetPlexEntries();

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
    }
}
