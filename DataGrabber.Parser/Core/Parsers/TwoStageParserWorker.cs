using System.Threading.Channels;
using AngleSharp.Html.Parser;
using DataGrabber.Parser.Core.ParsingStrategies;
using Microsoft.Extensions.Logging;

namespace DataGrabber.Parser.Core.Parsers;

public class TwoStageParserWorker<T> : IParserWorker where T : new()
{
    private readonly IParsingStrategy<T> _strategy;
    private readonly IInternetHtmlLoaderSettings _settings;
    private readonly ILogger<TwoStageParserWorker<T>> _logger;
    private readonly IHtmlLoader _loader;
    private readonly IParserNotification<T> _notifications;
    private ChannelWriter<T> _channelWriter;
    private bool _isActive;

    public TwoStageParserWorker(IParsingStrategy<T> strategy
        , IInternetHtmlLoaderSettings settings
        , IHtmlLoader loader
        , IParserNotification<T> notifications
        , ChannelWriter<T> channelWriter
        , ILogger<TwoStageParserWorker<T>> logger)
    {
        _strategy = strategy;
        _settings = settings;
        _logger = logger;
        _channelWriter = channelWriter;
        _loader = loader;
        _notifications = notifications;
    }
    
    /// <summary>
    /// Start the parser worker
    /// </summary>
    public async Task Start()
    {
        _isActive = true;
        await Work();
    }

    /// <summary>
    /// Stop the parser worker
    /// </summary>
    public void Abort()
    {
        _isActive = false;
    }

    /// <summary>
    /// The main method that does the work
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
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
                source = await _loader.LoadHtmlAsync(_settings.GetSourceUrl(i));
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
                        await _channelWriter.WriteAsync(result);
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
}