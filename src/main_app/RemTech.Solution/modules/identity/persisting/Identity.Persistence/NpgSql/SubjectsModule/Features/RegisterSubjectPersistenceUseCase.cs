using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class RegisterSubjectPersistenceUseCase
{
    extension(RegisterSubject origin)
    {
        public RegisterSubject WithPersistence(SubjectsStorage storage) => async (args) =>
        {
            Result<Subject> subject = await origin(args);
            if (subject.IsFailure) return subject.Error;
            
            Result<Unit> saving = await subject.Value.SaveTo(storage, args.Ct);
            return saving.IsSuccess ? subject : saving.Error;
        };
    }
}