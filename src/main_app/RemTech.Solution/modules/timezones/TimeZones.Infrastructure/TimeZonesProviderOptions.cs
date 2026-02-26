namespace TimeZones.Infrastructure;

public sealed class TimeZonesProviderOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new InvalidOperationException($"{nameof(TimeZonesProviderOptions)} требуется установить значение {nameof(ApiKey)} для использования.");
        }

    }
}

public sealed class TimeZonesProvider
{
    private readonly TimeZonesProviderOptions _options;

    public TimeZonesProvider(TimeZonesProviderOptions options)
    {
        _options = options;
    }

    private sealed class TimeZoneListResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public IReadOnlyList<TimeZoneInfo> Zones { get; set; } = [];
    }

    

    // http://api.timezonedb.com/v2.1/list-time-zone?key=N75LSH9ABRHP&format=json&fields=zoneName&country=RU

}

public sealed class TimeZoneInfo
{
    public required string ZoneName { get; init; }
}

public static class HttpRequestMessageConstructionModule
{
    extension(HttpRequestMessage)
    {
        public static HttpRequestMessage CreateGet(UriBuilder builder)
        {
            return new HttpRequestMessage(HttpMethod.Get, builder.Uri);
        }
    }
}

public sealed class HttpRequestQueryBuilder
{
    private readonly NameValueCollection _query = System.Web.HttpUtility.ParseQueryString(string.Empty);

    public HttpRequestQueryBuilder Add(string key, string value)
    {

        _query[key] = value;
        return this;
    }

    public UriBuilder ApplyTo(UriBuilder uriBuilder)
    {
        uriBuilder.Query = _query.ToString();
        return uriBuilder;
    }

}
