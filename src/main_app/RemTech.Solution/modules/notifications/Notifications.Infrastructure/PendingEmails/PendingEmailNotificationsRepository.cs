using System.Data;
using Dapper;
using Notifications.Core.Common.Contracts;
using Notifications.Core.PendingEmails;
using Notifications.Core.PendingEmails.Contracts;
using Npgsql;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Notifications.Infrastructure.PendingEmails;

public sealed class PendingEmailNotificationsRepository(
    NpgSqlSession session, 
    INotificationsModuleUnitOfWork changeTracker) : IPendingEmailNotificationsRepository
{
    private NpgSqlSession Session { get; } = session;
    private INotificationsModuleUnitOfWork ChangeTracker { get; } = changeTracker;
    
    public async Task Add(PendingEmailNotification notification, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO notifications_module.pending_emails
                           (id, recipient, subject, body, was_sent)
                           VALUES
                           (@id, @recipient, @subject, @body, @was_sent)
                           """;
        
        object parameters = new
        {
            id = notification.Id,
            recipient = notification.Recipient,
            subject = notification.Subject,
            body = notification.Body,
            was_sent = notification.WasSent
        };
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        await Session.Execute(command);
    }

    public async Task<PendingEmailNotification[]> GetMany(PendingEmailNotificationsSpecification spec, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(spec);
        string lockClause = LockClause(spec);
        string limitClause = LimitClause(spec);
        string sql = $"""
                      SELECT
                      n.id as id,
                      n.recipient as recipient,
                      n.subject as subject,
                      n.body as body,
                      n.was_sent as was_sent
                      FROM notifications_module.pending_emails n
                      {filterSql}
                      {limitClause}
                      {lockClause}
                      """;
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        PendingEmailNotification[] notifications = await Session.QueryMultipleUsingReader(command, Map);
        ChangeTracker.Track(notifications);
        return notifications;
    }

    public async Task<int> Remove(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default)
    {
        if (!notifications.Any()) return 0;
        const string sql = "DELETE FROM notifications_module.pending_emails WHERE id = ANY (@ids)";
        DynamicParameters parameters = new();
        parameters.Add("@ids", notifications.Select(n => n.Id).ToArray());
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        NpgsqlConnection connection = await Session.GetConnection(ct);
        return await connection.ExecuteAsync(command);
    }

    private static PendingEmailNotification Map(IDataReader reader)
    {
        Guid id = reader.GetValue<Guid>("id");
        string recipient = reader.GetValue<string>("recipient");
        string subject = reader.GetValue<string>("subject");
        string body = reader.GetValue<string>("body");
        bool wasSent = reader.GetValue<bool>("was_sent");
        return new PendingEmailNotification(id, recipient, subject, body, wasSent);
    }

    private static (DynamicParameters parameters, string sql) WhereClause(
        PendingEmailNotificationsSpecification specification)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();

        if (specification.SentOnly.HasValue)
        {
            filters.Add("n.was_sent is true");
        }

        if (specification.NotSentOnly.HasValue)
        {
            filters.Add("n.was_sent is false");
        }
        
        return (parameters, filters.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", filters));
    }
    
    private static string LockClause(PendingEmailNotificationsSpecification specification) =>
        specification.LockRequired.HasValue && specification.LockRequired.Value ? "FOR UPDATE OF n" : string.Empty;
    
    private static string LimitClause(PendingEmailNotificationsSpecification specification) =>
        specification.Limit.HasValue ? $"LIMIT {specification.Limit.Value}" : string.Empty;
}