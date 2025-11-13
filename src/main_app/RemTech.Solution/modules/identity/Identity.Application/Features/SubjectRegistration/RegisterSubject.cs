using Identity.Core.SubjectsModule.Contexts;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;

namespace Identity.Application.Features.SubjectRegistration;

public sealed class RegisterSubject(NotificationsRegistry Registry)
{
    public Task<Result<Subject>> Register(RegisterSubjectArgs args)
    {
        SubjectMetadataConstructionContext constructMeta = new(args.Login, None<Guid>());
        SubjectCredentialsConstructionContext constructCreds = new(args.Email, args.Password);
        SubjectActivationConstructionContext constructActivation = new(None<DateTime>());
        SubjectPermissionsConstructionContext constructPerms = new([]); 
        IdentitySubjectConstructionContext constructSubject = new(constructMeta, constructCreds, constructActivation, constructPerms);
        return Subject
            .Create(constructSubject, Registry).ContinueAsync<Subject>(s => 
                s.Registered().ContinueAsync<Subject>(r => 
                    Registry.OnSuccessProcession(() => r, args.Ct)));
    }
}