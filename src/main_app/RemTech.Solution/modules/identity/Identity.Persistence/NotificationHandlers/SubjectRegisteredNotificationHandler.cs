using System.Data;
using Dapper;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NotificationHandlers;

internal abstract class NpgSqlSubjectNotificationHandler : IIdentitySubjectNotificationPersistenceHandler
{
    private Func<NpgSqlSession, CancellationToken, Task<Result<Unit>>> _handler;

    public NpgSqlSubjectNotificationHandler()
    {
        _handler = async (_, _) =>
        {
            await Task.Yield();
            return Unit.Value;
        };
    }

    public abstract void Record(IdentitySubjectNotification notification);
    
    public async Task<Result<Unit>> Handle(NpgSqlSession session, CancellationToken ct = default)
    {
        Task<Result<Unit>> task = _handler(session, ct);
        return await task;
    }
    
    protected void AttachHandle (Func<NpgSqlSession, CancellationToken, Task<Result<Unit>>> handler) =>
        _handler = handler;
}

internal sealed class SubjectRegisteredNotificationHandler : NpgSqlSubjectNotificationHandler
{
    public override void Record(IdentitySubjectNotification notification)
    {
        if (notification is IdentitySubjectRegisteredNotification registered)
            AttachHandle(async (session, token) =>
            {
                const string sql
                    = """
                      INSERT INTO identity_module.subjects(id, email, login, password, isActive)
                      VALUES (@id, @email, @login, @password, @isActive)
                      """;

                DynamicParameters parameters = new();
                parameters.Add("@id", registered.Id, DbType.Guid);
                parameters.Add("@email", registered.Email, DbType.String);
                parameters.Add("@login", registered.Login, DbType.String);
                parameters.Add("@password", registered.Password, DbType.String);
                parameters.Add(@"isActive", false, DbType.Boolean);
                await session.Execute(parameters, sql, token);
                return Unit.Value;
            });
    }
}