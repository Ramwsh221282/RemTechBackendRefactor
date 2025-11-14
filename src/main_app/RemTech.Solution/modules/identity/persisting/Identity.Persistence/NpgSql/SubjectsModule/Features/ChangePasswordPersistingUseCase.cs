using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class ChangePasswordPersistingUseCase
{
    internal static ChangePassword ChangePassword(ChangePassword origin, SubjectsStorage storage) =>
        async args =>
        {
            CancellationToken ct = args.Ct;
            SubjectQueryArgs querySubject = new(Id: args.Id);
            Optional<Subject> subject = await storage.Find(querySubject, ct);
            ChangePasswordArgs withSubject = args with { Target = subject };
            
            Result<Subject> withChangedPassword = await origin(withSubject);
            if (withChangedPassword.IsFailure) return withChangedPassword.Error;
            
            Result<Unit> saving = await withChangedPassword.Value.UpdateIn(storage, ct);
            return saving.IsSuccess ? withChangedPassword : saving.Error;
        };

    internal static ChangePassword ChangePassword(ChangePassword origin, NpgSqlSession session) =>
        async args =>
        {
            CancellationToken ct = args.Ct;
            await session.GetTransaction(ct);
            Result<Subject> result = await origin(args);
            return !await session.Commited(ct) ? Error.Application("Не удается сохранить изменения.") : result;
        };

    extension(ChangePassword origin)
    {
        public ChangePassword WithPersisting(IServiceProvider sp)
        {
            SubjectsStorage storage = sp.Resolve<SubjectsStorage>();
            NpgSqlSession session=  sp.Resolve<NpgSqlSession>();
            return ChangePassword(ChangePassword(origin, storage), session);
        }
    }
}