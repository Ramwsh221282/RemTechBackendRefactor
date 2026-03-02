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
        return await TimeZoneListResponse.FromMessage(response, ct);
    }

    // TODO: test this method.
    public async Task<TimeZoneRecord?> FetchTimeZone(string zoneName, CancellationToken ct = default)
    {
        string key = Options.ApiKey;
        const string baseUrl = "http://api.timezonedb.com/v2.1/get-time-zone";
        UriBuilder uriBuilder = new(baseUrl);
        new HttpRequestQueryBuilder()
            .Add("key", key)
            .Add("by", "zone")
            .Add("zone", zoneName)
            .Add("country", "RU")
            .Add("format", "json")
            .ApplyTo(uriBuilder);

        using HttpClientHandler handler = new() { UseProxy = false };
        using HttpClient client = new(handler);
        HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri, ct);
        await Task.Delay(TimeSpan.FromSeconds(RequestDelaySeconds), ct);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        TimeZoneByNameResponse? result = await response.Content.ReadFromJsonAsync<TimeZoneByNameResponse>(cancellationToken: ct);
        return result?.ToTimeZoneRecord();
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

        public static async Task<Dictionary<string, TimeZoneRecord>> FromMessage(
            HttpResponseMessage message,
            CancellationToken ct = default)
        {
            TimeZoneListResponse? responseResult = await message
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
    }

    private sealed class TimeZoneByNameResponse
    {
        public required string zoneName { get; set; }
        public required ulong timestamp { get; set; }
        public required ulong gmtOffset { get; set; }

        public TimeZoneRecord ToTimeZoneRecord()
        {
            return new TimeZoneRecord()
            {
                GmtOffset = gmtOffset,
                Timestamp = timestamp,
                ZoneName = zoneName
            };
        }
    }
}
