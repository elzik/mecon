using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.IO;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Framework.Tests.Integration.Infrastructure.Plex.Options
{
    public class PlexOptionsTests
    {
        [Fact]
        public void GetService_ForPlexOptions_ReturnsExpectedOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Act
            services.Configure<PlexOptions>(configuration.GetSection("Plex"));
            var serviceProvider = services.BuildServiceProvider();
            var plexOptions = serviceProvider.GetService<IOptions<PlexOptions>>();

            // Assert
            plexOptions.Should().NotBeNull();
            plexOptions?.Value.Should().NotBeNull();
            plexOptions?.Value.AuthToken.Should().Be("test_token");
            plexOptions?.Value.BaseUrl.Should().Be("test_base_url");
        }
    }
}
