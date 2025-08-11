using System.Data.Common;
using Models.Module.Types;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Models.Module.Features.GetModel;

internal sealed class GetModelByNameHandler(NpgsqlConnection connection)
    : ICommandHandler<GetModelCommand, IModel>
{
    public async Task<IModel> Handle(GetModelCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(command.Name))
            throw new GetModelByNameNameEmptyException();
        string sql = string.Intern(
            "SELECT id, name, rating FROM models_module.models WHERE name = @name;"
        );
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@name", command.Name));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new GetModelByNameNotFoundException(command.Name);
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        long rating = reader.GetInt64(reader.GetOrdinal("rating"));
        return new Model(id, name, rating);
    }
}
