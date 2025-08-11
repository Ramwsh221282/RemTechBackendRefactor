using System.Data.Common;
using Categories.Module.Types;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Categories.Module.Features.GetCategory;

internal sealed class GetCategoryByNameHandler(NpgsqlConnection connection)
    : ICommandHandler<GetCategoryCommand, ICategory>
{
    public async Task<ICategory> Handle(GetCategoryCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new GetCategoryByNameNameEmptyException();
        string sql = string.Intern(
            "SELECT id, name, rating FROM categories_module.categories WHERE name = @name;"
        );
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@name", command.Name));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new GetCategoryByNameNotFoundException(command.Name);
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        long rating = reader.GetInt64(reader.GetOrdinal("rating"));
        return new Category(id, name, rating);
    }
}
