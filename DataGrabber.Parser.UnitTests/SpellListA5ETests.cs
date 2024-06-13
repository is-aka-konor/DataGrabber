using AngleSharp;
using AngleSharp.Html.Parser;
using DataGrabber.Parser.Core.ParsingStrategies;

namespace DataGrabber.Parser.UnitTests;

public class SpellListA5EStrategyTests
{
    private readonly string _htmlContent;

    public SpellListA5EStrategyTests()
    {
        // Read the HTML file content
        _htmlContent = File.ReadAllText("HtmlPageSources/SpellListA5E.html");
    }
    
    [Fact]
    public async Task SpellListA5EStrategy_ShouldExtract50Elements()
    {
        // Arrange
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var parser = context.GetService<IHtmlParser>();
        var document = await parser.ParseDocumentAsync(_htmlContent);
        var strategy = new SpellListA5EStrategy();

        // Act
        var links = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(50, links.Count);
    }
}