namespace RemTech.Spares.Module.Features.SinkSpare.Persistance;

internal interface ISparePersistanceCommand<TPersistance>
{
    TPersistance Read();
}
