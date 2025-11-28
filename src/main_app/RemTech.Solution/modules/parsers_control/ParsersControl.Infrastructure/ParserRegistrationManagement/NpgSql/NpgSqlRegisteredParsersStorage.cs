using System.Data;
using Dapper;
using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserRegistrationManagement.NpgSql;

public sealed class NpgSqlRegisteredParsersStorage(NpgSqlSession session) : IRegisteredParsersStorage
{
    public async Task Persist(RegisteredParser instance, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO parsers_control_module.registered_parsers
                           (id, domain, type)
                           VALUES
                           (@id, @domain, @type)
                           """;
        NpgSqlRegisteredParserParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    public async Task<RegisteredParser?> Fetch(RegisteredParserQuery args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = FillParameters(args);
        string lockClause = LockClause(args);
        string sql = $"""
                      SELECT
                      id,
                      domain,
                      type
                      FROM
                      parsers_control_module.registered_parsers
                      {filterSql}
                      {lockClause}
                      LIMIT 1
                      """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlRegisteredParserRow? row = await session.QueryMaybeRow<NpgSqlRegisteredParserRow?>(command);
        return row?.ToRegisteredParser();
    }

    private static string LockClause(RegisteredParserQuery query)
    {
        return query.WithLock ? "FOR UPDATE" : string.Empty;
    }
    
    private static (DynamicParameters parameters, string filterSql) FillParameters(RegisteredParserQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (query.Id.HasValue)
        {
            filters.Add("@id=id");
            parameters.Add("@id", query.Id.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.Domain))
        {
            filters.Add("@domain=domain");
            parameters.Add("@domain", query.Domain, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            filters.Add("@type=type");
            parameters.Add("@type", query.Type, DbType.String);
        }
        
        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, sql);
    }
    
    private sealed class NpgSqlRegisteredParserParameters
    {
        private readonly DynamicParameters _parameters = new();
        private void AddId(Guid id) =>  _parameters.Add("@id", id, DbType.Guid);
        private void AddDomain(string domain) =>  _parameters.Add("@domain", domain, DbType.String);
        private void AddType(string type) =>  _parameters.Add("@type", type, DbType.String);

        public NpgSqlRegisteredParserParameters(RegisteredParser parser)
        {
            parser.Write(writeId: AddId, writeDomain: AddDomain, writeType: AddType);
        }

        public DynamicParameters Read()
        {
            return _parameters;
        }
    }

    private sealed class NpgSqlRegisteredParserRow
    {
        public required Guid Id { get; init; }
        public required string Domain { get; init; }
        public required string Type { get; init; }

        public RegisteredParser ToRegisteredParser()
        {
            return new RegisteredParser(new ParserData(Id, Type, Domain));
        }
    }
}