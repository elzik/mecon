using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Elzik.Mecon.Console.Tests.Unit
{
    public class ConfigurationTests
    {
        private readonly IFixture _fixture;

        public ConfigurationTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void ToJsonString_WithValidConfig_ReturnsExpectedJson()
        {
            // Arrange
            var config = new ConfigurationManager();
            config.AddJsonFile("appsettings.Test.json");

            // Act
            var jsonText = config.ToJsonString();

            // Assert
            var expected = File.ReadAllText("appsettings.Test.json")
                .Replace("TestPlexAuthToken", "<set-but-redacted>");
            jsonText.Should().Be(expected);
        }

        [Fact]
        public void Handle_CalledWithPlexOptions_SetsPlexConfiguration()
        {
            // Arrange
            var testPlexHost = _fixture.Create<string>();
            var testPlexToken = _fixture.Create<string>();
            var testCommandLineArgs = new[] { "-p", testPlexHost, "-t", testPlexToken, "-l"};

            // Act
            var configManager = Configuration.Get(testCommandLineArgs);

            // Assert
            var config = ((IConfigurationBuilder)configManager).Build();
            config.GetValue<string>("Plex:BaseUrl").Should().Be(testPlexHost);
            config.GetValue<string>("Plex:AuthToken").Should().Be(testPlexToken);
        }
    }
}