using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wilkywayre.Iot.Service.Services.SmartThingsCloud.Configuration;

namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud.MessageHandler;

public class SmartThingsHandler : DelegatingHandler
{
    private readonly SmartThings _configuration;
    private readonly ILogger<SmartThingsHandler> _logger;
    private readonly string Token;

    public SmartThingsHandler(IOptions<SmartThings> configuration, ILogger<SmartThingsHandler> logger)
    {
        _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
        Token = _configuration.PersonalAccessToken;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",Token);
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
    
    private async Task<string> ReadStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var data =  await reader.ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin);
        return data;
    }
    
    
}