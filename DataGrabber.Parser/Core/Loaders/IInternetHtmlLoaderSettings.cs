namespace DataGrabber.Parser.Core;

public interface IInternetHtmlLoaderSettings
{
    string BaseURL { get; set; }

    string QueryParameters { get; set; }

    int StartPoint { get; set; }

    int EndPoint { get; set; }

    string[]? Index { get; set; }
    string GetSourceUrl(int index);
}

public class InternetHtmlLoaderSettings : IInternetHtmlLoaderSettings
{
    public string BaseURL { get; set; }
    public string QueryParameters { get; set; }
    public int StartPoint { get; set; }
    public int EndPoint { get; set; }
    public string[]? Index { get; set; }
    
    private readonly bool _useIndex;

    public InternetHtmlLoaderSettings()
    {
        this._useIndex = this.Index != null;
    }
    
    public string GetSourceUrl(int index){
        var currentIndex = this._useIndex ? Index[index] : index.ToString();
        var url = $"{BaseURL}/{QueryParameters}" + @"{CurrentIndex}";
        var currentUrl = url.Replace("{CurrentIndex}", currentIndex);
        return currentUrl;
    }
}