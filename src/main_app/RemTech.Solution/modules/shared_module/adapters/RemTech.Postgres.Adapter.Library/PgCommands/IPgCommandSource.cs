using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public interface IPgCommandSource
{
    NpgsqlCommand Command();
}