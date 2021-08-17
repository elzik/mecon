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
using Xunit;
using FileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.FileSystem;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests
{
    public class FileSystemTests
    {
        private readonly IFixture _fixture;

        private readonly IDirectory _mockDirectory;

        public FileSystemTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _mockDirectory = Substitute.For<IDirectory>();
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
                new FileSystem(_mockDirectory, testNullFileSystemOptionsValue));

            // Assert
            ex.Message.Should().Be("Value of fileSystemOptions must not be null.");
        }

        [Fact]
        public void GetMediaFilePaths_InvalidFolderDefinitionName_Throws()
        {
            // Arrange
            var invalidFolderDefinitionName = _fixture.Create<string>();
            var testFileSystemOptions = new OptionsWrapper<FileSystemOptions>(_fixture.Create<FileSystemOptions>());
            var fileSystem = new FileSystem(_mockDirectory, testFileSystemOptions);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => 
                fileSystem.GetMediaFilePaths(invalidFolderDefinitionName));

            // Assert
            ex.Message.Should().Be($"Folder definition with name of {invalidFolderDefinitionName} is not found.");
        }

        [Fact]
        public void GetMediaFilePaths_NoFileExtensions_ReturnsAllPaths()
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
            var testFilePaths = new List<string>()
            {
                "/FileOne.ext1",
                "/FileTwo.ext2",
                "/FileThree.ext3",
            };

            _mockDirectory.EnumerateFiles(
                Arg.Is(testNoFileExtensionsDefinition.FolderPath), 
                Arg.Is("*.*"), 
                Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(testFilePaths);

            var fileSystem = new FileSystem(_mockDirectory, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFilePaths(testNoFileExtensionsDefinition.Name);

            // Assert
            filePaths.Should().BeEquivalentTo(testFilePaths);
        }

        [Fact]
        public void GetMediaFilePaths_NoExistingFileExtensions_ReturnsNoPaths()
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
            var testFilePaths = new List<string>()
            {
                "/FileOne.ext1",
                "/FileTwo.ext2",
                "/FileThree.ext3",
            };

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testNoFileExtensionsDefinition.FolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(testFilePaths);

            var fileSystem = new FileSystem(_mockDirectory, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFilePaths(testNoFileExtensionsDefinition.Name);

            // Assert
            filePaths.Should().BeEmpty();
        }

        [Fact]
        public void GetMediaFilePaths_WithExistingFileExtensions_ReturnsExpectedPaths()
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
            var testFilePaths = new List<string>()
            {
                "/FileOne.ext1",
                "/FileTwo.ext2",
                "/FileThree.ext3",
            };

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testNoFileExtensionsDefinition.FolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(testFilePaths);

            var fileSystem = new FileSystem(_mockDirectory, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFilePaths(testNoFileExtensionsDefinition.Name);

            // Assert
            testFilePaths.Remove("/FileThree.ext3");
            filePaths.Should().BeEquivalentTo(testFilePaths);
        }

        [Fact]
        public void GetMediaFilePaths_WithExistingFileExtensionsDifferingInCase_ReturnsExpectedPaths()
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
            var testFilePaths = new List<string>()
            {
                "/FileOne.ext1",
                "/FileTwo.ext2",
                "/FileThree.ext3",
            };

            _mockDirectory.EnumerateFiles(
                    Arg.Is(testNoFileExtensionsDefinition.FolderPath),
                    Arg.Is("*.*"),
                    Arg.Is<EnumerationOptions>(options => options.RecurseSubdirectories))
                .Returns(testFilePaths);

            var fileSystem = new FileSystem(_mockDirectory, testFileSystemOptions);

            // Act
            var filePaths = fileSystem.GetMediaFilePaths(testNoFileExtensionsDefinition.Name);

            // Assert
            testFilePaths.Remove("/FileThree.ext3");
            filePaths.Should().BeEquivalentTo(testFilePaths);
        }
    }
}
