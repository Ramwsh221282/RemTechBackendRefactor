using Identity.Domain.Roles;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Entities.Tickets;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.Policies.RoleChangesProtectionPolicies;
using Identity.Domain.Users.Ports.Passwords;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Aggregate;

public sealed class User
{
    private readonly List<IDomainEvent> _events = [];
    public UserId Id { get; }
    public UserRolesCollection Roles { get; private set; }
    public UserTicketsCollection Tickets { get; private set; } = UserTicketsCollection.Empty();
    public UserProfile Profile { get; private set; }

    public Status Verify(UserPassword password, IPasswordManager passwordManager)
    {
        bool verified = Profile.Password.VerifyPassword(password, passwordManager, out Error error);
        if (!verified)
            return Status.Failure(error);

        return Status.Success();
    }

    public Status ConfirmEmail(UserTicketId ticketId)
    {
        UserTicket? ticket = Tickets.Tickets.FirstOrDefault(t => t.Id == ticketId);
        return ticket == null
            ? Status.NotFound("Заявка подтверждения почты не найдена у пользователя.")
            : ConfirmEmail(ticket);
    }

    public Status FormResetPasswordTicket()
    {
        if (!HasVerifiedEmail())
            return Status.Conflict("Почта пользователя не подтверждена.");

        var issuerId = UserTicketIssuerId.Create(Id.Id);
        var type = UserTicketType.PasswordResetConfirmation;
        var created = DateTime.UtcNow;
        var expires = created.AddHours(1);
        var lifeTime = UserTicketLifeTime.Create(created, expires);
        var ticket = new UserTicket(issuerId, type, lifeTime);

        _events.Add(new UserCreatedEmailConfirmTicket(ticket));
        return Status.Success();
    }

    public Status ConfirmEmail(UserTicket ticket)
    {
        if (HasVerifiedEmail())
            return Status.Conflict("Почта пользователя была подтверждена. Заявка недействительна.");

        if (ticket.Type != UserTicketType.EmailConfirmation)
            return Status.Conflict("Заявка не является заявкой подтверждения почты.");

        Status confirmation = ConfirmTicket(ticket);
        if (confirmation.IsFailure)
            return Status.Failure(confirmation.Error);

        Profile = new UserProfile(Profile, true);
        DropTicket(ticket);

        _events.Add(new UserConfirmedTicket(this, ticket));
        return Status.Success();
    }

    public Status FormEmailConfirmationTicket()
    {
        if (HasVerifiedEmail())
            return Status.Conflict("Пользователь уже имеет подтвержденный email.");

        var created = DateTime.UtcNow;
        var expires = created.AddHours(1);
        var issuerId = UserTicketIssuerId.Create(Id.Id);
        var type = UserTicketType.EmailConfirmation;
        var lifeTime = UserTicketLifeTime.Create(created, expires);
        var ticket = new UserTicket(issuerId, type, lifeTime);

        AddTicket(ticket);
        _events.Add(new UserCreatedEmailConfirmTicket(ticket));
        return Status.Success();
    }

    public Status Promote(User user, IdentityRole role)
    {
        if (user.HasRole(role))
            return Status.Conflict($"У пользователя уже есть роль: {role.Name.Value}");

        if (!this.IsRoleChangeAllowed())
            return Status.Conflict("Нет прав на изменения прав других пользователей.");

        user.AddRole(role);
        return Status.Success();
    }

    public Status Demote(User user, IdentityRole role)
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

    private void AddTicket(UserTicket ticket) =>
        Tickets = new UserTicketsCollection(Tickets, ticket);

    private Status ConfirmTicket(UserTicket ticket) => ticket.Confirm(this);

    private void DropTicket(UserTicket ticket) => Tickets = Tickets.Without(ticket);

    private bool HasVerifiedEmail() => Profile.EmailConfirmed;

    private bool HasRole(IdentityRole role) => Roles.HasRole(role);

    private bool HasNotRole(IdentityRole role) => Roles.HasNotRole(role);

    private void AddRole(IdentityRole role)
    {
        Roles = Roles.With(role);
        _events.Add(new UserPromoted(Id.Id, role.Id.Value));
    }

    private void DropRole(IdentityRole role)
    {
        Roles = Roles.Without(role);
        _events.Add(new UserDemoted(Id.Id, role.Id.Value));
    }

    private User(UserProfile profile, UserRolesCollection roles, UserId? id = null)
    {
        Profile = profile;
        Roles = roles;
        Id = id ?? new UserId();
    }

    public User(User user, UserRolesCollection roles)
        : this(user.Profile, user.Roles, user.Id) => Roles = roles;

    public User(User user, UserTicketsCollection tickets)
        : this(user, user.Roles) => Tickets = tickets;

    public static User Create(
        UserProfile profile,
        UserRolesCollection rolesCollection,
        UserId? id = null
    )
    {
        User user = new User(profile, rolesCollection, id);

        if (id == null)
        {
            var profileEa = profile.ToEventArgs();
            var rolesEa = rolesCollection.Roles.Select(r => r.ToEventArgs());
            user._events.Add(new UserCreated(user.Id.Id, profileEa, rolesEa));
        }

        return user;
    }
}
