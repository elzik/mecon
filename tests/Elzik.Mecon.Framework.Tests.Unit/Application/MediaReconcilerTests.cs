using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.FileSystem;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using IFileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.IFileSystem;

namespace Elzik.Mecon.Framework.Tests.Unit.Application
{
    public class MediaReconcilerTests
    {
        private readonly IFixture _fixture;

        private readonly MockLogger<MediaReconciler> _mockLogger;
        private readonly IFileSystem _mockFileSystem;
        private readonly IPlexEntries _mockPlexEntries;
        private readonly OptionsWrapper<PlexWithCachingOptions> _testPlexOptionsWrapper;
        private readonly FolderDefinition _testFolderDefinition;
        private readonly List<FileSystemTests.TestFileInfoImplementation> _testFiles;
        private readonly List<PlexEntry> _testPlexEntries;

        public MediaReconcilerTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _mockLogger = Substitute.For<MockLogger<MediaReconciler>>();
            _mockFileSystem = Substitute.For<IFileSystem>();
            _mockPlexEntries = Substitute.For<IPlexEntries>();

            var testPlexOptions = _fixture.Create<PlexWithCachingOptions>();
            _testPlexOptionsWrapper = new OptionsWrapper<PlexWithCachingOptions>(testPlexOptions);

            _testFolderDefinition = _fixture.Create<FolderDefinition>();

            _testFiles = _fixture.CreateMany<FileSystemTests.TestFileInfoImplementation>().ToList();
            _mockFileSystem.GetFolderDefinition(_testFolderDefinition.Name).Returns(_testFolderDefinition);
            _mockFileSystem.GetMediaFileInfos(
                Arg.Is(_testFolderDefinition.FolderPath), 
                Arg.Is(_testFolderDefinition.SupportedFileExtensions))
            .Returns(_testFiles);

