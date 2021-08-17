using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.IO;
using System.Linq;
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
            fileSystemOptions?.Value.FolderDefinitions.Should().NotBeNull();
            fileSystemOptions?.Value.FolderDefinitions.Should().HaveCount(3);
            AssertFolderDefinitions(fileSystemOptions, 1);
            AssertFolderDefinitions(fileSystemOptions, 2);
            AssertFolderDefinitions(fileSystemOptions, 3);
        }

        private static void AssertFolderDefinitions(IOptions<FileSystemOptions> fileSystemOptions, int i)
        {
            fileSystemOptions?.Value.FolderDefinitions.Should().Contain(option =>
                option.Name == $"test_name_{i}" &&
                option.FolderPath == $"test_path_{i}" &&
                option.SupportedFileExtensions.SequenceEqual(new[]
                {
                    $"test_ext_{i}_1", $"test_ext_{i}_2", $"test_ext_{i}_3"
                }));
        }
    }
}
