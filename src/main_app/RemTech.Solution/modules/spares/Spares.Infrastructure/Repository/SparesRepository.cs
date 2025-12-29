using RemTech.SharedKernel.Infrastructure.Database;
using Spares.Domain.Models;
using Spares.Infrastructure.Extensions;

namespace Spares.Infrastructure.Repository;

public sealed class SparesRepository(NpgSqlSession session) : ISparesRepository
{
    public async Task<int> AddMany(IEnumerable<Spare> spares, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO spares_module.spares (id, contained_item_id, url, content)
                           SELECT @id, @contained_item_id, @url, @content::jsonb
                           WHERE NOT EXISTS (
                               SELECT 1 FROM spares_module.spares
                               WHERE id = @id OR url = @url
                           );
                           """;
        object[] parameters = spares.Select(s => s.ExtractForParameters()).ToArray();
        return await session.ExecuteBulkWithAffectedCount(sql, parameters);
    }
}