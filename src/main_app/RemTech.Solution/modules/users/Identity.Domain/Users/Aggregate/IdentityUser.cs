using Identity.Domain.Roles;
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
    private readonly List<IdentityUserToken> _tokens = [];
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

            IEnumerable<IdentityUserRoleEventArgs> rolesEa = roles.Roles.Select(r =>
                r.ToEventArgs()
            );

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

    public Status FormEmailConfirmationToken()
    {
        if (Profile.HasEmailConfirmed())
            return Status.Conflict("Пользователь уже подтвердил Email.");

        IdentityTokenId tokenId = new();
        DateTime created = DateTime.UtcNow;
        DateTime expires = created.AddMinutes(10);
        string tokenType = "EMAIL_CONFIRMATION";
        IdentityUserToken token = new(tokenId, Id, created, expires, tokenType);

        EmailConfirmationCreatedTokenEvent @event = new(token, Profile, Id);
        _events.Add(@event);
        _tokens.Add(token);
        return Status.Success();
    }

    public Status ConfirmEmail(IdentityTokenId tokenId)
    {
        if (Profile.HasEmailConfirmed())
            return Status.Conflict("Пользователь уже подтвердил Email.");

        IdentityUserToken? userToken = _tokens.FirstOrDefault(t => t.Id == tokenId);
        if (userToken == null)
            return Status.NotFound("Не найден токен пользователя.");

        if (userToken.HasExpired())
            return Status.NotFound("Токен закончился.");

        if (!userToken.BelongsToUser(Id))
            return Status.Conflict("Токен не принадлежит данному пользователю.");

        if (!userToken.IsOfType("EMAIL_CONFIRMATION"))
            return Status.Conflict("Токен не является токеном подтверждения почты.");

        Profile.ConfirmEmail();
        _events.Add(new IdentityUserEmailConfirmedEvent(Id, Profile, userToken));
        _tokens.Remove(userToken);
        return Status.Success();
    }

    public IdentityUserToken? FindToken(Func<IdentityUserToken, bool> predicate) =>
        _tokens.FirstOrDefault(predicate);

    public async Task<Status> PublishEvents(
        IDomainEventsDispatcher dispatcher,
        CancellationToken ct = default
    ) => await dispatcher.Dispatch(_events, ct);
}
