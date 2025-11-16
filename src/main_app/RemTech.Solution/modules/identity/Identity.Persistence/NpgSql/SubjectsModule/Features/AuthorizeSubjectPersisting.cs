using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class AuthorizeSubjectPersisting
{
    private static AuthorizeSubject AuthorizeSubject(
        AuthorizeSubject origin,
        SubjectsStorage storage,
        Optional<NotificationsRegistry> registry) => async args =>
    {
        if (args.Email.NoValue && args.Login.NoValue)
            return Error.Conflict("Не удается авторизоваться. Не указаны данные входа Email или Login.");

        Optional<Subject> subject = await ResolveSujectAuthorization(args, storage);
        return await origin(args with { Target = subject, Registry = registry });
    };

    private static async Task<Optional<Subject>> ResolveSujectAuthorization(
        AuthorizeSubjectArgs args, 
        SubjectsStorage storage)
    {
        SubjectQueryArgs query = args.Login.HasValue switch
        {
            true => new SubjectQueryArgs(Login: args.Login.Value),
            false => new SubjectQueryArgs(Email: args.Email.Value)
        };
        
        return await storage.Find(query, args.Ct);
    }

    extension(AuthorizeSubject origin)
    {
        public AuthorizeSubject WithPeristence(IServiceProvider sp, Optional<NotificationsRegistry> registry)
        {
            SubjectsStorage storage = sp.Resolve<SubjectsStorage>();
            return AuthorizeSubject(origin, storage, registry);
        }
    }
}