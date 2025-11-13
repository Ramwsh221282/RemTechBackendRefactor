using System.Data;
using Dapper;
using Identity.Core.TicketsModule;
using Identity.Core.TicketsModule.Contracts;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using RemTech.Primitives.Extensions;

namespace Identity.Persistence.NpgSql.TicketsModule;

/* SCHEMA:
CREATE TABLE IF NOT EXISTS identity_module.tickets
(
    id UUID PRIMARY KEY,
    creator_id UUID,
    type VARCHAR(128) not null,
    created timestamp not null,
    closed timestamptz,
    active boolean,
    CONSTRAINT fk_subjects
        FOREIGN KEY (creator_id) REFERENCES identity_module.subjects(id)
            ON DELETE CASCADE ON UPDATE CASCADE
);
*/
public static class NpgSqlTicketsModule
{
    public static InsertTicket InsertTicket(NpgSqlSession session) => async (ticket, ct) =>
        await Unit.Executed(async () => await session.Execute(CreateCommand(
            """
            INSERT INTO identity_module.tickets
            (id, creator_id, type, created, closed, active)
            VALUES
            (@id, @creator_id, @type, @created, @closed, @active);
            """,
            session,
            ticket,
            ct
        ))); 

    public static DeleteTicket DeleteTicket(NpgSqlSession session) => async (ticket, ct) =>
        await Unit.Executed(async () => await session.Execute(CreateCommand(
            """
            DELETE FROM identity_module.tickets
            WHERE id = @id;
            """,
            session,
            ticket,
            ct
            )));

    public static UpdateTicket UpdateTicket(NpgSqlSession session) =>
        async (ticket, ct) => await Unit.Executed(async () => 
            await session.Execute(CreateCommand(
                $"""
                 UPDATE identity_module.tickets
                 {UpdateClause()}
                 """,
                session, ticket, ct
                )));

    public static GetTicket GetTicket(NpgSqlSession session)
    {
        return async (args, ct) =>
        {
            string whereClause = WhereClause(args);
            if (string.IsNullOrWhiteSpace(whereClause))
                return Optional.None<Ticket>();
            
            string sql =
                $"""
                 SELECT * FROM identity_module.tickets
                 WHERE {whereClause}
                 {LockClause(args)}
                 """;
            
            CommandDefinition command = CreateCommand(sql, session, args, ct);
            TableTicket? tableTicket = await session.QueryMaybeRow<TableTicket>(command);
            return OptionalTableTicketToTicket(tableTicket);
        };
    }

    private static CommandDefinition CreateCommand(
        string sql, 
        NpgSqlSession session, 
        QueryTicketArgs args, 
        CancellationToken ct)
    {
        DynamicParameters parameters = FillParameters(args);
        return CreateCommand(sql, session, parameters, ct);
    }

    private static CommandDefinition CreateCommand(
        string sql, 
        NpgSqlSession session, 
        Ticket ticket,
        CancellationToken ct)
    {
        DynamicParameters parameters = FillParameters(ticket);
        return CreateCommand(sql, session, parameters, ct);
    }
    
    private static CommandDefinition CreateCommand(
        string sql, 
        NpgSqlSession session,
        DynamicParameters parameters,
        CancellationToken ct)
    {
        return new CommandDefinition(
            sql, 
            parameters, 
            cancellationToken: ct, 
            transaction: 
            session.Transaction);
    }
    
    private static DynamicParameters FillParameters(Ticket ticket) 
    {
        TicketSnapshot snapshot = ticket.Snapshotted();
        return NpgSqlParametersModule.Scoped(snapshot)
            .With("@id", s => s.Id, DbType.Guid)
            .With("@creator_id", s => s.CreatorId, DbType.Guid)
            .With("@created", s => s.Created)
            .WithNullable("@closed", s => s.Closed, DbType.DateTime)
            .With("@active", s => s.Active)
            .With("@type", s => s.Type, DbType.String)
            .GetParameters();
    }
    
    private static string WhereClause(QueryTicketArgs args)
    {
        List<string> clauses = [];
        if (args.TicketId.HasValue)
            clauses.Add("id = @id");
        if (args.CreatorId.HasValue)
            clauses.Add("creator_id = @creator_id");
        if (!string.IsNullOrWhiteSpace(args.Type))
            clauses.Add("type = @type");
        return clauses.Count < 0 ? string.Empty : "WHERE " + Strings.Joined(clauses, " AND ");
    }

    private static string LockClause(QueryTicketArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }
    
    private static DynamicParameters FillParameters(QueryTicketArgs args)
    {
        return NpgSqlParametersModule.Scoped(args)
            .WithIfNotNull("@id", s => s.TicketId, DbType.Guid)
            .WithIfNotNull("@creator_id", s => s.CreatorId, DbType.Guid)
            .WithIfNotNull("@type", s => s.Type, DbType.String)
            .GetParameters();
    }
    
    private static string UpdateClause()
    {
        string[] clauses =
        [
            "closed = @closed",
            "active = @active"
        ];
        return "SET " + Strings.Joined(clauses, ", ");
    }
    
    private static Result<Optional<Ticket>> OptionalTableTicketToTicket(TableTicket? ticket)
    {
        return ticket == null ? Optional.None<Ticket>() : Optional<Ticket>.Some(TableTicketToTicket(ticket));
    }
    
    private static Result<Ticket> TableTicketToTicket(TableTicket ticket)
    {
        TicketSnapshot snap = new(
            ticket.Id, 
            ticket.CreatorId, 
            ticket.Type, 
            ticket.Created,
            ticket.Closed, 
            ticket.Active);
        return Ticket.Create(snap);
    }
}