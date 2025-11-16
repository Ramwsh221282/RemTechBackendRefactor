using System.Data;
using System.Reflection;
using Dapper;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule.SubjectTickets;

public static class NpgSqlSubjectTickets
{
    private static readonly Assembly Assembly = typeof(NpgSqlSubjectTickets).Assembly;
    
    public static Insert Insert(NpgSqlSession session) => async (ticket, ct) =>
    {
        string sql = """
                     INSERT INTO identity_module.subject_tickets
                     (id, creator_id, type, active)
                     VALUES
                     (@id, @creator_id, @type, @active)
                     """;
        CommandDefinition command = ticket.CreateCommand(sql, session, ct);
        await session.Execute(command);
        return Unit.Value;
    };

    extension(SubjectTicket ticket)
    {
        private DynamicParameters FillParameters()
        {
            SubjectTicketSnapshot snap = ticket.Snapshot();
            if (snap.CreatorId.NoValue)
                throw new InvalidOperationException("Subject ticket cannot be saved without creator id.");

            return NpgSqlParametersStorage.New()
                .With("@id", snap.Id, DbType.Guid)
                .With("@active", snap.Active, DbType.Boolean)
                .With("@creator_id", snap.CreatorId.Value, DbType.Guid)
                .With("@type", snap.Type, DbType.String)
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
        public void AddSubjectTicketsPersistence()
        {
            services.AddScopedDelegate<Insert>(Assembly);
            services.AddScoped<SubjectTicketsStorage>();
        }
    }
}