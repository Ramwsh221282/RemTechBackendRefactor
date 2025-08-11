using System.Data.Common;
using Brands.Module.Types;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Brands.Module.Features.GetBrand;

internal sealed class GetBrandByNameHandler(NpgsqlConnection connection)
    : ICommandHandler<GetBrandCommand, IBrand>
{
    public async Task<IBrand> Handle(GetBrandCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new BrandRawByNameEmptyNameException();
        string sql = string.Intern(
            """
            SELECT id, name, rating FROM brands_module.brands
            WHERE name = @name;
            """
        );
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@name", command.Name));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new BrandRawByNameNotFoundException(command.Name);
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        long rating = reader.GetInt64(reader.GetOrdinal("rating"));
        return new Brand(id, name, rating);
    }
}
