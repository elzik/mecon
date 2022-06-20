using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Elzik.Mecon.Console.CommandLine.Users;
using Elzik.Mecon.Framework.Domain.Plex;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Elzik.Mecon.Console.Tests.Unit.CommandLine.Users
{
    public sealed class UsersHandlerTests : IDisposable
    {
        private readonly IFixture _fixture;
        private readonly StringWriter _consoleWriter;

        public UsersHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _consoleWriter = new StringWriter();
            System.Console.SetOut(_consoleWriter);
        }

        [Fact]
        public void DisplayUsers_UsersReturned_DisplaysUsers()
        {
            // Arrange
            var mockPlexUsers = Substitute.For<IPlexUsers>();
            var testUsers = _fixture.CreateMany<PlexUser>().ToList();
            mockPlexUsers.GetPlexUsers().Returns(testUsers);

            // Act
            var userHandler = new UsersHandler(mockPlexUsers);
            userHandler.DisplayUsers();


            // Assert
            var expectedOutput = string.Join(Environment.NewLine, testUsers.Select(user => user.UserTitle)) + Environment.NewLine;
            _consoleWriter.ToString().Should().Be(expectedOutput);
        }

        public void Dispose()
        {
            _consoleWriter.Dispose();
        }
    }
}
