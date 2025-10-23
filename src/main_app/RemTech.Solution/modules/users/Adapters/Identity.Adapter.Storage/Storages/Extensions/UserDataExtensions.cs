using Identity.Adapter.Storage.Storages.Models;
using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Entities.Tickets;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;

namespace Identity.Adapter.Storage.Storages.Extensions;

internal static class UserDataExtensions
{
    internal static User ToIdentityUser(this UserData data)
    {
        var userId = UserId.Create(data.Id);
        if (userId.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        var login = UserLogin.Create(data.Name);
        if (login.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        var userEmail = UserEmail.Create(data.Email);
        if (userEmail.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");
        bool emailConfirmed = data.EmailConfirmed;

        var password = HashedUserPassword.Create(data.Password);
        if (password.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        var profile = new UserProfile(login, userEmail, password, emailConfirmed);
        var roles = new UserRolesCollection(data.Roles.Select(d => d.ToIdentityRole()));
        var tickets = new UserTicketsCollection(data.Tickets.Select(t => t.ToUserTicket()));
        var user = User.Create(profile, roles, userId);
        user = new User(user, tickets);
        return user;
    }

    private static IdentityRole ToIdentityRole(this RoleData role)
    {
        var id = RoleId.Create(role.Id);
        if (id.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        var name = RoleName.Create(role.Name);
        if (name.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        return IdentityRole.Create(name, id);
    }

    private static UserTicket ToUserTicket(this UserTicketData ticket)
    {
        var id = UserTicketId.Create(ticket.Id);
        if (id.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        var userId = UserTicketIssuerId.Create(ticket.UserId);
        if (userId.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        var lifeTime = UserTicketLifeTime.Create(ticket.Created, ticket.Expired, ticket.Deleted);
        if (lifeTime.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        var type = UserTicketType.Create(ticket.Type);
        if (type.IsFailure)
            throw new ApplicationException("Bad user mapping from db.");

        return new UserTicket(userId, type, lifeTime, id);
    }
}
