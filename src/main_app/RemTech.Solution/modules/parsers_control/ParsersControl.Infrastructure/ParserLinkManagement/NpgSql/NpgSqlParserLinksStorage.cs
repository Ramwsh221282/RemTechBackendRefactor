using System.Data;
using Dapper;
using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserLinkManagement.NpgSql;

public sealed class NpgSqlParserLinksStorage(NpgSqlSession session) : IParserLinksStorage
{
    public async Task Persist(ParserLink instance, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO parsers_control_module.links
                           (id, name, url, is_ignored, parser_id)
                           VALUES
                           (@id, @name, @url, @is_ignored, @parser_id);
                           """;
        NpgSqlParserLinkParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    public async Task<ParserLink?> Fetch(ParserLinkQueryArgs args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(args);
        string lockClause = LockClause(args);
        string sql = $"""
                      SELECT
                      id,
                      name,
                      url,
                      is_ignored,
                      parser_id
                      FROM parsers_control_module.links
                      {filterSql}
                      {lockClause}
                      LIMIT 1
                      """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlParserLink? row = await session.QueryMaybeRow<NpgSqlParserLink?>(command);
        return row?.ToParserLink();
    }

    public async Task Update(ParserLink instance, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE parsers_control_module.links 
                           SET
                               name = @name,
                               url = @url,
                               is_ignored = @is_ignored
                           WHERE id = @id
                           """;
        NpgSqlParserLinkParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    private static string LockClause(ParserLinkQueryArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }
    
    private static (DynamicParameters parameters, string filterSql) WhereClause(ParserLinkQueryArgs args)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (args.Id.HasValue)
        {
            filters.Add("id=@id");
            parameters.Add("@id", args.Id.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(args.Name))
        {
            filters.Add("name=@name");
            parameters.Add("@name", args.Name, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(args.Url))
        {
            filters.Add("url=@url");
            parameters.Add("@url", args.Url, DbType.String);
        }

        if (args.ParserId.HasValue)
        {
            filters.Add("parser_id=@parser_id");
            parameters.Add("@parser_id", args.ParserId.Value, DbType.Guid);
        }
        
        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, sql);
    }

    private sealed class NpgSqlParserLink
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Url { get; init; }
        public required bool IsIgnored { get; init; }
        public required Guid ParserId { get; init; }

        public ParserLink ToParserLink()
        {
            ParserLinkData data = new(Id, Name, Url, IsIgnored, ParserId);
            return new ParserLink(data);
        }
    }
    
    private sealed class NpgSqlParserLinkParameters
    {
        private readonly DynamicParameters _parameters = new();
        private void AddId(Guid id) => _parameters.Add("@id", id, DbType.Guid);
        private void AddName(string name) => _parameters.Add("@name", name, DbType.String);
        private void AddUrl(string url) => _parameters.Add("@url", url, DbType.String);
        private void AddIgnored(bool ignored) => _parameters.Add("@is_ignored", ignored, DbType.Boolean);
        private void AddParserId(Guid id) => _parameters.Add("@parser_id", id, DbType.Guid);
        public DynamicParameters Read() => _parameters;
        public NpgSqlParserLinkParameters(ParserLink link)
        {
            link.Write(AddId, AddUrl, AddName, AddIgnored, AddParserId);
        }
    }
}