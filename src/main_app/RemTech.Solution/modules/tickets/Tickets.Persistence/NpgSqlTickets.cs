using System.Data;
using System.Reflection;
using Dapper;
using Identity.Persistence.NpgSql.TicketsModule;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using RemTech.Primitives.Extensions;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Persistence;

// CREATE TABLE IF NOT EXISTS identity_module.tickets
// (
//     id UUID primary key,
//     creator_id UUID NOT NULL,
//     type varchar(128) NOT NULL,
//     created timestamptz NOT NULL,
//     closed timestamptz,
//     active boolean
// );
public static class NpgSqlTickets
{
    private static readonly Assembly Assembly = typeof(NpgSqlTickets).Assembly;
    
    public static Insert Insert(NpgSqlSession session) => async (ticket, ct) =>
    {
        string sql =
            """
            INSERT INTO identity_module.tickets
            (id, creator_id, type, created, closed, active)
            VALUES
            (@id, @creator_id, @type, @created, @closed, @active)
            """;
        CommandDefinition command = ticket.CreateCommand(sql, session, ct);
        await session.Execute(command);
        return Unit.Value;
    };

    public static Delete Delete(NpgSqlSession session) => async (ticket, ct) =>
    {
        const string sql = "DELETE FROM identity_module.tickets WHERE id = @id";
        CommandDefinition command = ticket.CreateCommand(sql, session, ct);
        await session.Execute(command);
        return Unit.Value;
    };

    public static Update Update(NpgSqlSession session) => async (ticket, ct) =>
    {
        string updateClause = ticket.UpdateClause();
        string sql = $"UPDATE identity_module.tickets {updateClause} WHERE id = @id";
        CommandDefinition command = ticket.CreateCommand(sql, session, ct);
        await session.Execute(command);
        return Unit.Value;
    };

    public static Find Find(NpgSqlSession session) => async (args, ct) =>
    {
        string whereClause = args.WhereClause();
        if (string.IsNullOrWhiteSpace(whereClause)) return Optional<Ticket>.None();
        string lockClause = args.LockClause();
        string sql = $"SELECT * FROM identity_module.tickets {whereClause} {lockClause}";
        CommandDefinition command = args.CreateCommand(sql, session, ct);
        TableTicket? ticket = await session.QueryMaybeRow<TableTicket>(command);
        return ticket.ToMaybeTicket();
    };

    extension(TableTicket? ticket)
    {
        private Optional<Ticket> ToMaybeTicket()
        {
            return ticket == null ? Optional.None<Ticket>() : Optional<Ticket>.Some(ticket.ToTicket());
        }
    }

    extension(TableTicket ticket)
    {
        private Ticket ToTicket()
        {
            TicketMetadata metadata = TicketMetadata.Create(ticket.CreatorId, ticket.Id, ticket.Type);
            TicketLifecycle lifecycle = TicketLifecycle.Create(ticket.Id, ticket.Created, ticket.Closed, ticket.Status);
            return Ticket.Create(metadata, lifecycle);
        }
    }
    
    extension(QueryTicketArgs args)
    {
        private CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            DynamicParameters parameters = args.FillParameters();
            return new CommandDefinition(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        }

        private DynamicParameters FillParameters()
        {
            DynamicParameters parameters = new();
            if (args.TicketId.HasValue) parameters.Add("@id", args.TicketId.Value, DbType.Guid);
            if (args.CreatorId.HasValue) parameters.Add("@creator_id", args.CreatorId.Value, DbType.Guid);
            if (!string.IsNullOrWhiteSpace(args.Type)) parameters.Add("@type", args.Type, DbType.String);
            return parameters;
        }
        
        private string LockClause()
        {
            return args.WithLock ? " FOR UPDATE " : string.Empty;
        }
        
        private string WhereClause()
        {
            string[] clauses = args.WhereClauses();
            return clauses.Length == 0 ? string.Empty : "WHERE " + Strings.Joined(clauses, " AND ");
        }
        
        private string[] WhereClauses()
        {
            List<string> clauses = [];
            if (args.TicketId.HasValue) clauses.Add("id = @id");
            if (args.CreatorId.HasValue) clauses.Add("creator_id = @creator_id");
            if (!string.IsNullOrWhiteSpace(args.Type)) clauses.Add("type = @type");
            return clauses.ToArray();
        }
    }
    
    extension(Ticket ticket)
    {
        private string UpdateClause()
        {
            List<string> clauses = [];
            clauses.Add("id = @id");
            clauses.Add("creator_id = @creator_id");
            clauses.Add("type = @type");
            clauses.Add("created = @created");
            clauses.Add("closed = @closed");
            clauses.Add("status = @status");
            return "SET " + Strings.Joined(clauses, ", ");
        }
        
        private DynamicParameters FillParameters()
        {
            TicketSnapshot snapshot = ticket.Snapshot();
            TicketMetadataSnapshot metadata = snapshot.Metadata;
            TicketLifeCycleSnapshot lifeCycle = snapshot.LifeCycle;
            
            return NpgSqlParametersStorage.New()
                .With("@id", metadata.TicketId, DbType.Guid)
                .With("@creator_id", metadata.CreatorId, DbType.Guid)
                .With("@type", metadata.Type, DbType.String)
                .With("@created", lifeCycle.Created, DbType.DateTime)
                .WithValueOrNull("@closed", lifeCycle.Closed, c => c.HasValue, c => c!.Value, DbType.DateTime)
                .With("@status", lifeCycle.Status, DbType.Boolean)
                .GetParameters();
        }

        private CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            DynamicParameters parameters = ticket.FillParameters();
            return new CommandDefinition(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        }
    }
    
    extension(IServiceCollection services)
    {
        public void AddTicketsPersistence()
        {
            services.AddScopedDelegate<Insert>(Assembly);
            services.AddScopedDelegate<Delete>(Assembly);
            services.AddScopedDelegate<Update>(Assembly);
            services.AddScopedDelegate<Find>(Assembly);
            services.AddScoped<TicketsStorage>();
        }
    }
}