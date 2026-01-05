using System.Data;
using Dapper;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common.UnitOfWork;

public sealed class AccountTicketsChangeTracker(NpgSqlSession session)
{
    private NpgSqlSession Session { get; } = session;
    private readonly Dictionary<Guid, AccountTicket> _tracking = [];

    public void StartTracking(IEnumerable<AccountTicket> tickets)
    {
        foreach (AccountTicket ticket in tickets) _tracking.TryAdd(ticket.TicketId, ticket);
    }

    public async Task SaveChanges(IEnumerable<AccountTicket> tickets, CancellationToken ct)
    {
        IEnumerable<AccountTicket> tracking = GetTrackingTickets(tickets);
        if (!tracking.Any()) return;
        await SaveTicketChanges(tracking, ct);
    }

    private async Task SaveTicketChanges(IEnumerable<AccountTicket> tickets, CancellationToken ct = default)
    {
        List<string> updateSet = [];
        DynamicParameters parameters = new();

        if (tickets.Any(t => t.Finished != _tracking[t.TicketId].Finished))
        {
            string clause = string.Join(" ", tickets.Select((t, i) =>
            {
                string paramName = $"@finished_{i}";
                parameters.Add(paramName, t.Finished, DbType.Boolean);
                return $"{WhenClause(i)} THEN {paramName}";
            }));
            updateSet.Add($"finished = CASE {clause} ELSE finished END");
        }

        if (updateSet.Count == 0) return;
        
        List<Guid> ids = [];
        int index = 0;
        foreach (AccountTicket ticket in tickets)
        {
            Guid id = ticket.TicketId;
            string paramName = $"@ticket_id_{index}";
            parameters.Add(paramName, id, DbType.Guid);
            ids.Add(id);
            index++;
        }
        
        parameters.Add("@ids", ids.ToArray());
        
        string updateSql = $"""
                           UPDATE identity_module.account_tickets ac
                           SET {string.Join(", ", updateSet)}
                           WHERE ac.ticket_id = ANY (@ids)
                           """;
        
        CommandDefinition command = Session.FormCommand(updateSql, parameters, ct);
        await Session.Execute(command);
    }

    private IEnumerable<AccountTicket> GetTrackingTickets(IEnumerable<AccountTicket> tickets)
    {
        List<AccountTicket> tracking = [];
        foreach (AccountTicket ticket in tickets)
        {
            if (_tracking.ContainsKey(ticket.TicketId))
                tracking.Add(ticket);
        }
        return tracking;
    }

    private string WhenClause(int index)
    {
        return $"WHEN ac.ticket_id = @ticket_id_{index}";
    }
}