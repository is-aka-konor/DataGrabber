using AngleSharp.Html.Dom;
using DataGrabber.Parser.Models;

namespace DataGrabber.Parser.Core.ParsingStrategies;

public class SpellListA5EStrategy : IParsingStrategy<List<string>>
{
    public List<string> Parse(IHtmlDocument htmlDocument)
    {
        // Use CSS selectors to find the specific table column and extract href values
        var linkElements = htmlDocument.QuerySelectorAll("td.views-field-title a");

        return linkElements
            .Select(element => element.GetAttribute("href"))
            .ToList();
    }

    public Task<List<string>> ParseAsync(IHtmlDocument htmlDocument)
    {
        return Task.FromResult(Parse(htmlDocument));
    }
}