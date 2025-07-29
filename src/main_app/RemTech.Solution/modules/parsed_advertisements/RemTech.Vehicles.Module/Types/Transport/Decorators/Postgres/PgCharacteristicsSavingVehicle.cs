using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators.Postgres;

public sealed class PgCharacteristicsSavingVehicle(Vehicle vehicle) : Vehicle(vehicle)
{
    public async Task<int> SaveAsync(NpgsqlConnection connection, CancellationToken ct = default)
    {
        VehicleCharacteristic[] ctxes = Characteristics.Read();
        string sql = FormSql(ctxes);
        ParametrizingPgCommand parametrized = FormParametrizedCommand(ctxes, connection, sql);
        int affected = await new AsyncExecutedCommand(
            new AsyncPreparedCommand(parametrized)
        ).AsyncExecuted(ct);
        return affected == 0
            ? throw new OperationException(
                "Не удается вставить характеристики техники в таблицу parsed vehicle characteristics"
            )
            : affected;
    }

    private string FormSql(VehicleCharacteristic[] ctxes)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.parsed_vehicle_characteristics
            (vehicle_id, ctx_id, ctx_name, ctx_value, ctx_measure)
            VALUES
            {0}
            """
        );
        List<string> insertions = [];
        for (int index = 0; index < ctxes.Length; index++)
        {
            string insertion =
                $"(@vehicle_id_{index}, @ctx_id_{index}, @ctx_name_{index}, @ctx_value_{index}, @ctx_measure_{index})";
            insertions.Add(insertion);
        }

        return string.Format(sql, string.Join(", ", insertions));
    }

    private ParametrizingPgCommand FormParametrizedCommand(
        VehicleCharacteristic[] ctxes,
        NpgsqlConnection connection,
        string sql
    )
    {
        ParametrizingPgCommand parametrized = new(new PgCommand(connection, sql));
        string vehicleId = Identity.Read();
        for (int index = 0; index < ctxes.Length; index++)
        {
            VehicleCharacteristic ctx = ctxes[index];
            parametrized = ctx.CtxPgCommand(index, parametrized)
                .With($"@vehicle_id_{index}", vehicleId);
        }

        return parametrized;
    }
}
