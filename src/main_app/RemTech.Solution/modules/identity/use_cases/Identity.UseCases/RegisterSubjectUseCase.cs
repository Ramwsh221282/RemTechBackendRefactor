using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.Functional.Extensions;

namespace Identity.UseCases;

public static class RegisterSubjectUseCase
{
    public static RegisterSubject RegisterSubject() => args =>
    {
        Result<Subject> result = Subject.Create(args.Email, args.Login, args.Password);
        return Task.FromResult(result); 
    };
}