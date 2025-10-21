using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Entities;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.Ports.EventHandlers;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Aggregate;

public sealed class IdentityUser
{
    private readonly List<IdentityUserToken> _tokens = [];
    private readonly List<IdentityUserEvent> _events = [];
    private readonly UserId _id;
    private readonly IdentityUserRoles _roles;
    private IdentityUserProfile _profile;

    private IdentityUser(IdentityUserProfile profile, IdentityUserRoles roles, UserId? id = null)
    {
        _profile = profile;
        _roles = roles;
        _id = id ?? new UserId();
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

            user._events.Add(new IdentityUserCreatedEvent(user._id.Id, profileEa, rolesEa));
        }
        return user;
    }

    public Status Verify(UserPassword password, IPasswordManager passwordManager) =>
        _profile.Password.VerifyPassword(password, passwordManager, out Error error)
            ? Status.Failure(error)
            : Status.Success();

    public void ChangeEmail(UserEmail email)
    {
        _profile = new IdentityUserProfile(_profile.Login, email, _profile.Password);
        _events.Add(new IdentityUserEmailChangedEvent(_id, _profile));
    }

    public void ChangePassword(HashedUserPassword password)
    {
        _profile = new IdentityUserProfile(_profile.Login, _profile.Email, password);
        _events.Add(new IdentityUserEmailChangedEvent(_id, _profile));
    }

    public Status RegisterUser(IdentityUser user)
    {
        if (_roles.HasNotRole(RoleName.Admin) && _roles.HasNotRole(RoleName.Root))
            return Status.Forbidden("Пользователь не имеет прав добавления пользователей.");

        if (_roles.HasRole(RoleName.Root))
            if (user._roles.HasRole(RoleName.Root))
                return Status.Forbidden(
                    "Root пользователь не имеет прав добавления root пользователей."
                );

        if (_roles.HasRole(RoleName.Admin))
            if (user._roles.HasRole(RoleName.Admin))
                return Status.Forbidden(
                    "Admin Пользователь не имеет прав добавления admin пользователей."
                );

        IdentityUserCreatedEvent creatorInfo = new(
            _id.Id,
            _profile.ToEventArgs(),
            _roles.Roles.Select(r => r.ToEventArgs())
        );

        IdentityUserCreatedEvent createdInfo = new(
            user._id.Id,
            user._profile.ToEventArgs(),
            user._roles.Roles.Select(r => r.ToEventArgs())
        );

        IdentityUserCreatedByUserEvent @event = new(creatorInfo, createdInfo);
        _events.Add(@event);
        return Status.Success();
    }

    public Status FormEmailConfirmationToken()
    {
        if (_profile.HasEmailConfirmed())
            return Status.Conflict("Пользователь уже подтвердил Email.");

        IdentityTokenId tokenId = new();
        DateTime created = DateTime.UtcNow;
        DateTime expires = created.AddMinutes(10);
        string tokenType = "EMAIL_CONFIRMATION";
        IdentityUserToken token = new(tokenId, _id, created, expires, tokenType);

        EmailConfirmationCreatedTokenEvent @event = new(token, _profile, _id);
        _events.Add(@event);
        _tokens.Add(token);
        return Status.Success();
    }

    public Status ConfirmEmail(IdentityTokenId tokenId)
    {
        if (_profile.HasEmailConfirmed())
            return Status.Conflict("Пользователь уже подтвердил Email.");

        IdentityUserToken? userToken = _tokens.FirstOrDefault(t => t.Id == tokenId);
        if (userToken == null)
            return Status.NotFound("Не найден токен пользователя.");

        if (userToken.HasExpired())
            return Status.NotFound("Токен закончился.");

        if (!userToken.BelongsToUser(_id))
            return Status.Conflict("Токен не принадлежит данному пользователю.");

        if (!userToken.IsOfType("EMAIL_CONFIRMATION"))
            return Status.Conflict("Токен не является токеном подтверждения почты.");

        _profile.ConfirmEmail();
        _events.Add(new IdentityUserEmailConfirmedEvent(_id, _profile, userToken));
        _tokens.Remove(userToken);
        return Status.Success();
    }

    public Status RemoveUser(IdentityUser user)
    {
        if (_roles.HasNotRole(RoleName.Admin) && _roles.HasNotRole(RoleName.Root))
            return Status.Forbidden("Пользователь не имеет прав удаления пользователей.");

        if (_roles.HasRole(RoleName.Root))
            if (user._roles.HasRole(RoleName.Root))
                return Status.Forbidden(
                    "Root Пользователь не имеет прав удаления root пользователей."
                );

        if (_roles.HasRole(RoleName.Admin))
            if (user._roles.HasRole(RoleName.Admin))
                return Status.Forbidden(
                    "Admin Пользователь не имеет прав удаления admin пользователей."
                );

        IdentityUserRemovedEventInfo removerInfo = new(_id, _profile, _roles);
        IdentityUserRemovedEventInfo removedInfo = new(user._id, user._profile, user._roles);

        _events.Add(new IdentityUserRemovedEvent(removerInfo, removedInfo));
        return Status.Success();
    }

    public IdentityUserToken? FindToken(Func<IdentityUserToken, bool> predicate) =>
        _tokens.FirstOrDefault(predicate);

    public async Task<Status> PublishEvents(
        IIdentityUserEventHandler handler,
        CancellationToken ct = default
    ) => await handler.Handle(_events, ct);
}
