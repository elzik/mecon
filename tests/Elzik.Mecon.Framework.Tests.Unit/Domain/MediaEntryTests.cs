using AutoFixture;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Tests.Unit.Shared;
using FluentAssertions;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Domain
{
    public class MediaEntryTests : PropertyTests<MediaEntry>
    {
        [Fact]
        public void FilesystemEntry_AfterInstantiation_HasDefaultValue()
        {
            // Arrange
            var testPath = Fixture.Create<string>();
            var mediaEntry = new MediaEntry(testPath);

            // Assert
            mediaEntry.FilesystemEntry.Should().NotBeNull();
            mediaEntry.FilesystemEntry.FileSystemPath.Should().Be(testPath);
        }

        [Fact]
        public void ReconciledEntries_AfterInstantiation_HasDefaultValue()
        {
            // Arrange
            var testPath = Fixture.Create<string>();
            var mediaEntry = new MediaEntry(testPath);

            // Assert
            mediaEntry.ReconciledEntries.Should().BeEmpty();
        }
    }
}
