using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

internal sealed class TimeZonesProvider : ITimeZonesProvider
{    
    public TimeZonesProvider(IOptions<TimeZonesProviderOptions> options)
    {        
        _options = options;
    }

    private readonly IOptions<TimeZonesProviderOptions> _options;
    private TimeZonesProviderOptions Options => _options.Value;    
    private const int RequestDelaySeconds = 3;

    public async Task<Dictionary<string, TimeZoneRecord>> FetchTimeZones(CancellationToken ct = default)
    {        
        string key = Options.ApiKey;        
        using HttpClientHandler handler = new() { UseProxy = false };        
        using HttpClient client = new(handler);
        using HttpResponseMessage response = await InvokeRequest(client, key, ct);                
        return await ParseResponseMessage(response, ct);                        
    }    

    private static async Task<Dictionary<string, TimeZoneRecord>> ParseResponseMessage(HttpResponseMessage response, CancellationToken ct)
    {
        TimeZoneListResponse? responseResult = await response
            .Content
            .ReadFromJsonAsync<TimeZoneListResponse>(cancellationToken: ct);
        
        if (responseResult is null)
        {
            return [];
        }

        Dictionary<string, TimeZoneRecord> result = [];
        foreach (TimeZoneRecord record in responseResult.Zones)
        {
            string zoneName = record.ZoneName;
            result.Add(zoneName, record);
        }

        return result;
    }

    private static async Task<HttpResponseMessage> InvokeRequest(HttpClient client, string key, CancellationToken ct)
    {
        const string requestUrl = "http://api.timezonedb.com/v2.1/list-time-zone";
        UriBuilder uriBuilder = new(requestUrl);
        new HttpRequestQueryBuilder()
            .Add("key", key)
            .Add("format", "json")
            .Add("fields", "zoneName,gmtOffset,timestamp")
            .Add("country", "RU")
            .ApplyTo(uriBuilder);

        HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri, ct);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new InvalidOperationException("TimeZoneDb вернул не 200 статус при запросе чтения часовых поясов.");
        }
        
        await Task.Delay(TimeSpan.FromSeconds(RequestDelaySeconds), ct);
        return response;
    }

    private sealed class TimeZoneListResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public IReadOnlyList<TimeZoneRecord> Zones { get; set; } = [];
    }
}
