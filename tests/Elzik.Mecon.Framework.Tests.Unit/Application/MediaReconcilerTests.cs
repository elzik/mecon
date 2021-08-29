using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests;
using Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex.TestData;
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

        private readonly ILogger<MediaReconciler> _mockLogger;
        private readonly IFileSystem _mockFileSystem;
        private readonly IPlexEntries _mockPlexEntries;

        public MediaReconcilerTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _mockLogger = Substitute.For<ILogger<MediaReconciler>>();
            _mockFileSystem = Substitute.For<IFileSystem>();
            _mockPlexEntries = Substitute.For<IPlexEntries>();
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(MediaReconciler).GetConstructors());
        }

        [Fact]
        public async Task GetMediaEntries_AllFilesHaveMatchignPlexEntries_ReturnsExpectedMediaEntries()
        {
            // Arrange
            var testFolderDefinitionName = _fixture.Create<string>();
            var testFiles = _fixture.CreateMany<FileSystemTests.TestFileInfoImplementation>().ToList();
            _mockFileSystem.GetMediaFileInfos(Arg.Is(testFolderDefinitionName)).Returns(testFiles);

            var testPlexOptions = _fixture.Create<PlexWithCachingOptions>();
            var testPlexOptionsWrapper = new OptionsWrapper<PlexWithCachingOptions>(testPlexOptions);
            var testPlexEntries = _fixture.CreateMany<PlexEntry>().ToList();
            _mockPlexEntries.GetPlexEntries().Returns(testPlexEntries);

            Aligner.AlignFileSystemWithPlexMediaContainer(testFiles, testPlexEntries);

            // Act
            var mediaReconciler = new MediaReconciler(_mockLogger, _mockFileSystem, _mockPlexEntries, testPlexOptionsWrapper);
            var mediaEntries =  await mediaReconciler.GetMediaEntries(testFolderDefinitionName);

            // Assert
            var mediaEntryList = mediaEntries.ToList();
            mediaEntryList.Should().NotBeNull();
            mediaEntryList.Should().HaveSameCount(testFiles);
            mediaEntryList.Should().HaveSameCount(testPlexEntries);
            for (var index = 0; index < mediaEntryList.Count; index++)
            {
                var mediaEntry = mediaEntryList[index];
                var testFileInfo = testFiles[index];
                var testPlexEntry = testPlexEntries[index];

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
    }
}
