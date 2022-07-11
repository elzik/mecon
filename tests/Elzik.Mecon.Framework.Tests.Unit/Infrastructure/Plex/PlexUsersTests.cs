using System;
using System.Collections.Generic;
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
using Plex.ServerApi.PlexModels.Account;
using Plex.ServerApi.PlexModels.Account.User;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex
{
    public class PlexUsersTests
    {
        private readonly IFixture _fixture;

        private readonly IPlexAccountClient _mockPlexAccountClient;
        private readonly OptionsWrapper<PlexOptions> _testPlexOptions;
        private PlexUsers _plexUsers;

        public PlexUsersTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _testPlexOptions = new OptionsWrapper<PlexOptions>(_fixture.Create<PlexOptions>());

            _mockPlexAccountClient = Substitute.For<IPlexAccountClient>();
            var testHomeUserContainer = EmbeddedResources.GetJsonTestData<HomeUserContainer>("Infrastructure/Plex/TestData/TestHomeUserContainer/TestHomeUserContainer.json");
            _mockPlexAccountClient.GetHomeUsersAsync(_testPlexOptions.Value.AuthToken).Returns(testHomeUserContainer);
            var testFriends = EmbeddedResources.GetJsonTestData<List<Friend>>("Infrastructure/Plex/TestData/TestFriends.json");
            _mockPlexAccountClient.GetFriendsAsync(_testPlexOptions.Value.AuthToken).Returns(testFriends);
            _plexUsers = new PlexUsers(_mockPlexAccountClient, _testPlexOptions);
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(PlexUsers).GetConstructors());
        }

        [Fact]
        public void Constructor_WithoutPlexOptions_Throws()
        {
            // Arrange
            var nullPlexOptions = new OptionsWrapper<PlexOptions>(null);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() =>
                new PlexUsers(_mockPlexAccountClient, nullPlexOptions));

            // Assert
            ex.Message.Should().Be("Value of plexOptions must not be null.");
        }

        [Fact]
        public async Task GetPlexUsers_WithValidUsers_ReturnsUsers()
        {
            // Act
            var users = await _plexUsers.GetPlexUsers();

            // Assert
            users.Should().BeEquivalentTo(new[]
            {
                new PlexUser() { UserTitle = "FriendOneTitle", AccountId = 648167784 },
                new PlexUser() { UserTitle = "FriendTwoTitle", AccountId = 132111983},
                new PlexUser() { UserTitle = "UserAdminTitle", AccountId = 39388434 },
                new PlexUser() { UserTitle = "UserOneTitle", AccountId = 78624707 },
                new PlexUser() { UserTitle = "UserThreeTitle", AccountId = 64927093 },
                new PlexUser() { UserTitle = "UserTwoTitle", AccountId = 92140768 }
            }, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task GetAccountIds_WithValidTitles_ReturnsAccountIds()
        {
            // Arrange
            var testUserTitles = new string[]
            {
                "FriendOneTitle", "FriendTwoTitle", "UserAdminTitle", "UserOneTitle", "UserThreeTitle", "UserTwoTitle"
            };
            var expectedUserIds = new int[]
            {                
                648167784, 132111983, 39388434, 78624707, 64927093, 92140768
            };

            // Act
            var accountIds = await _plexUsers.GetAccountIds(testUserTitles);

            // Assert
            accountIds.Should().BeEquivalentTo(expectedUserIds);
        }

        [Fact]
        public async Task GetAccountIds_WithLowerCasedTitles_IgnoresCaseAndReturnsAccountIds()
        {
            // Arrange
            var testUserTitles = new string[]
            {
                "friendonetitle", "friendtwotitle", "useradmintitle", "useronetitle", "userthreetitle", "usertwotitle"
            };
            var expectedUserIds = new int[]
            {
                648167784, 132111983, 39388434, 78624707, 64927093, 92140768
            };

            // Act
            var accountIds = await _plexUsers.GetAccountIds(testUserTitles);

            // Assert
            accountIds.Should().BeEquivalentTo(expectedUserIds);
        }

        [Fact]
        public async Task GetAccountIds_WithUnknownUser_Throws()
        {
            // Arrange
            var testUserTitles = new string[]
            {
                "FriendOneTitle", "Unknown"
            };

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _plexUsers.GetAccountIds(testUserTitles));

            // Assert
            ex.Message.Should().Be("User with title 'Unknown' does not exist.");
        }
    }
}
