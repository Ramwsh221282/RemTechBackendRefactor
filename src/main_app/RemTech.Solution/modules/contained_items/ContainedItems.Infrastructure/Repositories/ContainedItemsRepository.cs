using System.Data;
using ContainedItems.Domain.Contracts;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.Extensions;
using Dapper;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ContainedItems.Infrastructure.Repositories;

public sealed class ContainedItemsRepository(NpgSqlSession session) : IContainedItemsRepository
{
    public async Task<ContainedItem[]> Query(ContainedItemsQuery query, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(query);
        string lockClause = LockClause(query);
        string limitClause = LimitClause(query);
        string sql = $"""
                      SELECT
                          id as id,
                          service_item_id as service_item_id,
                          creator_id as creator_id,
                          creator_type as creator_type,
                          creator_domain as creator_domain,
                          content as content,
                          created_at as created_at,
                          deleted_at as deleted_at,
                          status as status
                      FROM contained_items_module.contained_items
                      {filterSql}
                      {lockClause}
                      {limitClause}
                      """;
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        return await session.QueryMultipleUsingReader(command, MapFromReader);
    }
    
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

    private static (DynamicParameters parameters, string filterSql) WhereClause(ContainedItemsQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            filters.Add("status = @status");
            parameters.Add("@status", query.Status, DbType.String);
        }
        
        return filters.Count == 0 
            ? (parameters, string.Empty) 
            : (parameters, $"WHERE {string.Join(" AND ", filters)}");
    }

    private static string LimitClause(ContainedItemsQuery query)
    {
        return query.Limit.HasValue ? $"LIMIT {query.Limit.Value}" : string.Empty;
    }

    private static string LockClause(ContainedItemsQuery query)
    {
        return query.WithLock ? "FOR UPDATE" : string.Empty;
    }

    private static ContainedItem MapFromReader(IDataReader reader)
    {
        Guid id = reader.GetValue<Guid>("id");
        string itemId = reader.GetValue<string>("service_item_id");
        Guid creatorId = reader.GetValue<Guid>("creator_id");
        string creatorType = reader.GetValue<string>("creator_type");
        string creatorDomain = reader.GetValue<string>("creator_domain");
        string content = reader.GetValue<string>("content");
        DateTime createdAt = reader.GetValue<DateTime>("created_at");
        DateTime? deletedAt = reader.GetNullable<DateTime>("deleted_at");
        string status = reader.GetValue<string>("status");
        ContainedItemId idVo = ContainedItemId.Create(id);
        ServiceItemId itemIdVo = ServiceItemId.Create(itemId);
        ServiceCreatorInfo creatorInfo = ServiceCreatorInfo.Create(
            creatorId: creatorId,
            type: creatorType,
            domain: creatorDomain
        );
        ContainedItemInfo itemInfo = ContainedItemInfo.Create(
            content: content,
            createdAt: createdAt,
            deletedAt: deletedAt
        );
        ContainedItemStatus statusVo = ContainedItemStatus.CreateFromString(status);
        return new ContainedItem(
            id: idVo,
            serviceItemId: itemIdVo,
            creatorInfo: creatorInfo,
            info: itemInfo,
            status: statusVo
        );
    }
}