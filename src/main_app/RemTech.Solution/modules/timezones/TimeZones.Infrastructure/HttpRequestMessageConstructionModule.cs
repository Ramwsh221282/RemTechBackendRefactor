namespace TimeZones.Infrastructure;

internal static class HttpRequestMessageConstructionModule
{
    extension(HttpRequestMessage)
    {
        public static HttpRequestMessage CreateGet(UriBuilder builder)
        {
            return new HttpRequestMessage(HttpMethod.Get, builder.Uri);
        }
    }
}
