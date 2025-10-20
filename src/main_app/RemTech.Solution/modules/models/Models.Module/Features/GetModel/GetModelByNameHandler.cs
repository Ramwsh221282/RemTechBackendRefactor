using System.Data.Common;
using Models.Module.Types;
using Npgsql;
using RemTech.Core.Shared.Cqrs;

namespace Models.Module.Features.GetModel;

internal sealed class GetModelByNameHandler(NpgsqlConnection connection)
    : ICommandHandler<GetModelCommand, IModel>
{
    private const string Sql =
        "SELECT id, name, rating FROM models_module.models WHERE name = @name;";
    private const string NameParam = "@name";
    private const string IdColumn = "id";
    private const string NameColumn = "name";
    private const string RatingColumn = "rating";

    public async Task<IModel> Handle(GetModelCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(command.Name))
            throw new GetModelByNameNameEmptyException();
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>(NameParam, command.Name));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new GetModelByNameNotFoundException(command.Name);
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal(IdColumn));
        string name = reader.GetString(reader.GetOrdinal(NameColumn));
        long rating = reader.GetInt64(reader.GetOrdinal(RatingColumn));
        return new Model(id, name, rating);
    }
}
