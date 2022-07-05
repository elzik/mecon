using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Domain;
using FluentAssertions;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Domain
{
    public class MediaEntriesTests
    {
        private readonly IFixture _fixture;
        public MediaEntriesTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(MediaEntryCollection).GetConstructors());
        }

        [Fact]
        public void WhereNotInPlex_WithNoEntries_ReturnsNoEntries()
        {
            // Arrange
            var testEmptyCollection = new MediaEntryCollection();

            // Act
            var entriesNotInPlex = testEmptyCollection.WhereNotInPlex();

            // Assert
            entriesNotInPlex.Should().BeEmpty();
        }

        [Fact]
        public void WhereNotInPlex_WithNoPlexEntries_ReturnsAllEntries()
        {
            // Arrange
            var nonPlexEntries = GetOnlyNonPlexEntries();

            // Act
            var entriesNotInPlex = nonPlexEntries.WhereNotInPlex();

            // Assert
            entriesNotInPlex.Should().BeEquivalentTo(nonPlexEntries);
        }

        [Fact]
        public void WhereNotInPlex_WithOnlyPlexEntries_ReturnsNoEntries()
        {
            // Arrange
            var plexEntries = GetOnlyPlexEntries();

            // Act
            var entriesNotInPlex = plexEntries.WhereNotInPlex();

            // Assert
            entriesNotInPlex.Should().BeEmpty();
        }

        [Fact]
        public void WhereNotInPlex_WithVariousEntries_ReturnsOnlyNonPlexEntries()
        {
            // Arrange
            var testNonPlexEntries = GetOnlyNonPlexEntries();
            var testVariousEntries = GetOnlyPlexEntries();
            testVariousEntries.AddRange(testNonPlexEntries);

            // Act
            var entriesNotInPlex = testVariousEntries!.WhereNotInPlex();

            // Assert
            entriesNotInPlex.Should().BeEquivalentTo(testNonPlexEntries);
        }

        [Fact]
        public void WhereInPlex_WithNoEntries_ReturnsNoEntries()
        {
            // Arrange
            var testEmptyCollection = new MediaEntryCollection();

            // Act
            var entriesNotInPlex = testEmptyCollection.WhereInPlex();

            // Assert
            entriesNotInPlex.Should().BeEmpty();
        }

        [Fact]
        public void WhereInPlex_WithNoPlexEntries_ReturnsNoEntries()
        {
            // Arrange
            var nonPlexEntries = GetOnlyNonPlexEntries();

            // Act
            var entriesNotInPlex = nonPlexEntries.WhereInPlex();

            // Assert
            entriesNotInPlex.Should().BeEmpty();
        }

        [Fact]
        public void WhereInPlex_WithOnlyPlexEntries_ReturnsAllEntries()
        {
            // Arrange
            var plexEntries = GetOnlyPlexEntries();

            // Act
            var entriesNotInPlex = plexEntries.WhereInPlex();

            // Assert
            entriesNotInPlex.Should().BeEquivalentTo(plexEntries);
        }

        [Fact]
        public void WhereInPlex_WithVariousEntries_ReturnsOnlyPlexEntries()
        {
            // Arrange
            var testPlexEntries = GetOnlyPlexEntries();
            var testVariousEntries = GetOnlyNonPlexEntries();
            testVariousEntries.AddRange(testPlexEntries);

            // Act
            var entriesNotInPlex = testVariousEntries!.WhereInPlex();

            // Assert
            entriesNotInPlex.Should().BeEquivalentTo(testPlexEntries);
        }

        [Fact]
        public void WhereWatchedByAccounts_WithNullAccounts_Throws()
        {
            // Arrange
            var testEntries = new MediaEntryCollection();

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => testEntries.WhereWatchedByAccounts(null));

            // Assert
            ex.ParamName.Should().Be("accountIds");
        }

        [Fact]
        public void WhereWatchedByAccounts_WithNoAccounts_Throws()
        {
            // Arrange
            var testEntries = _fixture.Create<MediaEntryCollection>();

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => testEntries.WhereWatchedByAccounts(Array.Empty<int>()));

            // Assert
            ex.Message.Should().Be("At least one account ID must be specified.");
        }

        [Fact]
        public void WhereWatchedByAccounts_WithOneAccount_ReturnsOnlyPlexEntriesWatchedByThatAccount()
        {
            // Arrange
            var testPlexEntries = GetOnlyPlexEntries();
            var testVariousEntries = GetOnlyNonPlexEntries(); 
            testVariousEntries.AddRange(testPlexEntries);

            var testWatchedByAccount = new[] { _fixture.Create<int>() };
            var mediaEntry = testVariousEntries!.First(entry => entry.ReconciledEntries.First() is Framework.Domain.Plex.PlexEntry);
            var testPlexEntry = (Framework.Domain.Plex.PlexEntry)mediaEntry.ReconciledEntries.First();
            testPlexEntry.WatchedByAccounts = testWatchedByAccount;

            // Act
            var watchedEntries = testVariousEntries.WhereWatchedByAccounts(testWatchedByAccount);

            // Assert
            watchedEntries.Should().ContainSingle(entry => entry == mediaEntry);
        }

        [Fact]
        public void WhereWatchedByAccounts_WithMultipleAccounts_ReturnsOnlyPlexEntriesWatchedByThoseAccounts()
        {
            // Arrange
            var testPlexEntries = GetOnlyPlexEntries();
            var testVariousEntries = GetOnlyNonPlexEntries();
            testVariousEntries.AddRange(testPlexEntries);

            var watchedByAccounts = _fixture.CreateMany<int>().ToList();
            var mediaEntry = testVariousEntries!.First(entry => entry.ReconciledEntries.First() is Framework.Domain.Plex.PlexEntry);
            var testPlexEntry = (Framework.Domain.Plex.PlexEntry)mediaEntry.ReconciledEntries.First();
            testPlexEntry.WatchedByAccounts = watchedByAccounts;

            // Act
            var watchedEntries = testVariousEntries.WhereWatchedByAccounts(watchedByAccounts);

            // Assert
            watchedEntries.Should().ContainSingle(entry => entry == mediaEntry);
        }

        [Fact]
        public void WhereWatchedByAccounts_WithMultipleAccountsAndWatchedByOnlyASubset_ReturnsNoEntries()
        {
            // Arrange
            var testPlexEntries = GetOnlyPlexEntries();
            var testVariousEntries = GetOnlyNonPlexEntries();
            testVariousEntries.AddRange(testPlexEntries);

            var watchedByAccounts = _fixture.CreateMany<int>().ToList();
            var mediaEntry = testVariousEntries!.First(entry => entry.ReconciledEntries.First() is Framework.Domain.Plex.PlexEntry);
            var testPlexEntry = (Framework.Domain.Plex.PlexEntry)mediaEntry.ReconciledEntries.First();
            var watchedByAccountsSubset = watchedByAccounts.Take(watchedByAccounts.Count -1);
            testPlexEntry.WatchedByAccounts = watchedByAccountsSubset;

            // Act
            var watchedEntries = testVariousEntries.WhereWatchedByAccounts(watchedByAccounts);

            // Assert
            watchedEntries.Should().BeEmpty();
        }

        [Fact]
        public void WhereWatchedByAccounts_WithMultipleAccountsAndMultipleEntriesWatched_ReturnsOnlyPlexEntriesWatchedByThoseAccounts()
        {
            // Arrange
            var testPlexEntries = GetOnlyPlexEntries();
            var testVariousEntries = GetOnlyNonPlexEntries();
            testVariousEntries.AddRange(testPlexEntries);

            var watchedByAccounts = _fixture.CreateMany<int>().ToList(); 
            var mediaEntries = new MediaEntryCollection()
            {
                testVariousEntries!.First(entry => entry.ReconciledEntries.First() is Framework.Domain.Plex.PlexEntry),
                testVariousEntries.Last(entry => entry.ReconciledEntries.First() is Framework.Domain.Plex.PlexEntry)
            };
            foreach (var mediaEntry in mediaEntries)
            {
                ((Framework.Domain.Plex.PlexEntry)mediaEntry.ReconciledEntries.First()).WatchedByAccounts =
                    watchedByAccounts;
            }

            // Act
            var watchedEntries = testVariousEntries.WhereWatchedByAccounts(watchedByAccounts);

            // Assert
            watchedEntries.Should().BeEquivalentTo(mediaEntries);
        }

        [Fact]
        public void AddRange_WithValidEntries_AddsAll()
        {
            // Arrange
            var testMediaEntriesToAdd = _fixture.CreateMany<MediaEntry>().ToList();
            var mediaEntries = new MediaEntryCollection();

            // Act
            mediaEntries.AddRange(testMediaEntriesToAdd);

            // Assert
            mediaEntries.Should().BeEquivalentTo(testMediaEntriesToAdd);
        }

        [Fact]
        public void AddRange_WithNullEntries_Throws()
        {
            // Arrange
            var testMediaEntries = new MediaEntryCollection();

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => 
                testMediaEntries.AddRange(null));

            // Assert
            ex.ParamName.Should().Be("values");
        }

        [Fact]
        public void Instantiated_WithValidEntries_AddsAll()
        {
            // Arrange
            var testMediaEntriesToAdd = _fixture.CreateMany<MediaEntry>().ToList();

            // Act
            var mediaEntries = new MediaEntryCollection(testMediaEntriesToAdd);

            // Assert
            mediaEntries.Should().BeEquivalentTo(testMediaEntriesToAdd);
        }

        private MediaEntryCollection GetOnlyNonPlexEntries()
        {
            var nonPlexEntries = new MediaEntryCollection();
            nonPlexEntries.AddMany(() => _fixture.Create<MediaEntry>(), 3);

            return nonPlexEntries;
        }

        private MediaEntryCollection GetOnlyPlexEntries()
        {
            var testPlexEntries = GetOnlyNonPlexEntries();

            foreach (var plexEntry in testPlexEntries)
            {
                plexEntry.ReconciledEntries.Clear();
                plexEntry.ReconciledEntries.Add(_fixture.Create<Framework.Domain.Plex.PlexEntry>());
            }

            return testPlexEntries;
        }
    }
}
