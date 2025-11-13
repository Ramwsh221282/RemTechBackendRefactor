using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Persistence.NpgSql;
using RemTech.Functional.Extensions;

namespace Identity.Application.Features.AccountActivation;

public sealed record RequireAccountActivationArgs(Guid SubjectId, CancellationToken Ct);

public sealed class RequireAccountActivation(
    NpgSqlIdentityCommands Db, 
    NotificationsRegistry Registry)
{
    public async Task<Result<Unit>> Invoke(RequireAccountActivationArgs args)
    {
        throw new NotImplementedException();
        
        // await Db.Session.ExecuteUnderTransaction(async () =>
        // {
        //      Db.Subjects TODO create get subjects method.
        // });
    }
}