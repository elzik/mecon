using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Elzik.Mecon.Console.Tests.Unit.CommandLine.Reconciliation
{
    [Collection("RedirectedConsoleOutput")]
    public sealed class ReconciliationHandlerTests : IDisposable
    {
        private readonly IFixture _fixture;
        private readonly StringWriter _consoleWriter;
        private readonly StringWriter _errorWriter;
        private readonly IOutputOperations _mockOutputOperations;
        private readonly ReconciliationOptions _testReconciliationOptionsWithDirectoryDefinition;
        private readonly ReconciliationOptions _testReconciliationOptionsWithoutDirectoryDefinition;
        private readonly MediaEntryCollection _testInputMediaEntries;
        private readonly MediaEntryCollection _testOutputMediaEntries;
        private readonly IFileSystem _mockFileSystem;
        private readonly ConfigurationBuilder _mockConfigurationBuilder;
        private readonly ReconciliationHandler _reconciliationHandler;

        public ReconciliationHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _consoleWriter = new StringWriter();
            System.Console.SetOut(_consoleWriter);
            _errorWriter = new StringWriter();
            System.Console.SetError(_errorWriter);

            _testReconciliationOptionsWithDirectoryDefinition = _fixture.Create<ReconciliationOptions>();
            _testReconciliationOptionsWithoutDirectoryDefinition = _fixture.Build<ReconciliationOptions>().Without(options => options.DirectoryKey).Create();
            var testDirectoryDefinition = _fixture.Create<DirectoryDefinition>();
            _testInputMediaEntries = new MediaEntryCollection(_fixture.CreateMany<MediaEntry>());
            _testOutputMediaEntries = new MediaEntryCollection(_fixture.CreateMany<MediaEntry>());
            _mockOutputOperations = Substitute.For<IOutputOperations>();

            _mockOutputOperations.PerformOutputFilters(Arg.Is(_testInputMediaEntries), Arg.Is(_testReconciliationOptionsWithDirectoryDefinition))
                .Returns(_testOutputMediaEntries);
            _mockOutputOperations.PerformOutputFilters(Arg.Is(_testInputMediaEntries), Arg.Is(_testReconciliationOptionsWithoutDirectoryDefinition))
                .Returns(_testOutputMediaEntries);

            var mockReconciledMedia = Substitute.For<IReconciledMedia>();

            mockReconciledMedia.GetMediaEntries(Arg.Is(testDirectoryDefinition)).Returns(_testInputMediaEntries);
            mockReconciledMedia.GetMediaEntries(Arg.Is<DirectoryDefinition>(definition =>
                    definition.Recurse == _testReconciliationOptionsWithoutDirectoryDefinition.Recurse &&
                    definition.DirectoryFilterRegexPattern == _testReconciliationOptionsWithoutDirectoryDefinition.MatchRegex &&
                    definition.DirectoryPath == _testReconciliationOptionsWithoutDirectoryDefinition.DirectoryPath &&
                    definition.MediaTypes.Count() == _testReconciliationOptionsWithoutDirectoryDefinition.MediaTypes!.Count() &&
                    definition.SupportedFileExtensions.Length == _testReconciliationOptionsWithoutDirectoryDefinition.FileExtensions!.Count()))
                .Returns(_testInputMediaEntries);

            _mockFileSystem = Substitute.For<IFileSystem>();
            _mockFileSystem.GetDirectoryDefinition(Arg.Is(_testReconciliationOptionsWithDirectoryDefinition.DirectoryKey))
                .Returns(testDirectoryDefinition);
            _mockConfigurationBuilder = new ConfigurationBuilder();
            _reconciliationHandler = new ReconciliationHandler(mockReconciledMedia, _mockFileSystem, _mockOutputOperations);
        }

        [Fact]
        public void Handle_WithDirectoryKey_WritesExpectedFilePaths()
        {
            // Act
            _reconciliationHandler.Handle(_mockConfigurationBuilder, _testReconciliationOptionsWithDirectoryDefinition);

            // Assert
            var expectedOutput = string.Join(Environment.NewLine,
                _testOutputMediaEntries.Select(entry => entry.FilesystemEntry.FileSystemPath)) + Environment.NewLine;
            _consoleWriter.ToString().Should()
                .Be(expectedOutput);
        }

        [Fact]
        public void Handle_WithDirectoryKey_FiltersOutput()
        {
            // Act
            _reconciliationHandler.Handle(_mockConfigurationBuilder, _testReconciliationOptionsWithDirectoryDefinition);

            // Assert
            _mockOutputOperations.Received(1)
                .PerformOutputFilters(Arg.Is(_testInputMediaEntries), Arg.Is(_testReconciliationOptionsWithDirectoryDefinition));
        }

        [Fact]
        public void Handle_WithoutDirectoryKey_WritesExpectedFilePaths()
        {
            // Act
            _reconciliationHandler.Handle(_mockConfigurationBuilder, _testReconciliationOptionsWithoutDirectoryDefinition);

            // Assert
            var expectedOutput = string.Join(Environment.NewLine,
                _testOutputMediaEntries.Select(entry => entry.FilesystemEntry.FileSystemPath)) + Environment.NewLine;
            _consoleWriter.ToString().Should()
                .Be(expectedOutput);
        }

        [Fact]
        public void Handle_WithoutDirectoryKey_FiltersOutput()
        {
            // Arrange
            _testReconciliationOptionsWithDirectoryDefinition.DirectoryKey = null;

            // Act
            _reconciliationHandler.Handle(_mockConfigurationBuilder, _testReconciliationOptionsWithoutDirectoryDefinition);

            // Assert
            _mockOutputOperations.Received(1)
                .PerformOutputFilters(Arg.Is(_testInputMediaEntries), Arg.Is(_testReconciliationOptionsWithoutDirectoryDefinition));
        }

        public void Dispose()
        {
            _consoleWriter.Dispose();
            _errorWriter.Dispose();
        }
    }
}
