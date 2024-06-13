using System.Text;
using Microsoft.Extensions.Logging;

namespace DataGrabber.Parser.Core;

public class InternetHtmlLoader : IHtmlLoader
{
    private readonly HttpClient _client;
    private readonly ILogger<InternetHtmlLoader> _logger;
    public InternetHtmlLoader(IHttpClientFactory clientFactory, ILogger<InternetHtmlLoader> logger)
    {
        _logger = logger;
        _client = clientFactory.CreateClient();
    }
    
    public InternetHtmlLoader(IHttpClientFactory clientFactory, string clientName, ILogger<InternetHtmlLoader> logger)
    {
        _logger = logger;
        _client = clientFactory.CreateClient(clientName);
    }
    
    public async Task<string> LoadHtmlAsync(string source)
    {
        var result = string.Empty;
        try
        {
            using (HttpResponseMessage response = await _client.GetAsync(source))
            using (HttpContent content = response.Content)
            {
                if (response?.StatusCode == System.Net.HttpStatusCode.OK
                    || response?.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // ... Read the string.
                    var bytes = await content.ReadAsByteArrayAsync();
                    result = Encoding.UTF8.GetString(bytes, 0, bytes.Length - 1);
                }
            }
        }catch(Exception ex){
            _logger.LogError("Could not process {url} because of the exception {exception} ",source, ex.Message);
        }

        return result;
    }
}