            _testPlexEntries = _fixture.CreateMany<PlexEntry>().ToList();
            _mockPlexEntries.GetPlexEntries().Returns(_testPlexEntries);
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(MediaReconciler).GetConstructors());
        }

        [Fact]
        public async Task GetMediaEntries_AllFilesHaveMatchingPlexEntries_ReturnsExpectedMediaEntries()
        {
            // Arrange
            Aligner.AlignFileSystemWithPlexMediaContainer(_testFiles, _testPlexEntries);

            // Act
            var mediaReconciler =
                new MediaReconciler(_mockLogger, _mockFileSystem, _mockPlexEntries, _testPlexOptionsWrapper);
            var mediaEntries = await mediaReconciler.GetMediaEntries(_testFolderDefinition.Name);

            // Assert
            var mediaEntryList = mediaEntries.ToList();
            mediaEntryList.Should().NotBeNull();
            mediaEntryList.Should().HaveSameCount(_testFiles);
            mediaEntryList.Should().HaveSameCount(_testPlexEntries);
            for (var index = 0; index < mediaEntryList.Count; index++)
            {
                var mediaEntry = mediaEntryList[index];
                var testFileInfo = _testFiles[index];
                var testPlexEntry = _testPlexEntries[index];

                mediaEntry.FilesystemEntry.Key.Filename.Should().Be(testFileInfo.Name);
                mediaEntry.FilesystemEntry.Key.ByteCount.Should().Be(testFileInfo.Length);
                mediaEntry.FilesystemEntry.Key.Should().BeEquivalentTo(testPlexEntry.Key);

                mediaEntry.FilesystemEntry.FileSystemPath.Should().Be(testFileInfo.FullName);
                mediaEntry.FilesystemEntry.Title.Should().Be(testFileInfo.Name);
                mediaEntry.FilesystemEntry.Type.Should().Be("FilesystemEntry");

                mediaEntry.ReconciledEntries.Should().HaveCount(1);

                mediaEntry.ReconciledEntries.Single().Should().BeEquivalentTo(testPlexEntry);

                mediaEntry.ThumbnailUrl.Should().Be(testPlexEntry.ThumbnailUrl);

            }
        }

        [Fact]
        public async Task GetMediaEntries_NoFilesHaveMatchingPlexEntries_ReturnsExpectedMediaEntries()
        {
            // Act
            var mediaReconciler =
                new MediaReconciler(_mockLogger, _mockFileSystem, _mockPlexEntries, _testPlexOptionsWrapper);
            var mediaEntries = await mediaReconciler.GetMediaEntries(_testFolderDefinition.Name);

            // Assert
            var mediaEntryList = mediaEntries.ToList();
            mediaEntryList.Should().NotBeNull();
            mediaEntryList.Should().HaveSameCount(_testFiles);
            for (var index = 0; index < mediaEntryList.Count; index++)
            {
                var mediaEntry = mediaEntryList[index];
                var testFileInfo = _testFiles[index];
                
                mediaEntry.FilesystemEntry.Key.Filename.Should().Be(testFileInfo.Name);
                mediaEntry.FilesystemEntry.Key.ByteCount.Should().Be(testFileInfo.Length);

                mediaEntry.FilesystemEntry.FileSystemPath.Should().Be(testFileInfo.FullName);
                mediaEntry.FilesystemEntry.Title.Should().Be(testFileInfo.Name);
                mediaEntry.FilesystemEntry.Type.Should().Be("FilesystemEntry");

                mediaEntry.ReconciledEntries.Should().BeEmpty();

                mediaEntry.ThumbnailUrl.Should().BeNull();

            }
        }

        [Theory]
        [InlineData(null, "test")]
        [InlineData("", "test")]
        [InlineData(" ", "test")]
        [InlineData("   ", "test")]
        [InlineData("test", null)]
        [InlineData("test", "")]
        [InlineData("test", " ")]
        [InlineData("test", "   ")]
        public async Task GetMediaEntries_PlexIsDisabled_ReturnsExpectedMediaEntries(string authToken, string baseUrl)
        {
            // Arrange
            Aligner.AlignFileSystemWithPlexMediaContainer(_testFiles, _testPlexEntries);
            _testPlexOptionsWrapper.Value.AuthToken = authToken;
            _testPlexOptionsWrapper.Value.BaseUrl = baseUrl;
            
            // Act
            var mediaReconciler =
                new MediaReconciler(_mockLogger, _mockFileSystem, _mockPlexEntries, _testPlexOptionsWrapper);
            var mediaEntries = await mediaReconciler.GetMediaEntries(_testFolderDefinition.Name);

            // Assert
            var mediaEntryList = mediaEntries.ToList();
            mediaEntryList.Should().NotBeNull();
            mediaEntryList.Should().HaveSameCount(_testFiles);
            for (var index = 0; index < mediaEntryList.Count; index++)
            {
                var mediaEntry = mediaEntryList[index];
                var testFileInfo = _testFiles[index];

                mediaEntry.FilesystemEntry.Key.Filename.Should().Be(testFileInfo.Name);
                mediaEntry.FilesystemEntry.Key.ByteCount.Should().Be(testFileInfo.Length);

                mediaEntry.FilesystemEntry.FileSystemPath.Should().Be(testFileInfo.FullName);
                mediaEntry.FilesystemEntry.Title.Should().Be(testFileInfo.Name);
                mediaEntry.FilesystemEntry.Type.Should().Be("FilesystemEntry");

                mediaEntry.ReconciledEntries.Should().BeEmpty();

                mediaEntry.ThumbnailUrl.Should().BeNull();

            }
        }

        [Fact]
        public void GetMediaEntries_PlexIsEnabledWithCaching_Logs()
        {
            // Act
            _ = new MediaReconciler(_mockLogger, _mockFileSystem, _mockPlexEntries, _testPlexOptionsWrapper);

            // Assert
            _mockLogger.Received(1)
                .Log(Arg.Any<LogLevel>(), Arg.Is<string>(s =>
                    s.Contains($"Plex reconciliation is enabled against {_testPlexOptionsWrapper.Value.BaseUrl} " +
                               $"with a cache expiration of {_testPlexOptionsWrapper.Value.CacheExpiry} seconds.")));
        }

        [Fact]
        public void GetMediaEntries_PlexIsEnabledWithoutCaching_Logs()
        {
            // Arrange
            _testPlexOptionsWrapper.Value.CacheExpiry = null;

            // Act
            _ = new MediaReconciler(_mockLogger, _mockFileSystem, _mockPlexEntries, _testPlexOptionsWrapper);

            // Assert
            _mockLogger.Received(1)
                .Log(Arg.Any<LogLevel>(), Arg.Is<string>(s =>
                    s.Contains($"Plex reconciliation is enabled against {_testPlexOptionsWrapper.Value.BaseUrl} " +
                               "with no caching enabled")));
        }

        [Theory]
        [InlineData(null, "test")]
        [InlineData("", "test")]
        [InlineData(" ", "test")]
        [InlineData("   ", "test")]
        [InlineData("test", null)]
        [InlineData("test", "")]
        [InlineData("test", " ")]
        [InlineData("test", "   ")]
        public void GetMediaEntries_PlexIsDisabled_Logs(string authToken, string baseUrl)
        {
            // Arrange
            _testPlexOptionsWrapper.Value.AuthToken = authToken;
            _testPlexOptionsWrapper.Value.BaseUrl = baseUrl;

            // Act
            _ = new MediaReconciler(_mockLogger, _mockFileSystem, _mockPlexEntries, _testPlexOptionsWrapper);

            // Assert
            _mockLogger.Received(1)
                .Log(Arg.Any<LogLevel>(), Arg.Is<string>(s =>
                    s.Contains("Plex reconciliation is not configured; a BaseUrl and AuthToken must be supplied to enable it.")));
        }
    }
}
