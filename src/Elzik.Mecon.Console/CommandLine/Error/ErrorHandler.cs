using CommandLine;
using CommandLine.Text;

namespace Elzik.Mecon.Console.CommandLine.Error
{
    public static class ErrorHandler
    {
        public static void Display(ParserResult<object> parserResult, IEnumerable<global::CommandLine.Error> errors)
        {
            var helpText = HelpText.AutoBuild(parserResult, h =>
            {
                h.Copyright = "mecon is licensed under the GNU General Public License v3.0";

                return HelpText.DefaultParsingErrorsHandler(parserResult, h);
            });

            if (errors.All(error => error.Tag
                    is ErrorType.HelpRequestedError or ErrorType.HelpVerbRequestedError))
            {
                System.Console.WriteLine(helpText);
            }
            else
            {
                System.Console.Error.WriteLine(helpText);
            }
        }
    }
}
