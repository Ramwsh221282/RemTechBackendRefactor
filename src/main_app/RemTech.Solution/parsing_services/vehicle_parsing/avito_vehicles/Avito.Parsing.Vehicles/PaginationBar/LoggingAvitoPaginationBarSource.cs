using Parsing.SDK.Logging;

namespace Avito.Parsing.Vehicles.PaginationBar;

public sealed class LoggingAvitoPaginationBarSource(
    IParsingLog log,
    IAvitoPaginationBarSource origin) 
    : IAvitoPaginationBarSource
{
    public async  Task<AvitoPaginationBarElement> Read()
    {
        log.Info("Reading avito pagination bar.");
        AvitoPaginationBarElement bar = await origin.Read();
        log.Info("Avito pagination pages amount: {0}.", bar.Amount());
        return bar;
    }
}