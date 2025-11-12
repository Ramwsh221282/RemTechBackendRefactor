using Identity.Core.SubjectsModule.Contexts;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;

namespace Identity.Application.Features;

public sealed record RegisterIdentitySubjectArgs(string Email, string Password, string Login, CancellationToken Ct);

public sealed class RegisterIdentitySubject(SubjectEventsRegistry Registry)
{
    public Task<Result<Subject>> Register(RegisterIdentitySubjectArgs args)
    {
        IdentitySubjectMetadataConstructionContext constructMeta = new(args.Login, None<Guid>());
        IdentitySubjectCredentialsConstructionContext constructCreds = new(args.Email, args.Password);
        IdentitySubjectActivationConstructionContext constructActivation = new(None<DateTime>());
        IdentitySubjectPermissionsConstructionContext constructPerms = new([]); 
        IdentitySubjectConstructionContext constructSubject = new(constructMeta, constructCreds, constructActivation, constructPerms);
        return Subject
            .Create(constructSubject, Registry).ContinueAsync<Subject>(s => 
                s.Registered().ContinueAsync<Subject>(r => 
                    Registry.OnSuccessProcession(() => r, args.Ct)));
    }
}