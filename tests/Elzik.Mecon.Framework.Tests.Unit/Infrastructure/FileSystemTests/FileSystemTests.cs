using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Infrastructure.FileSystem.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Elzik.Mecon.Framework.Infrastructure.FileSystem;
using Xunit;
using FileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.FileSystem;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests
{
    public partial class FileSystemTests
    {
        private readonly IFixture _fixture;

        private readonly IDirectory _mockDirectory;
        private readonly IFileSystem _mockFileSystem;
        private readonly List<IFileInfo> _testFileInfos;
        private readonly OptionsWrapper<FileSystemOptions> _testOptionsWrapper;

        public FileSystemTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _mockDirectory = Substitute.For<IDirectory>();
            _mockFileSystem = Substitute.For<IFileSystem>();
            _testFileInfos = new List<IFileInfo>()
            {
                new TestFileInfoImplementation("/FileOne.ext1", "FileOne.ext1", _fixture.Create<int>()),
                new TestFileInfoImplementation("/FileTwo.ext2", "FileTwo.ext2", _fixture.Create<int>()),
                new TestFileInfoImplementation("/FileThree.ext3", "FileThree.ext3", _fixture.Create<int>())
            };
            foreach (var testFileInfo in _testFileInfos)
            {
                _mockFileSystem.FileInfo.FromFileName(Arg.Is(testFileInfo.FullName)).Returns(testFileInfo);
            }

            _testOptionsWrapper = new OptionsWrapper<FileSystemOptions>(new FileSystemOptions());
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(FileSystem).GetConstructors());
        }

        [Fact]
        public void Constructor_NullFileSystemOptionsValue_Throws()
        {
            // Arrange
            var testNullFileSystemOptionsValue = new OptionsWrapper<FileSystemOptions>(null);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => 
                new FileSystem(_mockDirectory, _mockFileSystem, testNullFileSystemOptionsValue));

            // Assert
            ex.Message.Should().Be("Value of fileSystemOptions must not be null.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetMediaFileInfos_NoFileExtensions_ReturnsExpectedPaths(bool testRecurse)
        {
            // Arrange
            var testNoFileExtensions = Array.Empty<string>();
            var testFolderPath = _fixture.Create<string>();

            _mockDirectory.EnumerateFiles(
                Arg.Is(testFolderPath), 
                Arg.Is("*.*"), 
                Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories == testRecurse))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testFolderPath, testNoFileExtensions, testRecurse);

            // Assert
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetMediaFileInfos_NullFileExtensions_ReturnsExpectedPaths(bool testRecurse)
        {
            // Arrange
            var testFolderPath = _fixture.Create<string>();

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testFolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories == testRecurse))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testFolderPath, null, testRecurse);

            // Assert
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Fact]
        public void GetMediaFileInfos_NoExistingFileExtensions_ReturnsNoPaths()
        {
            // Arrange
            var testNonExistingFileExtensions = new[] {"no1", "no2", "no3"};
            var testFolderPath = _fixture.Create<string>();

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testFolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testFolderPath, testNonExistingFileExtensions, true);

            // Assert
            filePaths.Should().BeEmpty();
        }

        [Theory]
        [InlineData("ext1", "ext2")]
        [InlineData("EXT1", "EXT2")]
        public void GetMediaFileInfos_WithExistingFileExtensions_ReturnsExpectedPaths(
            params string[] testExistingFileExtensions)
        {
            // Arrange
            var testFolderPath = _fixture.Create<string>();

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testFolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testFolderPath, testExistingFileExtensions, true);

            // Assert
            _testFileInfos.RemoveAll(info => info.FullName == "/FileThree.ext3");
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Fact]
        public void GetFolderDefinition_NoFolderDefinitions_Throws()
        {
            // Arrange
            var testInvalidFolderDefinitionName = _fixture.Create<string>();

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var ex = Assert.Throws<InvalidOperationException>(() =>
                fileSystem.GetFolderDefinition(testInvalidFolderDefinitionName));

            // Assert
            ex.Message.Should().Be($"No folder definitions are configured; {testInvalidFolderDefinitionName} is not found.");
        }

        [Fact]
        public void GetFolderDefinition_EmptyFolderDefinitions_Throws()
        {
            // Arrange
            var testInvalidFolderDefinitionName = _fixture.Create<string>();
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(new FileSystemOptions()
            { FolderDefinitions = new Dictionary<string, FolderDefinition>() });

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);
            var ex = Assert.Throws<InvalidOperationException>(() =>
                fileSystem.GetFolderDefinition(testInvalidFolderDefinitionName));

            // Assert
            ex.Message.Should().Be($"No folder definitions are configured; {testInvalidFolderDefinitionName} is not found.");
        }

        [Fact]
        public void GetFolderDefinition_MissingFolderDefinitions_Throws()
        {
            // Arrange
            var testInvalidFolderDefinitionName = _fixture.Create<string>();
            var testEmptyFileSystemOptions = _fixture.Create<OptionsWrapper<FileSystemOptions>>();

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testEmptyFileSystemOptions);
            var ex = Assert.Throws<InvalidOperationException>(() =>
                fileSystem.GetFolderDefinition(testInvalidFolderDefinitionName));

            // Assert
            ex.Message.Should().Be($"Folder definition with name of {testInvalidFolderDefinitionName} is not found.");
        }

        [Fact]
        public void GetFolderDefinition_FolderDefinitionExists_ReturnsFolderDefinition()
        {
            // Arrange
            var testValidFolderDefinitionName = _fixture.Create<string>();
            var testValidFolderDefinition = _fixture.Create<FolderDefinition>();
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(new FileSystemOptions()
            {
                FolderDefinitions = new Dictionary<string, FolderDefinition>()
                    {{testValidFolderDefinitionName, testValidFolderDefinition}}
            });
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);

            // Act
            var folderDefinition = fileSystem.GetFolderDefinition(testValidFolderDefinitionName);

            // Assert
            folderDefinition.Should().Be(testValidFolderDefinition);
        }
    }
}
