using System.Collections.Specialized;

namespace TimeZones.Infrastructure;

internal sealed class HttpRequestQueryBuilder
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
