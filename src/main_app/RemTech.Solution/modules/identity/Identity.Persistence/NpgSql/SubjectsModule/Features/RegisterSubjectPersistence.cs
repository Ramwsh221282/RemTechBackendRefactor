using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class RegisterSubjectPersistence
{
    private static RegisterSubject WithPersistence(RegisterSubject origin, SubjectsStorage storage) => async args =>
    {
        Result<Subject> subject = await origin(args);
        if (subject.IsFailure) return subject.Error;
            
        Result<Unit> saving = await subject.Value.SaveTo(storage, args.Ct);
        return saving.IsSuccess ? subject : saving.Error;
    };
    
    extension(RegisterSubject origin)
    {
        public RegisterSubject WithPersistence(IServiceProvider sp)
        {
           SubjectsStorage storage = sp.Resolve<SubjectsStorage>();
           return WithPersistence(origin, storage);
        }
    }
}