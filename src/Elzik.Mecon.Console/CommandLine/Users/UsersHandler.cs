using Elzik.Mecon.Framework.Domain.Plex;
using Nito.AsyncEx;

namespace Elzik.Mecon.Console.CommandLine.Users
{
    public class UsersHandler : IUsersHandler
    {
        private readonly IPlexUsers _users;

        public UsersHandler(IPlexUsers users)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public void DisplayUsers()
        {
            var plexUsers = AsyncContext.Run(() => _users.GetPlexUsers());

            System.Console.WriteLine(
                string.Join(Environment.NewLine, plexUsers.Select(user => user.UserTitle)));
        }
    }
}
