using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.IO;
using Elzik.Mecon.Framework.Infrastructure.FileSystem.Options;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Framework.Tests.Integration.Infrastructure.FileSystem.Options
{
    public class PlexOptionsTests
    {
        [Fact]
        public void GetService_ForFileSystemOptions_ReturnsExpectedOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();


            // Act
            services.Configure<FileSystemOptions>(configuration.GetSection("FileSystem"));
            var serviceProvider = services.BuildServiceProvider();
            var fileSystemOptions = serviceProvider.GetService<IOptions<FileSystemOptions>>();

            // Assert
            fileSystemOptions.Should().NotBeNull();
            fileSystemOptions?.Value.Should().NotBeNull();
            fileSystemOptions?.Value.DirectoryDefinitions.Should().NotBeNull();
            fileSystemOptions?.Value.DirectoryDefinitions.Should().HaveCount(3);
            AssertDirectoryDefinitions(fileSystemOptions, 1);
            AssertDirectoryDefinitions(fileSystemOptions, 2);
            AssertDirectoryDefinitions(fileSystemOptions, 3);
        }

        private static void AssertDirectoryDefinitions(IOptions<FileSystemOptions> fileSystemOptions, int i)
        {
            fileSystemOptions?.Value.DirectoryDefinitions[$"test_name_{i}"].DirectoryPath.Should().Be($"test_path_{i}");
            fileSystemOptions?.Value.DirectoryDefinitions[$"test_name_{i}"].SupportedFileExtensions.Should()
                .BeEquivalentTo(new[]
                {
                    $"test_ext_{i}_1", $"test_ext_{i}_2", $"test_ext_{i}_3"
                });
            // TODO: Not all options tested here
        }
    }
}
