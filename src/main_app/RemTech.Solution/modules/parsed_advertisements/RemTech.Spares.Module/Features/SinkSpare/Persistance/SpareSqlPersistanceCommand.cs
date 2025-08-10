using Npgsql;

namespace RemTech.Spares.Module.Features.SinkSpare.Persistance;

internal sealed class SpareSqlPersistanceCommand(NpgsqlCommand command)
    : ISparePersistanceCommand<NpgsqlCommand>
{
    private readonly NpgsqlCommand _command = command;

    public NpgsqlCommand Read()
    {
        return _command;
    }
}
