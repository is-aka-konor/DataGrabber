namespace DataGrabber.Parser.Core;

public interface IHtmlLoader
{
    Task<string> LoadHtmlAsync(string source);
}