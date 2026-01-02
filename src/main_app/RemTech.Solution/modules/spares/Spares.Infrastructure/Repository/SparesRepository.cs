using System.Data;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using Spares.Domain.Contracts;
using Spares.Domain.Models;

namespace Spares.Infrastructure.Repository;

public sealed class SparesRepository(NpgSqlSession session, ISpareAddressProvider addressProvider) : ISparesRepository
{
    public async Task<int> AddMany(IEnumerable<Spare> spares, CancellationToken ct = default)
    {
        DynamicParameters parameters = new();
        List<string> insertClauses = [];
        
        await FillParameters(parameters, insertClauses, spares, ct);
        CommandDefinition command = CreateInsertCommand(parameters, insertClauses);
        return await ExecuteCommand(command, ct);
    }

    private async Task<int> ExecuteCommand(CommandDefinition command, CancellationToken ct)
    {
        NpgsqlConnection connection = await session.GetConnection(ct);
        int affected = await connection.ExecuteAsync(command);
        return affected;
    }
    
    private CommandDefinition CreateInsertCommand(DynamicParameters parameters, List<string> insertClauses)
    {
        string insertClause = string.Join(", ", insertClauses);
        string insertSql = $"INSERT INTO spares_module.spares (url, id, price, is_nds, oem, text, region_id, type) VALUES {insertClause}";
        CommandDefinition command = new(insertSql, parameters, transaction: session.Transaction);
        return command;
    }
    
    private async Task FillParameters(DynamicParameters parameters, List<string> insertClauses, IEnumerable<Spare> spares, CancellationToken ct)
    {
        int index = 0;
        foreach (Spare spare in spares)
        {
            Result<Guid> regionId = await addressProvider.SearchRegionId(spare.Details.Address.Value, ct);
            if (regionId.IsFailure) continue;
            
            string url = spare.Source.Url;
            Guid id = spare.Id.Value;
            long price = spare.Details.Price.Value;
            bool isNds = spare.Details.Price.IsNds;
            string oem = spare.Details.Oem.Value;
            string text = spare.Details.Text.Value;
            string type = spare.Details.Type.Value;
            
            string urlParam = $"@url_{index}";
            string idParam = $"@id_{index}";
            string priceParam = $"@price_{index}";
            string isNdsParam = $"@is_nds_{index}";
            string oemParam = $"@oem_{index}";
            string textParam = $"@text_{index}";
            string regionIdParam = $"@region_id_{index}";
            string typeParam = $"@type_{index}";
            
            parameters.Add(urlParam, url, DbType.String);
            parameters.Add(idParam, id, DbType.Guid);
            parameters.Add(priceParam, price, DbType.Int64);
            parameters.Add(isNdsParam, isNds, DbType.Boolean);
            parameters.Add(oemParam, oem, DbType.String);
            parameters.Add(textParam, text, DbType.String);
            parameters.Add(regionIdParam, regionId.Value, DbType.Guid);
            parameters.Add(typeParam, type, DbType.String);

            string insertClause = $"({urlParam}, {idParam}, {priceParam}, {isNdsParam}, {oemParam}, {textParam}, {regionIdParam}, {typeParam})";
            insertClauses.Add(insertClause);
            index++;
        }
    }
    
    // public async Task<int> AddMany(IEnumerable<Spare> spares, CancellationToken ct = default)
    // {
    //     const string sql = """
    //                        INSERT INTO spares_module.spares (id, contained_item_id, url, content)
    //                        SELECT @id, @contained_item_id, @url, @content::jsonb
    //                        WHERE NOT EXISTS (
    //                            SELECT 1 FROM spares_module.spares
    //                            WHERE id = @id OR url = @url
    //                        );
    //                        """;
    //     object[] parameters = spares.Select(s => s.ExtractForParameters()).ToArray();
    //     return await session.ExecuteBulkWithAffectedCount(sql, parameters);
    // }
}