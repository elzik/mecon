using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Tests.Unit.Shared;
using FluentAssertions;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Domain
{
    public class FileSystemEntryTests : PropertyTests<FilesystemEntry>
    {
        [Fact]
        public void Type_AfterInstantiation_SHouldBeTypeName()
        {
            // Act
            var fileSystemEntry = new FilesystemEntry();

            // Assert
            fileSystemEntry.Type.Should().Be(nameof(FilesystemEntry));

        }
    }
}
