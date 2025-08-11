using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public interface IPgCommandSource
{
    NpgsqlCommand Command();
}
