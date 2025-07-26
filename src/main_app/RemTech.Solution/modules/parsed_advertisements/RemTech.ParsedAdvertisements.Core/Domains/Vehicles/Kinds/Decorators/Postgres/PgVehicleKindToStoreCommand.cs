using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators.Postgres;

public sealed class PgVehicleKindToStoreCommand(string text, Guid id)
{
    private readonly string _sql = string.Intern("""
                                                INSERT INTO parsed_advertisements_module.vehicle_kinds(id, text)
                                                VALUES (@id, @text)
                                                ON CONFLICT(text) DO NOTHING;
                                                """);

    public async Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new OperationException("Невозможно добавить тип техники. Идентификатор пустой.");
        if (string.IsNullOrWhiteSpace(text))
            throw new OperationException("Невозможно добавить тип техники. Название пустое..");
        return await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                        new PgCommand(connection, _sql))
                    .With("@id", id)
                    .With("@text", text))).AsyncExecuted(ct);
    }
}