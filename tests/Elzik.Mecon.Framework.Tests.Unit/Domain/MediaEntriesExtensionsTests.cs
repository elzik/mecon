using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Framework.Domain;
using FluentAssertions;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Domain
{
    public class MediaEntriesExtensionsTests
    {
        private readonly IFixture _fixture;
        public MediaEntriesExtensionsTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void WhereNotInPlex_WithNoEntries_ReturnsNoEntries()
        {
            // Arrange
            var testEmptyCollection = Array.Empty<MediaEntry>();

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
            var testVariousEntries = GetOnlyPlexEntries().Concat(testNonPlexEntries);

            // Act
            var entriesNotInPlex = testVariousEntries.WhereNotInPlex();

            // Assert
            entriesNotInPlex.Should().BeEquivalentTo(testNonPlexEntries);
        }

        [Fact]
        public void WhereInPlex_WithNoEntries_ReturnsNoEntries()
        {
            // Arrange
            var testEmptyCollection = Array.Empty<MediaEntry>();

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
            var testVariousEntries = GetOnlyNonPlexEntries().Concat(testPlexEntries);

            // Act
            var entriesNotInPlex = testVariousEntries.WhereInPlex();

            // Assert
            entriesNotInPlex.Should().BeEquivalentTo(testPlexEntries);
        }

        [Fact]
        public void WhereMatchRegex_MatchingTestWord_ReturnsExpectedMatches()
        {
            // Arrange
            var testStrings = GetMediaEntriesForTestWord();
            var testRegExPattern = "^.*TestWord.*$";

            //Act
            var matchedStrings = testStrings.WhereMatchRegex(testRegExPattern);

            //Assert
            matchedStrings.Should()
                .BeEquivalentTo(testStrings.Where(entry => 
                    entry.FilesystemEntry.FileSystemPath.Contains("TestWord")));
        }

        [Fact]
        public void WhereNoMatchRegex_NotMatchingTestWord_ReturnsExpectedMatches()
        {
            // Arrange
            var testStrings = GetMediaEntriesForTestWord();
            var testRegExPattern = "^.*TestWord.*$";

            //Act
            var matchedStrings = testStrings.WhereNoMatchRegex(testRegExPattern);

            //Assert
            matchedStrings.Should()
                .BeEquivalentTo(testStrings.Where(entry =>
                    !entry.FilesystemEntry.FileSystemPath.Contains("TestWord")));
        }

        private List<MediaEntry> GetOnlyNonPlexEntries()
        {
            return _fixture.CreateMany<MediaEntry>().ToList();
        }

        private List<MediaEntry> GetOnlyPlexEntries()
        {
            var testPlexEntries = _fixture.CreateMany<MediaEntry>().ToList();

            foreach (var plexEntry in testPlexEntries)
            {
                plexEntry.ReconciledEntries.Clear();
                plexEntry.ReconciledEntries.Add(_fixture.Create<Framework.Domain.PlexEntry>());
            }

            return testPlexEntries;
        }

        private static MediaEntry[] GetMediaEntriesForTestWord()
        {
            var testStrings = new[]
            {
                new MediaEntry("Example 1 without"),
                new MediaEntry("Example 2 with TestWord"),
                new MediaEntry("Example 3 without"),
                new MediaEntry("Example 4 with TestWord"),
                new MediaEntry("Example 5 without"),
                new MediaEntry("Example 6 with TestWord")
            };

            return testStrings;
        }
    }
}
