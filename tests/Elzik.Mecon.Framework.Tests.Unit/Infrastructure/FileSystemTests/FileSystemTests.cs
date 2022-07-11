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
using IFileSystem = System.IO.Abstractions.IFileSystem;
using Elzik.Mecon.Framework.Domain.FileSystem;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests
{
    public class FileSystemTests
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
            var testDirectoryDefinition = _fixture
                .Build<DirectoryDefinition>()
                .Without(definition => definition.DirectoryFilterRegexPattern)
                .With(definition => definition.Recurse, testRecurse)
                .With(definition => definition.SupportedFileExtensions, 
                    Array.Empty<string>())
                .Create();

            _mockDirectory.EnumerateFiles(
                Arg.Is(testDirectoryDefinition.DirectoryPath), 
                Arg.Is("*.*"), 
                Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories == testRecurse))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testDirectoryDefinition);

            // Assert
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetMediaFileInfos_NullFileExtensions_ReturnsExpectedPaths(bool testRecurse)
        {
            // Arrange
            var testDirectoryDefinition = _fixture
                .Build<DirectoryDefinition>()
                .Without(definition => definition.DirectoryFilterRegexPattern)
                .With(definition => definition.Recurse, testRecurse)
                .Without(definition => definition.SupportedFileExtensions)
                .Create();

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testDirectoryDefinition.DirectoryPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories == testRecurse))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testDirectoryDefinition);

            // Assert
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Fact]
        public void GetMediaFileInfos_NoExistingFileExtensions_ReturnsNoPaths()
        {
            // Arrange
            var testDirectoryDefinition = _fixture
                .Build<DirectoryDefinition>()
                .With(definition => definition.SupportedFileExtensions, new[] { "no1", "no2", "no3" })
                .Without(definition => definition.DirectoryFilterRegexPattern)
                .Create();

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testDirectoryDefinition.DirectoryPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testDirectoryDefinition);

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
            var testDirectoryDefinition = _fixture
                .Build<DirectoryDefinition>()
                .With(definition => definition.SupportedFileExtensions, testExistingFileExtensions)
                .Without(definition => definition.DirectoryFilterRegexPattern)
                .Create();

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testDirectoryDefinition.DirectoryPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testDirectoryDefinition);

            // Assert
            _testFileInfos.RemoveAll(info => info.FullName == "/FileThree.ext3");
            filePaths.Should().BeEquivalentTo(_testFileInfos);
        }

        [Fact]
        public void GetDirectoryDefinition_NoDirectoryDefinitions_Throws()
        {
            // Arrange
            var testInvalidDirectoryDefinitionName = _fixture.Create<string>();

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var ex = Assert.Throws<InvalidOperationException>(() =>
                fileSystem.GetDirectoryDefinition(testInvalidDirectoryDefinitionName));

            // Assert
            ex.Message.Should().Be($"No directory definitions are configured; {testInvalidDirectoryDefinitionName} is not found.");
        }

        [Fact]
        public void GetDirectoryDefinition_EmptyDirectoryDefinitions_Throws()
        {
            // Arrange
            var testInvalidDirectoryDefinitionName = _fixture.Create<string>();
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(new FileSystemOptions()
            { DirectoryDefinitions = new Dictionary<string, DirectoryDefinition>() });

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);
            var ex = Assert.Throws<InvalidOperationException>(() =>
                fileSystem.GetDirectoryDefinition(testInvalidDirectoryDefinitionName));

            // Assert
            ex.Message.Should().Be($"No directory definitions are configured; {testInvalidDirectoryDefinitionName} is not found.");
        }

        [Fact]
        public void GetDirectoryDefinition_MissingDirectoryDefinitions_Throws()
        {
            // Arrange
            var testInvalidDirectoryDefinitionName = _fixture.Create<string>();
            var testEmptyFileSystemOptions = _fixture.Create<OptionsWrapper<FileSystemOptions>>();

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testEmptyFileSystemOptions);
            var ex = Assert.Throws<InvalidOperationException>(() =>
                fileSystem.GetDirectoryDefinition(testInvalidDirectoryDefinitionName));

            // Assert
            ex.Message.Should().Be($"Directory definition with name of {testInvalidDirectoryDefinitionName} is not found.");
        }

        [Fact]
        public void GetDirectoryDefinition_DirectoryDefinitionExists_ReturnsDirectoryDefinition()
        {
            // Arrange
            var testValidDirectoryDefinitionName = _fixture.Create<string>();
            var testValidDirectoryDefinition = _fixture.Create<DirectoryDefinition>();
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(new FileSystemOptions()
            {
                DirectoryDefinitions = new Dictionary<string, DirectoryDefinition>()
                    {{testValidDirectoryDefinitionName, testValidDirectoryDefinition}}
            });
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, testFileSystemOptions);

            // Act
            var directoryDefinition = fileSystem.GetDirectoryDefinition(testValidDirectoryDefinitionName);

            // Assert
            directoryDefinition.Should().Be(testValidDirectoryDefinition);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetMediaFileInfos_WithRegExFilter_ReturnsExpectedPaths(bool testRecurse)
        {
            // Arrange
            var testDirectoryDefinition = _fixture
                .Build<DirectoryDefinition>()
                .With(definition => definition.Recurse, testRecurse)
                .Without(definition => definition.SupportedFileExtensions)
                .With(definition => definition.DirectoryFilterRegexPattern, "^(?!.*FileTwo).*$")
                .Create();

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testDirectoryDefinition.DirectoryPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories == testRecurse))
                .Returns(_testFileInfos.Select(info => info.FullName));

            // Act
            var fileSystem = new FileSystem(_mockDirectory, _mockFileSystem, _testOptionsWrapper);
            var filePaths = fileSystem.GetMediaFileInfos(testDirectoryDefinition);

            // Assert
            filePaths.Should().BeEquivalentTo(_testFileInfos.Where(info => !info.Name.Contains("FileTwo")));
        }
    }
}
