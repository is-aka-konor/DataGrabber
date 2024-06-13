// See https://aka.ms/new-console-template for more information

using System.Threading.Channels;
using DataGrabber.Parser.Core;
using DataGrabber.Parser.Core.Parsers;
using DataGrabber.Parser.Core.ParsingStrategies;
using DataGrabber.Parser.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
var loggerFactory = new LoggerFactory().AddSerilog(logger);

var settings = new InternetHtmlLoaderSettings()
{
    BaseURL = "https://a5e.tools/spells",
    StartPoint = 0,
    EndPoint = 8,
    Index = null,
    QueryParameters = "?combine=&field_spell_ritual_value=All&page="
};

var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddSerilog(logger))
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<IInternetHtmlLoaderSettings>(settings)
    .AddSingleton(Channel.CreateUnbounded<SpellModel>())
    .AddHttpClient()
    .AddScoped<IHtmlLoader, InternetHtmlLoader>()
    .AddScoped<IParserWorker, InternetParserWorker<List<string>>>()
    .AddScoped<IParsingStrategy<List<string>>, SpellListA5EStrategy>()
    .AddScoped<IParsingStrategy<SpellModel>, SpellA5EStrategy>()
    .AddTransient(typeof(IParserNotification<>), typeof(ParserConsoleNotification<>))
    .BuildServiceProvider();


var parserWorker = serviceProvider.GetRequiredService<IParserWorker>();
Console.WriteLine("Starting the parser worker");
await parserWorker.Start();
Console.WriteLine("Parser worker started");
