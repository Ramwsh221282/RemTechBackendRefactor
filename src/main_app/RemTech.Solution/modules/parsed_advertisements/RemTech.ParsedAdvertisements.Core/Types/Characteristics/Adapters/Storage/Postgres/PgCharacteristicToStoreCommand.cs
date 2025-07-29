using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics.Ports.Storage;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Types.Characteristics.Adapters.Storage.Postgres;

public sealed class PgCharacteristicToStoreCommand(Guid id, string text, string measuring)
    : IPgCharacteristicToStoreCommand
{
    private readonly string _sql = string.Intern(
        """
        INSERT INTO parsed_advertisements_module.vehicle_characteristics(id, text, measuring)
        VALUES (@id, @text, @measuring)
        ON CONFLICT(text) DO NOTHING;
        """
    );

    public async Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new OperationException("Нельзя добавить характеристику. Идентификатор пустой.");
        if (string.IsNullOrWhiteSpace(measuring))
            throw new OperationException(
                "Нельзя добавить характеристику. Единица измерения пустая."
            );
        if (string.IsNullOrWhiteSpace(text))
            throw new OperationException("Нельзя добавить характеристику. Название пустое.");
        return await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql))
                    .With("@id", id)
                    .With("@text", text)
                    .With("@measuring", measuring)
            )
        ).AsyncExecuted(ct);
    }
}
