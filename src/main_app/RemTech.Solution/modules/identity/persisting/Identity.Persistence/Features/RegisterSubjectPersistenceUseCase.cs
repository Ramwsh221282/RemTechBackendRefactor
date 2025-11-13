using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Features;

public static class RegisterSubjectPersistenceUseCase
{
    extension(RegisterSubject origin)
    {
        public RegisterSubject WithPersistence(SubjectsStorage Storage) => async (args) =>
        {
            Result<Subject> subject = await origin(args);
            if (subject.IsFailure) return subject.Error;

            SubjectSnapshot snapshot = subject.Value.Snapshot();

            if (!await Storage.IsEmailUnique(snapshot.Email, args.Ct))
                return Error.Conflict($"Почта: {snapshot.Email} занята.");
            
            if (!await Storage.IsLoginUnique(snapshot.Login, args.Ct))
                return Error.Conflict($"Логин: {snapshot.Login} занят.");

            Result<Unit> saving = await Storage.Insert(subject, args.Ct);
            return saving.IsSuccess ? subject : saving.Error;
        };
    }
}