using AngleSharp.Html.Dom;

namespace DataGrabber.Parser.Core.ParsingStrategies;

public interface IParsingStrategy<T> where T : new()
{
    Task<T> ParseAsync(IHtmlDocument htmlDocument);
    T Parse(IHtmlDocument htmlDocument); 
}