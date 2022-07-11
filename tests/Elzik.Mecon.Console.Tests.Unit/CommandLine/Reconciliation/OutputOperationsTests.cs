using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.Plex;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Elzik.Mecon.Console.Tests.Unit.CommandLine.Reconciliation
{
    public class OutputOperationsTests
    {
        private readonly IFixture _fixture;

        private readonly IPlexUsers _mockPlexUsers;
        private readonly OutputOperations _outputOperations;

        public OutputOperationsTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _mockPlexUsers = Substitute.For<IPlexUsers>();

            _outputOperations = new OutputOperations(_mockPlexUsers);
        }

        [Fact]
        public async Task PerformOutputFilters_MissingFromLibraryOption_ReturnsEntriesNotInPlex()
        {
            // Arrange
            var mockEntries = Substitute.For<IMediaEntryCollection>();
            var testEntriesNotInPlex = _fixture.CreateMany<MediaEntry>().ToList();
            mockEntries.WhereNotInPlex().Returns(new MediaEntryCollection(testEntriesNotInPlex));
            var testOptions = new ReconciliationOptions()
            {
                MissingFromLibrary = true
            };

            // Act
            var filteredEntries = await _outputOperations.PerformOutputFilters(mockEntries, testOptions);

            // Assert
            filteredEntries.Should().BeEquivalentTo(testEntriesNotInPlex);
        }

        [Fact]
        public async Task PerformOutputFilters_PresentInLibraryOption_ReturnsEntriesInPlex()
        {
            // Arrange
            var mockEntries = Substitute.For<IMediaEntryCollection>();
            var testEntriesInPlex = _fixture.CreateMany<MediaEntry>().ToList();
            mockEntries.WhereInPlex().Returns(new MediaEntryCollection(testEntriesInPlex));
            var testOptions = new ReconciliationOptions()
            {
                PresentInLibrary = true
            };

            // Act
            var filteredEntries = await _outputOperations.PerformOutputFilters(mockEntries, testOptions);

            // Assert
            filteredEntries.Should().BeEquivalentTo(testEntriesInPlex);
        }

        [Fact]
        public async Task PerformOutputFilters_WatchedByUsersOption_ReturnsEntriesInPlex()
        {
            // Arrange
            var mockEntries = Substitute.For<IMediaEntryCollection>();
            var testEntriesWatchedByUsers = _fixture.CreateMany<MediaEntry>().ToList();
            var testWatchedByUsernames = _fixture.CreateMany<string>().ToList();
            var testWatchedByUserIds = _fixture.CreateMany<int>().ToList();
            mockEntries.WhereWatchedByAccounts(Arg.Is(testWatchedByUserIds)).Returns(new MediaEntryCollection(testEntriesWatchedByUsers));
            _mockPlexUsers.GetAccountIds(Arg.Is<List<string>>(list => list.ContainsAll(testWatchedByUsernames))).Returns(testWatchedByUserIds);
            var testOptions = new ReconciliationOptions()
            {
                WatchedByUsers = testWatchedByUsernames
            };

            // Act
            var filteredEntries = await _outputOperations.PerformOutputFilters(mockEntries, testOptions);

            // Assert
            filteredEntries.Should().BeEquivalentTo(testEntriesWatchedByUsers);
        }

        [Fact]
        public async Task PerformOutputFilters_WatchedByAllUsersOption_ReturnsEntriesInPlex()
        {
            // Arrange
            var mockEntries = Substitute.For<IMediaEntryCollection>();
            var testEntriesWatchedByUsers = _fixture.CreateMany<MediaEntry>().ToList();
            var testWatchedByUsernames = new[] { "!" };
            var testAllUsers = _fixture.CreateMany<PlexUser>().ToList();
            var testAllUsernames = testAllUsers.Select(user => user.UserTitle);
            var testAllUserIds = testAllUsers.Select(user => user.AccountId).ToList();
            mockEntries.WhereWatchedByAccounts(Arg.Is(testAllUserIds)).Returns(new MediaEntryCollection(testEntriesWatchedByUsers));
            _mockPlexUsers.GetAccountIds(Arg.Is<List<string>>(list => list.ContainsAll(testAllUsernames))).Returns(testAllUserIds);
            _mockPlexUsers.GetPlexUsers().Returns(testAllUsers);
            var testOptions = new ReconciliationOptions()
            {
                WatchedByUsers = testWatchedByUsernames
            };

            // Act
            var filteredEntries = await _outputOperations.PerformOutputFilters(mockEntries, testOptions);

            // Assert
            filteredEntries.Should().BeEquivalentTo(testEntriesWatchedByUsers);
        }
    }
}
