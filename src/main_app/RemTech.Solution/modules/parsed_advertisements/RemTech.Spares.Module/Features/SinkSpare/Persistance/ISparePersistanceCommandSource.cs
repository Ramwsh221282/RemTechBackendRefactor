namespace RemTech.Spares.Module.Features.SinkSpare.Persistance;

internal interface ISparePersistanceCommandSource<TPersistance, TPersistanceCommand>
    where TPersistanceCommand : ISparePersistanceCommand<TPersistance>
{
    TPersistanceCommand Modify(TPersistanceCommand persistance);
}
