using System;
using System.Linq;
using AutoFixture;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using FluentAssertions;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex
{
    public class MediaTypeExtensionsTests
    {
        private readonly IFixture _fixture;

        public MediaTypeExtensionsTests()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(MediaType.Movie, "movie")]
        [InlineData(MediaType.TvShow, "show")]
        public void ToPlexLibraryType_WithValidMediaType_ReturnsPLexLibraryType(
            MediaType testMediaType, 
            string expectedLibraryType)
        {
            // Act
            var libraryType = testMediaType.ToPlexLibraryType();

            // Assert
            libraryType.Should().Be(expectedLibraryType);
        }

        [Fact]
        public void ToPlexLibraryType_WithInvalidMediaType_Throws()
        {
            var testMediaType = (MediaType) int.MinValue;

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => 
                testMediaType.ToPlexLibraryType());

            // Assert
            ex.Message.Should().Be($"MediaType {int.MinValue} cannot be converted to a Plex library type.");
        }

        [Fact]
        public void ToPlexMediaTypes_WithValidMediaTypes_ReturnsPLexLibraryType()
        {
            // Arrange
            var testMediaTypes = _fixture.CreateMany<MediaType>().Distinct().ToArray();

            // Act
            var libraryTypes = testMediaTypes.ToPlexLibraryTypes();

            // Assert
            libraryTypes.Should().BeEquivalentTo(
                testMediaTypes.Select(type => type.ToPlexLibraryType()));

        }

        [Fact]
        public void ToPlexMediaTypes_WithNullMediaTypes_ReturnsAllPLexLibraryType()
        {
            // Act
            var libraryTypes = ((MediaType[]) null).ToPlexLibraryTypes();

            // Assert
            libraryTypes.Should().BeEquivalentTo("movie", "show");

        }

        [Fact]
        public void ToPlexMediaTypes_WithNoMediaTypes_ReturnsAllPLexLibraryType()
        {
            // Act
            var libraryTypes = (Array.Empty<MediaType>().ToPlexLibraryTypes());

            // Assert
            libraryTypes.Should().BeEquivalentTo("movie", "show");

        }
    }
}
