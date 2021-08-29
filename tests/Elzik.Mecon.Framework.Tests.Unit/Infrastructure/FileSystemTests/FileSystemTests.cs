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
using Xunit;
using FileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.FileSystem;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests
{
    public partial class FileSystemTests
    {
        private readonly IFixture _fixture;

        private readonly IDirectory _mockDirectory;
        private readonly IFileSystem _mockFileSystem;
        private readonly List<IFileInfo> _testFileInfos;

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

        [Fact]
        public void GetMediaFileInfos_InvalidFolderDefinitionName_Throws()
        {
            // Arrange
            var invalidFolderDefinitionName = _fixture.Create<string>();
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(_fixture.Create<FileSystemOptions>());
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => 
                fileSystem.GetMediaFileInfos(invalidFolderDefinitionName));

            // Assert
            ex.Message.Should().Be($"Folder definition with name of {invalidFolderDefinitionName} is not found.");
        }

        [Fact]
        public void GetMediaFileInfos_NoFileExtensions_ReturnsAllPaths()
        {
            // Arrange
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(_fixture.Create<FileSystemOptions>());
            var testNoFileExtensionsDefinition = _fixture
                .Build<FolderDefinitionOption>()
                .With(option => option.Name, "NoFileExtensions")
                .With(option => option.SupportedFileExtensions, Array.Empty<string>())
                .Create();
            testFileSystemOptions.Value.FolderDefinitions.Add(
                testNoFileExtensionsDefinition);

            _mockDirectory.EnumerateFiles(
                Arg.Is(testNoFileExtensionsDefinition.FolderPath), 
                Arg.Is("*.*"), 
                Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFileInfos(testNoFileExtensionsDefinition.Name);

            // Assert
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Fact]
        public void GetMediaFileInfos_NoExistingFileExtensions_ReturnsNoPaths()
        {
            // Arrange
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(_fixture.Create<FileSystemOptions>());
            var testNoFileExtensionsDefinition = _fixture
                .Build<FolderDefinitionOption>()
                .With(option => option.Name, "NoFileExtensions")
                .With(option => option.SupportedFileExtensions, new []{"no1", "no2", "no3"})
                .Create();
            testFileSystemOptions.Value.FolderDefinitions.Add(
                testNoFileExtensionsDefinition);

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testNoFileExtensionsDefinition.FolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFileInfos(testNoFileExtensionsDefinition.Name);

            // Assert
            filePaths.Should().BeEmpty();
        }

        [Fact]
        public void GetMediaFileInfos_WithExistingFileExtensions_ReturnsExpectedPaths()
        {
            // Arrange
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(_fixture.Create<FileSystemOptions>());
            var testNoFileExtensionsDefinition = _fixture
                .Build<FolderDefinitionOption>()
                .With(option => option.Name, "NoFileExtensions")
                .With(option => option.SupportedFileExtensions, new[] { "ext1", "ext2" })
                .Create();
            testFileSystemOptions.Value.FolderDefinitions.Add(
                testNoFileExtensionsDefinition);

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testNoFileExtensionsDefinition.FolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFileInfos(testNoFileExtensionsDefinition.Name);

            // Assert
            _testFileInfos.RemoveAll(info => info.FullName == "/FileThree.ext3");
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Fact]
        public void GetMediaFileInfos_WithExistingFileExtensionsDifferingInCase_ReturnsExpectedPaths()
        {
            // Arrange
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(_fixture.Create<FileSystemOptions>());
            var testNoFileExtensionsDefinition = _fixture
                .Build<FolderDefinitionOption>()
                .With(option => option.Name, "NoFileExtensions")
                .With(option => option.SupportedFileExtensions, new[] { "EXT1", "EXT2" })
                .Create();
            testFileSystemOptions.Value.FolderDefinitions.Add(
                testNoFileExtensionsDefinition);

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testNoFileExtensionsDefinition.FolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFileInfos(testNoFileExtensionsDefinition.Name);

            // Assert
            _testFileInfos.RemoveAll(info => info.FullName == "/FileThree.ext3");
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }
    }
}
