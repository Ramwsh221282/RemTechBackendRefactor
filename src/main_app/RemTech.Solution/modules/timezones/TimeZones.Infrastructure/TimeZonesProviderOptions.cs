namespace TimeZones.Infrastructure;

public sealed class TimeZonesProviderOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            string message = $"{nameof(TimeZonesProviderOptions)} требуется установить значение {nameof(ApiKey)} для использования.";
            throw new InvalidOperationException(message);
        }
    }
}
