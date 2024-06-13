using System.Diagnostics.CodeAnalysis;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DataGrabber.Parser.Core.ParsingStrategies;
using Microsoft.Extensions.Logging;

namespace DataGrabber.Parser.Core.Parsers;

public class InternetParserWorker<T> : IParserWorker where T : new()
{
    private readonly IParsingStrategy<T> _strategy;
    private readonly IInternetHtmlLoaderSettings _settings;
    private readonly ILogger<InternetParserWorker<T>> _logger;
    private readonly IHtmlLoader _loader;
    private readonly IParserNotification<T> _notifications;
    private bool _isActive;
    private readonly bool _useIndex;

    [SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
    public InternetParserWorker(IParsingStrategy<T> strategy
        , IInternetHtmlLoaderSettings settings
        , IHtmlLoader loader
        , IParserNotification<T> notifications
        , ILogger<InternetParserWorker<T>> logger)
    {
        _strategy = strategy;
        _settings = settings;
        _logger = logger;
        _loader = loader;
        _notifications = notifications;


        this._useIndex = settings.Index != null;
    }
    
    public async Task Start()
    {
        _isActive = true;
        await Work();
    }

    public void Abort()
    {
        _isActive = false;
    }

    public async Task Work()
    {
        for (int i = _settings.StartPoint; i <= _settings.EndPoint; i++){
            if (!_isActive){
                _notifications.Parser_OnCompleted(this);
                return;
            }
            var pageLoaded = false;
            string source = string.Empty;
            // Try load the page till it's loaded
            while(!pageLoaded){
                source = await _loader.LoadHtmlAsync(GetSourceUrl(i));
                if (string.IsNullOrEmpty(source)){
                    Random rnd = new Random();
                    Thread.Sleep(1000 * (rnd.Next(5, 15)));
                }else{
                    pageLoaded = true;
                }
            }

            if (!string.IsNullOrEmpty(source)){
                var domParser = new HtmlParser();
                try{
                    var document = await domParser.ParseDocumentAsync(source);
                    if(document != null){
                        var result = await _strategy.ParseAsync(document);
                        _notifications.Parser_OnNewData(this, result);
                    }else{
                        var errorMessage = $"Could not parse page {i}";
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine(errorMessage);
                        Console.ResetColor();
                        _logger.LogDebug(errorMessage);
                    }

                }catch(Exception ex){
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    _logger.LogError("Exception message: {message}. Exception: {ex}",ex.Message, ex);
                }					
            }
        }

        _notifications.Parser_OnCompleted(this);
        _isActive = false;
    }
    
    private List<string?> ExtractSpellLinks(IHtmlDocument document){
        // Use CSS selectors to find the specific table column and extract href values
        var linkElements = document.QuerySelectorAll("td.views-field-title a");

        return linkElements
            .Select(element => element.GetAttribute("href"))
            .ToList();
    }
    
    private string GetSourceUrl(int index){
        var currentIndex = this._useIndex ? _settings.Index[index] : index.ToString();
        var url = $"{_settings.BaseURL}/{_settings.QueryParameters}" + @"{CurrentIndex}";
        var currentUrl = url.Replace("{CurrentIndex}", currentIndex);
        return currentUrl;
    }
}