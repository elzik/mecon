using CommandLine;

namespace Elzik.Mecon.Console.CommandLine.Users
{
    [Verb("users", HelpText = "Perform actions based for Plex users.")]
    public class UsersOptions
    {
        [Option('l', "list", Required = true,
            HelpText = "List all users for the Plex server accessible via the configured token. " +
                       "This includes all friends, home users and the home administrator themselves.")]
        public bool ListUsers { get; set; }
    }
}
