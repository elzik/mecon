using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Domain.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Elzik.Mecon.Framework.Tests.Unit.Shared;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.PlexModels.Account.User;
using Plex.ServerApi.PlexModels.Server.History;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex
{
    public class PlexPlayHistoryTests
    {
        private readonly IFixture _fixture;

        private readonly IPlexServerClient _mockPlexServerClient;
        private readonly IPlexAccountClient _mockPlexAccountClient;
        private readonly OptionsWrapper<PlexOptions> _testPlexOptions;
        private readonly HistoryMediaContainer _playHistoryContainer1;
        private readonly List<PlayedEntry> _expectedPlayedEntries;

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(PlexPlayHistory).GetConstructors());
        }

        public PlexPlayHistoryTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _testPlexOptions = new OptionsWrapper<PlexOptions>(_fixture.Build<PlexOptions>()
                .With(options => options.ItemsPerCall, 3).Create());

            _mockPlexAccountClient = Substitute.For<IPlexAccountClient>();
            var testHomeUserContainer = EmbeddedResources.GetJsonTestData<HomeUserContainer>("Infrastructure/Plex/TestData/TestHomeUserContainer/TestHomeUserContainer.json");
            _mockPlexAccountClient.GetHomeUsersAsync(_testPlexOptions.Value.AuthToken).Returns(testHomeUserContainer);
            var testHomeAdminId = testHomeUserContainer.Users.Single(user => user.IsAdmin).Id;

            _mockPlexServerClient = Substitute.For<IPlexServerClient>();

            var testPlayHistoryFolder = "Infrastructure/Plex/TestData/TestPlayHistory/";
            _playHistoryContainer1 = EmbeddedResources.GetJsonTestData<HistoryMediaContainer>(testPlayHistoryFolder + "TestPlayHistory1.json");
            _mockPlexServerClient
                .GetPlayHistory(_testPlexOptions.Value.AuthToken, _testPlexOptions.Value.BaseUrl,
                    0, _testPlexOptions.Value.ItemsPerCall).Returns(_playHistoryContainer1);

            var playHistoryContainer2 =
                EmbeddedResources.GetJsonTestData<HistoryMediaContainer>(testPlayHistoryFolder + "TestPlayHistory2.json");
            _mockPlexServerClient
                .GetPlayHistory(_testPlexOptions.Value.AuthToken, _testPlexOptions.Value.BaseUrl,
                   3,  _testPlexOptions.Value.ItemsPerCall).Returns(playHistoryContainer2);

            var playHistoryContainer3 =
                EmbeddedResources.GetJsonTestData<HistoryMediaContainer>(testPlayHistoryFolder + "TestPlayHistory3.json");
            _mockPlexServerClient
                .GetPlayHistory(_testPlexOptions.Value.AuthToken, _testPlexOptions.Value.BaseUrl,
                    6, _testPlexOptions.Value.ItemsPerCall).Returns(playHistoryContainer3);

            _expectedPlayedEntries = new List<PlayedEntry>()
            {
                new PlayedEntry() { AccountId = testHomeAdminId, LibraryKey = "/library/metadata/85769" },
                new PlayedEntry() { AccountId = 64927093, LibraryKey = "/library/metadata/94030" },
                new PlayedEntry() { AccountId = testHomeAdminId, LibraryKey = "/library/metadata/94012" },
                new PlayedEntry() { AccountId = 64927093, LibraryKey = "/library/metadata/94031" },
                new PlayedEntry() { AccountId = testHomeAdminId, LibraryKey = "/library/metadata/89628" },
                new PlayedEntry() { AccountId = testHomeAdminId, LibraryKey = "/library/metadata/94013" }
            };
        }

        [Fact]
        public async Task GetPlayHistory_WithAllItemsInLibrary_ReturnsExpectedHistory()
        {
            // Act
            var plexPlayHistory = new PlexPlayHistory(_mockPlexServerClient, _mockPlexAccountClient, _testPlexOptions);
            var playHistory = await plexPlayHistory.GetPlayHistory();
            var playHistoryList = playHistory.ToList();

            // Assert
            playHistoryList.Should().HaveCount(6);
            playHistoryList.Should().BeEquivalentTo(_expectedPlayedEntries);
        }

        [Fact]
        public async Task GetPlayHistory_WithSomeItemsNotInLibrary_ReturnsExpectedHistory()
        {
            // Arrange
            _playHistoryContainer1.HistoryMetadata.Remove(
                _playHistoryContainer1.HistoryMetadata.Single(metadata => metadata.Key == "/library/metadata/85769"));
            _expectedPlayedEntries.Remove(_expectedPlayedEntries.Single(entry => entry.LibraryKey == "/library/metadata/85769"));
            
            // Act
            var plexPlayHistory = new PlexPlayHistory(_mockPlexServerClient, _mockPlexAccountClient, _testPlexOptions);
            var playHistory = await plexPlayHistory.GetPlayHistory();
            var playHistoryList = playHistory.ToList();

            // Assert
            playHistoryList.Should().HaveCount(5);
            playHistoryList.Should().BeEquivalentTo(_expectedPlayedEntries);
        }

        [Fact]
        public async Task GetPlayHistory_WithNoAdmin_Throws()
        {
            // Arrange
            var testHomeUserContainer = EmbeddedResources.GetJsonTestData<HomeUserContainer>("Infrastructure/Plex/TestData/TestHomeUserContainer/TestHomeUserContainerWithNoAdmins.json");
            _mockPlexAccountClient.GetHomeUsersAsync(_testPlexOptions.Value.AuthToken).Returns(testHomeUserContainer);

            // Act
            var plexPlayHistory = new PlexPlayHistory(_mockPlexServerClient, _mockPlexAccountClient, _testPlexOptions);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => plexPlayHistory.GetPlayHistory());
            
            // Assert
            ex.Message.Should().Be("No home admin users were found.");
        }

        [Fact]
        public async Task GetPlayHistory_WithMultipleAdmins_Throws()
        {
            // Arrange
            var testHomeUserContainer = EmbeddedResources.GetJsonTestData<HomeUserContainer>("Infrastructure/Plex/TestData/TestHomeUserContainer/TestHomeUserContainerWithMultipleAdmins.json");
            _mockPlexAccountClient.GetHomeUsersAsync(_testPlexOptions.Value.AuthToken).Returns(testHomeUserContainer);

            // Act
            var plexPlayHistory = new PlexPlayHistory(_mockPlexServerClient, _mockPlexAccountClient, _testPlexOptions);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => plexPlayHistory.GetPlayHistory());

            // Assert
            ex.Message.Should().Be("More than one home admin users were found.");
        }
    }
}
