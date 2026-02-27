using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

public sealed class TimeZonesProvider : ITimeZonesProvider
{
    private readonly TimeZonesProviderOptions _options;

    public TimeZonesProvider(IOptions<TimeZonesProviderOptions> options)
    {
        _options = options.Value;
    }

    public async Task<Dictionary<string, TimeZoneInfo>> FetchTimeZones(CancellationToken ct = default)
    {
        string key = _options.ApiKey;
        const string requestUrl = "http://api.timezonedb.com/v2.1/list-time-zone";

        UriBuilder uriBuilder = new(requestUrl);
        new HttpRequestQueryBuilder()
            .Add("key", key)
            .Add("format", "json")
            .Add("fields", "zoneName")
            .Add("country", "ru")
            .ApplyTo(uriBuilder);

        using HttpClientHandler handler = new() { UseProxy = false };
        using HttpClient client = new(handler);
        HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri, ct);
        response.EnsureSuccessStatusCode();

        TimeZoneListResponse? responseResult = await response
            .Content
            .ReadFromJsonAsync<TimeZoneListResponse>(cancellationToken: ct);

        if (responseResult is null)
        {
            return [];
        }

        Dictionary<string, TimeZoneInfo> result = [];
        foreach (TimeZoneRecord record in responseResult.Zones)
        {
            string zoneName = record.ZoneName;
            result.Add(zoneName, TimeZoneInfo.FindSystemTimeZoneById(zoneName));
        }

        return result;
    }

    private sealed class TimeZoneListResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public IReadOnlyList<TimeZoneRecord> Zones { get; set; } = [];
    }
}
