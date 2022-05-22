using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Elzik.Mecon.Console.Tests.Unit.CommandLine.Reconciliation
{
    public sealed class ReconciliationHandlerTests : IDisposable
    {
        private readonly IFixture _fixture;
        private readonly StringWriter _consoleWriter;
        private readonly StringWriter _errorWriter;

        public ReconciliationHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _consoleWriter = new StringWriter();
            System.Console.SetOut(_consoleWriter);
            _errorWriter = new StringWriter();
            System.Console.SetError(_errorWriter);
        }

        [Fact]
        public void Handle_WithDirectoryKeyWhereInPlex_WritesExpectedFilePaths()
        {
            // Arrange
            var testReconciliationOptionsInPlex = _fixture
                .Build<ReconciliationOptions>()
                .With(options => options.MissingFromLibrary, false)
                .With(options => options.PresentInLibrary, true)
                .Create();
            var testDirectoryDefinition = _fixture.Create<DirectoryDefinition>();
            var testMediaEntriesWithoutPlex = _fixture.CreateMany<MediaEntry>();
            var testMediaEntriesWithPlex = _fixture.CreateMany<MediaEntry>().ToList();
            foreach (var mediaEntry in testMediaEntriesWithPlex)
            {
                mediaEntry.ReconciledEntries.Add(_fixture.Create<PlexEntry>());
            }
            var testAllMediaEntries = testMediaEntriesWithoutPlex.Concat(testMediaEntriesWithPlex);

            var mockReconciledMedia = Substitute.For<IReconciledMedia>();
            mockReconciledMedia.GetMediaEntries(Arg.Is(testDirectoryDefinition)).Returns(testAllMediaEntries);
            var mockFileSystem = Substitute.For<IFileSystem>();
            mockFileSystem.GetDirectoryDefinition(Arg.Is(testReconciliationOptionsInPlex.DirectoryKey))
                .Returns(testDirectoryDefinition);
            var mockConfigurationBuilder = new ConfigurationBuilder();

            // Act
            var reconciliationHandler = new ReconciliationHandler(mockReconciledMedia, mockFileSystem);
            reconciliationHandler.Handle(mockConfigurationBuilder, testReconciliationOptionsInPlex);

            // Assert
            var expectedOutput = string.Join(Environment.NewLine,
                testMediaEntriesWithPlex.Select(entry => entry.FilesystemEntry.FileSystemPath)) + Environment.NewLine;
            _consoleWriter.ToString().Should()
                .Be(expectedOutput);
        }

        [Fact]
        public void Handle_WithDirectoryKeyWhereNotInPlex_WritesExpectedFilePaths()
        {
            // Arrange
            var testReconciliationOptionsNotInPlex = _fixture
                .Build<ReconciliationOptions>()
                .With(options => options.MissingFromLibrary, true)
                .With(options => options.PresentInLibrary, false)
                .Create();
            var testDirectoryDefinition = _fixture.Create<DirectoryDefinition>();
            var testMediaEntriesWithoutPlex = _fixture.CreateMany<MediaEntry>().ToList();
            var testMediaEntriesWithPlex = _fixture.CreateMany<MediaEntry>().ToList();
            foreach (var mediaEntry in testMediaEntriesWithPlex)
            {
                mediaEntry.ReconciledEntries.Add(_fixture.Create<PlexEntry>());
            }
            var testAllMediaEntries = testMediaEntriesWithoutPlex.Concat(testMediaEntriesWithPlex);

            var mockReconciledMedia = Substitute.For<IReconciledMedia>();
            mockReconciledMedia.GetMediaEntries(Arg.Is(testDirectoryDefinition)).Returns(testAllMediaEntries);
            var mockFileSystem = Substitute.For<IFileSystem>();
            mockFileSystem.GetDirectoryDefinition(Arg.Is(testReconciliationOptionsNotInPlex.DirectoryKey))
                .Returns(testDirectoryDefinition);
            var mockConfigurationBuilder = new ConfigurationBuilder();

            // Act
            var reconciliationHandler = new ReconciliationHandler(mockReconciledMedia, mockFileSystem);
            reconciliationHandler.Handle(mockConfigurationBuilder, testReconciliationOptionsNotInPlex);

            // Assert
            var expectedOutput = string.Join(Environment.NewLine,
                testMediaEntriesWithoutPlex.Select(entry => entry.FilesystemEntry.FileSystemPath)) + Environment.NewLine;
            _consoleWriter.ToString().Should()
                .Be(expectedOutput);
        }

        [Fact]
        public void Handle_Throws_ExitsAndWritesToErrorStream()
        {
            // Arrange
            var testReconciliationOptionsNotInPlex = _fixture.Create<ReconciliationOptions>();
            var testException = _fixture.Create<InvalidOperationException>();

            var mockReconciledMedia = Substitute.For<IReconciledMedia>();
            var mockFileSystem = Substitute.For<IFileSystem>();
            mockFileSystem.GetDirectoryDefinition(Arg.Is(testReconciliationOptionsNotInPlex.DirectoryKey))
                .Throws(testException);
            var mockConfigurationBuilder = new ConfigurationBuilder();

            // Act
            var reconciliationHandler = new ReconciliationHandler(mockReconciledMedia, mockFileSystem);
            reconciliationHandler.Handle(mockConfigurationBuilder, testReconciliationOptionsNotInPlex);

            // Assert
            Environment.ExitCode.Should().Be(1);
            _errorWriter.ToString().Should().Be($"Error: {testException.Message}{Environment.NewLine}");
        }

        public void Dispose()
        {
            _consoleWriter.Dispose();
        }
    }
}
