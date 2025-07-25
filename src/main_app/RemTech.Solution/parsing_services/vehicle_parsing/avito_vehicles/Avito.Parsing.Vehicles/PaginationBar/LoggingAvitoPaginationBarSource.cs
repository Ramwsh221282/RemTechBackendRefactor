using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.PaginationBar;

public sealed class LoggingAvitoPaginationBarSource(
    ICustomLogger log,
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