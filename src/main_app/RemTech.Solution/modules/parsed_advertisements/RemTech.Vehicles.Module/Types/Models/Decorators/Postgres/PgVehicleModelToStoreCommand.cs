﻿using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Types.Models.Decorators.Postgres;

public sealed class PgVehicleModelToStoreCommand(Guid id, string name)
{
    private readonly string _sql = string.Intern(
        """
        INSERT INTO parsed_advertisements_module.vehicle_models(id, text)
        VALUES (@id, @text)
        ON CONFLICT(text) DO NOTHING;
        """
    );

    public async Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new OperationException(
                "Невозможно добавить модель техники. Идентификатор пустой."
            );
        if (string.IsNullOrWhiteSpace(name))
            throw new OperationException("Невозможно добавить модель техники. Название пустое.");
        return await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql))
                    .With("@id", id)
                    .With("@text", name)
            )
        ).AsyncExecuted(ct);
    }
}
