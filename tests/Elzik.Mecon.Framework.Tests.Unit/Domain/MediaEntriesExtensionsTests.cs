using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Framework.Domain;
using FluentAssertions;
using NSubstitute;
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
    }
}
