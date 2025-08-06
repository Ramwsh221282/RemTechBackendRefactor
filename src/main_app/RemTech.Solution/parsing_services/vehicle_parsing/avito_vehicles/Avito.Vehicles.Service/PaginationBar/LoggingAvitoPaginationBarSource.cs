namespace Avito.Vehicles.Service.PaginationBar;

public sealed class LoggingAvitoPaginationBarSource(
    Serilog.ILogger log,
    IAvitoPaginationBarSource origin
) : IAvitoPaginationBarSource
{
    public async Task<AvitoPaginationBarElement> Read()
    {
        log.Information("Reading avito pagination bar.");
        AvitoPaginationBarElement bar = await origin.Read();
        log.Information("Avito pagination pages amount: {0}.", bar.Amount());
        return bar;
    }
}
