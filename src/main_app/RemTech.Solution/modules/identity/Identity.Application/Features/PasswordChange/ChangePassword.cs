using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Persistence.NpgSql.SubjectsModule;
using RemTech.Functional.Extensions;

namespace Identity.Application.Features.PasswordChange;

public sealed class ChangePassword(
    NotificationsRegistry registry, 
    NpgSqlSubjectCommands commands, 
    HashPassword hash)
{
    public async Task<Result<Unit>> Handle(ChangePasswordArgs args)
    {
        SubjectQueryArgs queryArgs = new(Id: args.Id);
        Optional<Subject> subject = await commands.FindSingle(queryArgs, args.Ct);
        if (subject.NoValue) return NotFound("Учетная запись не найдена.");
        Subject attached = subject.Value.With(registry);
        Result<Subject> withChangedPassword = attached.WithOtherPassword(args.NextPassword, hash);
        if (withChangedPassword.IsFailure) return withChangedPassword.Error;
        return await registry.ProcessNotifications(args.Ct);
    }
}