using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class ChangeEmailPersistingUseCase
{
    internal static ChangeEmail ChangeEmail(ChangeEmail origin, SubjectsStorage storage) =>
        async args =>
        {
            SubjectQueryArgs query = new(Id: args.Id);
            CancellationToken ct = args.Ct;
            Optional<Subject> subject = await storage.Find(query, ct);
            ChangeEmailArgs withSubject = args with { Target = subject };
            
            Result<Subject> changed = await origin(withSubject);
            if (changed.IsFailure) return changed.Error;
            if (!await changed.Value.IsEmailUnique(storage, ct))
                return Error.Conflict($"Почта: {args.NextEmail} занята.");
            
            Result<Unit> saving = await changed.Value.UpdateIn(storage, ct);
            return saving.IsFailure ? saving.Error : changed;
        };

    internal static ChangeEmail ChangeEmail(ChangeEmail origin, NpgSqlSession session) =>
        async args =>
        {
            await session.GetTransaction(args.Ct);
            Result<Subject> changed = await origin(args);
            if (changed.IsFailure) return changed.Error;
            bool hasCommited = await session.Commited(args.Ct); 
            
            return !hasCommited 
                ? Error.Application("Не удается зафиксировать изменения") 
                : changed;
        };

    extension(ChangeEmail origin)
    {
        public ChangeEmail WithPersisting(IServiceProvider sp)
        {
            SubjectsStorage storage = sp.Resolve<SubjectsStorage>();
            NpgSqlSession session = sp.Resolve<NpgSqlSession>();
            return ChangeEmail(ChangeEmail(origin, storage), session);
        }
    }
}