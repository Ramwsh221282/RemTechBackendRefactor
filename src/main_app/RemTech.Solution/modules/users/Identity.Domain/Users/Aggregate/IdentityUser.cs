using Identity.Domain.Roles;
using Identity.Domain.Roles.Events;
using Identity.Domain.Users.Entities;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.Policies.RoleChangesProtectionPolicies;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Aggregate;

public sealed class IdentityUser
{
    private readonly List<IDomainEvent> _events = [];
    public UserId Id { get; }
    public IdentityUserRoles Roles { get; private set; }
    public IdentityUserProfile Profile { get; private set; }

    private IdentityUser(IdentityUserProfile profile, IdentityUserRoles roles, UserId? id = null)
    {
        Profile = profile;
        Roles = roles;
        Id = id ?? new UserId();
    }

    public static IdentityUser Create(
        IdentityUserProfile profile,
        IdentityUserRoles roles,
        UserId? id = null
    )
    {
        IdentityUser user = new IdentityUser(profile, roles, id);
        if (id == null)
        {
            IdentityUserProfileEventArgs profileEa = profile.ToEventArgs();
            IEnumerable<RoleEventArgs> rolesEa = roles.Roles.Select(r => r.ToEventArgs());
            user._events.Add(new IdentityUserCreatedEvent(user.Id.Id, profileEa, rolesEa));
        }
        return user;
    }

    public Status Verify(UserPassword password, IPasswordManager passwordManager)
    {
        bool verified = Profile.Password.VerifyPassword(password, passwordManager, out Error error);
        if (!verified)
            return Status.Failure(error);

        return Status.Success();
    }

    public void ChangeEmail(UserEmail email)
    {
        Profile = new IdentityUserProfile(Profile.Login, email, Profile.Password);
        _events.Add(new IdentityUserEmailChangedEvent(Id, Profile));
    }

    public void ChangePassword(HashedUserPassword password)
    {
        Profile = new IdentityUserProfile(Profile.Login, Profile.Email, password);
        _events.Add(new IdentityUserEmailChangedEvent(Id, Profile));
    }

    public Status FormConfirmationTicket()
    {
        if (HasVerifiedEmail())
            return Status.Conflict("Пользователь уже имеет подтвержденный email.");

        throw new NotImplementedException();
    }

    public Status Promote(IdentityUser user, IdentityRole role)
    {
        if (user.HasRole(role))
            return Status.Conflict($"У пользователя уже есть роль: {role.Name.Value}");

        if (!this.IsRoleChangeAllowed())
            return Status.Conflict("Нет прав на изменения прав других пользователей.");

        user.AddRole(role);
        return Status.Success();
    }

    public Status Demote(IdentityUser user, IdentityRole role)
    {
        if (!this.IsRoleChangeAllowed())
            return Status.Conflict("Нет прав на изменения прав других пользователей.");

        if (!user.CanBeChangedBy(this))
            return Status.Conflict("Нельзя изменить роль этому пользователю.");

        if (user.HasNotRole(role))
            return Status.Conflict($"У пользователя нет роли: {role.Name.Value}");

        user.DropRole(role);
        return Status.Success();
    }

    public async Task<Status> PublishEvents(
        IDomainEventsDispatcher dispatcher,
        CancellationToken ct = default
    ) => await dispatcher.Dispatch(_events, ct);

    private bool HasVerifiedEmail() => Profile.EmailConfirmed;

    private bool HasRole(IdentityRole role) => Roles.HasRole(role);

    private bool HasNotRole(IdentityRole role) => Roles.HasNotRole(role);

    private void AddRole(IdentityRole role)
    {
        Roles = Roles.With(role);
        _events.Add(new IdentityUserPromotedEvent(Id.Id, role.Id.Value));
    }

    private void DropRole(IdentityRole role)
    {
        Roles = Roles.Without(role);
        _events.Add(new IdentityUserDemotedEvent(Id.Id, role.Id.Value));
    }
}
