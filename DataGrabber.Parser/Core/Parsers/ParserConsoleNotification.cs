using Microsoft.Extensions.Logging;

namespace DataGrabber.Parser.Core.Parsers;

public class ParserConsoleNotification<T> : IParserNotification<T> where T : new()
{
    private readonly ILogger<ParserConsoleNotification<T>> _logger;
    
    public ParserConsoleNotification(ILogger<ParserConsoleNotification<T>> logger)
    {
        this._logger = logger;
    }
    
    public void Parser_OnNewData(object arg1, T arg2)
    {
        try
        {
            Console.WriteLine($"Something happened {arg2.ToString()}");
        }
        catch(Exception ex)
        {
            var errorMessaage = $"Could not save details for {nameof(T)}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessaage);
            Console.ResetColor();
            this._logger?.LogError(
                $"{errorMessaage}, exception: {Environment.NewLine}{ex.Message}" +
                $"{Environment.NewLine}{ex.StackTrace}");
        }
    }
    
    public void Parser_OnCompleted(object arg1)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Parser finished the work!");
        Console.ResetColor();
    }
}