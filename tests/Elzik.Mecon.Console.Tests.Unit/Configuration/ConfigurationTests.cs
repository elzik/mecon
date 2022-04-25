using Microsoft.Extensions.Configuration;
using Elzik.Mecon.Console.Configuration;
using FluentAssertions;
using Xunit;

namespace Elzik.Mecon.Console.Tests.Unit.Configuration
{
    public class ConfigurationTests
    {
        [Fact]
        public void ToJsonString_WithValidConfig_ReturnsExpectedJason()
        {
            // Arrange
            var config = new ConfigurationManager();
            config.AddJsonFile("Configuration/appsettings.Test.json");

            // Act
            var jsonText = config.ToJsonString();

            // Assert
            var expected = File.ReadAllText("Configuration/appsettings.Test.json")
                .Replace("TestPlexAuthToken", "<set-but-redacted>");
            jsonText.Should().Be(expected);
        }
    }
}