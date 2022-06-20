﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Domain.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
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

        public PlexUsersTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _testPlexOptions = new OptionsWrapper<PlexOptions>(_fixture.Create<PlexOptions>());

            _mockPlexAccountClient = Substitute.For<IPlexAccountClient>();
            var testHomeUserContainer = GetXmlTestData<HomeUserContainer>("Infrastructure/Plex/TestData/TestHomeUserContainer.xml");
            _mockPlexAccountClient.GetHomeUsersAsync(_testPlexOptions.Value.AuthToken).Returns(testHomeUserContainer);
            var testFriends = GetJsonTestData<List<Friend>>("Infrastructure/Plex/TestData/TestFriends.json");
            _mockPlexAccountClient.GetFriendsAsync(_testPlexOptions.Value.AuthToken).Returns(testFriends);
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
            var plexUsers = new PlexUsers(_mockPlexAccountClient, _testPlexOptions);

            var users = await plexUsers.GetPlexUsers();

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

        private static T GetXmlTestData<T>(string projectFilePath)
        {
            var dataStream = GetEmbeddedResourceStream(projectFilePath);

            var xmlSerialiser = new XmlSerializer(typeof(T));
            var data = (T)xmlSerialiser.Deserialize(dataStream);

            return data;
        }

        private static T GetJsonTestData<T>(string projectFilePath)
        {
            var dataStream = GetEmbeddedResourceStream(projectFilePath);

            var data = JsonSerializer.Deserialize<T>(dataStream);

            return data;
        }

        private static Stream GetEmbeddedResourceStream(string projectFilePath)
        {
            var assemblyName = typeof(PlexUsersTests).Assembly.GetName().Name;
            var resourceStreamName = $"{assemblyName}.{projectFilePath.TrimStart('/').Replace('/', '.')}";

            var embeddedResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceStreamName);
            if (embeddedResourceStream == null)
            {
                throw new InvalidOperationException($"{resourceStreamName} embedded resource not found.");
            }

            return embeddedResourceStream;
        }
    }
}