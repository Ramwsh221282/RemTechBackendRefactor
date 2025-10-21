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
    private readonly List<IdentityUserEvent> _events = [];
    private readonly UserId _id;
    private readonly IdentityUserSession _session;
    private readonly IdentityUserRoles _roles;
    private IdentityUserProfile _profile;

    private IdentityUser(
        IdentityUserProfile profile,
        IdentityUserSession session,
        IdentityUserRoles roles,
        UserId? id = null
    )
    {
        _profile = profile;
        _session = session;
        _roles = roles;
        _id = id ?? new UserId();
    }

    public static IdentityUser Create(
        IdentityUserProfile profile,
        IdentityUserSession session,
        IdentityUserRoles roles,
        UserId? id = null
    )
    {
        IdentityUser user = new IdentityUser(profile, session, roles, id);
        if (id == null)
            user._events.Add(
                new IdentityUserCreatedEvent(user._id, user._profile, user._session, user._roles)
            );
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

        IdentityUserCreatedEvent creatorInfo = new(_id, _profile, _session, _roles);
        IdentityUserCreatedEvent createdInfo = new(
            user._id,
            user._profile,
            user._session,
            user._roles
        );

        IdentityUserCreatedByUserEvent @event = new(creatorInfo, createdInfo);
        _events.Add(@event);
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

        IdentityUserRemovedEventInfo removerInfo = new(_id, _profile, _session, _roles);
        IdentityUserRemovedEventInfo removedInfo = new(
            user._id,
            user._profile,
            user._session,
            user._roles
        );

        _events.Add(new IdentityUserRemovedEvent(removerInfo, removedInfo));
        return Status.Success();
    }

    public async Task<Status> PublishEvents(
        IIdentityUserEventHandler handler,
        CancellationToken ct = default
    ) => await handler.Handle(_events, ct);
}
