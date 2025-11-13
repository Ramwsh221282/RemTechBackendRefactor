using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Persistence.NpgSql.SubjectsModule;
using RemTech.Functional.Extensions;

namespace Identity.Application.Features.EmailChange;

public sealed class ChangeEmail(NpgSqlSubjectCommands commands, NotificationsRegistry registry)
{
    public async Task<Result<Unit>> Handle(ChangeEmailArgs args)
    {
        SubjectQueryArgs queryArgs = new(Id: args.Id);
        Optional<Subject> subject = await commands.FindSingle(queryArgs, args.Ct);
        if (subject.NoValue) return NotFound("Учетная запись не найдена.");
        Subject withRegistry = subject.Value.With(registry);
        Result<Subject> withChangedEmail = withRegistry.WithOtherEmail(args.NextEmail);
        return withChangedEmail.IsFailure ? withChangedEmail.Error : await registry.ProcessNotifications(args.Ct);
    }
}