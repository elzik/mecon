using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex.ApiClients
{
    public class PlexHeaderHandlerTests
    {
        private readonly IFixture _fixture;

        public PlexHeaderHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(PlexHeaderHandler).GetConstructors());
        }

        [Fact]
        public void Constructor_WithoutPlexOptions_Throws()
        {
            // Arrange
            var testNullPlexOptions = new OptionsWrapper<PlexOptions>(null);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() =>
                new PlexHeaderHandler(testNullPlexOptions));

            // Assert
            ex.Message.Should().Be("Value of options must not be null.");
        }

        [Fact]
        public void Constructor_WithoutToken_Throws()
        {
            // Arrange
            var plexOptions = _fixture.Build<PlexOptions>().Without(options => options.AuthToken).Create();
            var testWrappedPlexOptionsWithoutToken = new OptionsWrapper<PlexOptions>(plexOptions);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() =>
                new PlexHeaderHandler(testWrappedPlexOptionsWithoutToken));

            // Assert
            ex.Message.Should().Be("PlexOptions must contain an AuthToken.");
        }
    }
}
