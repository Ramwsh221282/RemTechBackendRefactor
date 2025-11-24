using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Outbox.Shared;

namespace Tests.ModuleFixtures;

public sealed class IdentityModule
{
    private readonly IServiceProvider _sp;

    public IdentityModule(IServiceProvider sp)
    {
        _sp = sp;
    }

    public async Task<Result<Permission>> RegisterPermission(string name)
    {
        await using AsyncServiceScope scope = Scope();
        CancellationToken ct = CancellationToken.None;
        RegisterPermissionArgs args = new(name, ct);
        RegisterPermission useCase = scope.Resolve<RegisterPermission>();
        return await useCase(args);
    }

    public async Task<Result<Subject>> ChangeSubjectEmail(Guid subjectId, string email)
    {
        ChangeEmailArgs args = new(subjectId, email, Optional<Subject>.None(), CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        ChangeEmail useCase = scope.Resolve<ChangeEmail>();
        return await useCase(args);
    }

    public async Task<Result<Subject>> ChangePassword(Guid subjectId, string password)
    {
        ChangePasswordArgs args = new(subjectId, password, Optional.None<Subject>(), CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        ChangePassword useCase = scope.Resolve<ChangePassword>();
        return await useCase(args);
    }

    public async Task<Result<Subject>> RegisterSubject(string login, string email, string password)
    {
        RegisterSubjectUseCaseArgs args = new(login, email, password, CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        RegisterSubject usecase = scope.Resolve<RegisterSubject>();
        return await usecase(args);
    }

    public async Task<Result<Subject>> UpdateSubject(Subject subject)
    {
        await using AsyncServiceScope scope = Scope();
        SubjectsStorage storage = scope.Resolve<SubjectsStorage>();
        await subject.UpdateIn(storage, CancellationToken.None);
        return subject;
    }

    public async Task<Result<RequireActivationTicketResult>> RequireActivationTicket(Guid subjectId)
    {
        RequireActivationTicketArgs args = new(subjectId, Optional.None<Subject>(),
            Optional<NotificationsRegistry>.None(), CancellationToken.None);

        await using AsyncServiceScope scope = Scope();
        RequireActivationTicket useCase = scope.Resolve<RequireActivationTicket>();
        return await useCase(args);
    }

    public async Task<Result<SubjectTicket>> RequirePasswordResetTicket(Guid subjectId)
    {
        RequirePasswordResetTicketArgs args = RequirePasswordResetTicketArgs.Default(subjectId, CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        RequirePasswordResetTicket useCase = scope.Resolve<RequirePasswordResetTicket>();
        return await useCase(args);
    }

    public async Task<Result<Subject>> AddSubjectPermission(Guid subjectId, Guid permissionId)
    {
        AddSubjectPermissionArgs args = new(
            permissionId,
            subjectId,
            Optional.None<Subject>(),
            Optional.None<Permission>(),
            CancellationToken.None);
        await using AsyncServiceScope scope = Scope();
        AddSubjectPermission useCase = scope.Resolve<AddSubjectPermission>();
        return await useCase(args);
    }

    public async Task<bool> OutboxHasMessages()
    {
        OutboxServicesRegistry registry = _sp.Resolve<OutboxServicesRegistry>();
        await using AsyncServiceScope scope = Scope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        OutboxService service = registry.GetService(session, "identity_module");
        return await service.HasAny(CancellationToken.None);
    }

    public async Task<IEnumerable<OutboxMessage>> GetOutboxMessages(int maxAmount)
    {
        OutboxServicesRegistry registry = _sp.Resolve<OutboxServicesRegistry>();
        await using AsyncServiceScope scope = Scope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        OutboxService service = registry.GetService(session, "identity_module");
        return await service.GetPendingMessages(maxAmount, CancellationToken.None);
    }
    
    private AsyncServiceScope Scope()
    {
        return _sp.CreateAsyncScope();
    }
}