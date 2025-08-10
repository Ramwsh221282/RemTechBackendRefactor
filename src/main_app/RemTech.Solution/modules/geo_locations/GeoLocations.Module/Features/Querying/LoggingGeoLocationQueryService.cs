namespace GeoLocations.Module.Features.Querying;

public sealed class LoggingGeoLocationQueryService(
    Serilog.ILogger logger,
    IGeoLocationQueryService service
) : IGeoLocationQueryService
{
    public async Task<GeoLocationInfo> VectorSearch(string text, CancellationToken ct = default)
    {
        try
        {
            logger.Information("Querying geo location. Text: {Text}.", text);
            GeoLocationInfo result = await service.VectorSearch(text, ct);
            logger.Information("Querying result: {Region} {City}", result.Region, result.City);
            return result;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "Fatal at: {Service}. Error: {Ex}.",
                nameof(IGeoLocationQueryService),
                ex.Message
            );
            throw;
        }
    }
}
