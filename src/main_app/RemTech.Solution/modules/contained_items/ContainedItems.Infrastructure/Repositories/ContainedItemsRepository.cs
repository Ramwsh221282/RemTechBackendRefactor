using ContainedItems.Domain.Contracts;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.Extensions;
using Dapper;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ContainedItems.Infrastructure.Repositories;

public sealed class ContainedItemsRepository(NpgSqlSession session) : IContainedItemsRepository
{
    public async Task<int> AddMany(IEnumerable<ContainedItem> items, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO contained_items_module.contained_items 
                               (id, service_item_id, creator_id, creator_type, creator_domain, content, created_at, deleted_at, status)
                           VALUES
                               (@id, @service_item_id, @creator_id, @creator_type, @creator_domain, @content::jsonb, @created_at, @deleted_at, @status)
                           ON CONFLICT (service_item_id) DO NOTHING 
                           """;
        object[] parameters = items.ExtractForParameters();
        return await session.ExecuteBulkWithAffectedCount(sql, parameters);
    }

    public async Task UpdateMany(IEnumerable<ContainedItem> items, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE contained_items_module.contained_items
                           set deleted_at = @deleted_at,
                               status = @status
                           WHERE id = @id    
                           """;
        object[] parameters = items.ExtractForParameters();
        await session.ExecuteBulk(sql, parameters);
    }
}