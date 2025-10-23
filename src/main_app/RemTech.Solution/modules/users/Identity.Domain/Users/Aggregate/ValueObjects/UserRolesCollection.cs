using Identity.Domain.Roles;
using Identity.Domain.Roles.Events;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Entities.Tickets;

namespace Identity.Domain.Users.Aggregate.ValueObjects;

public sealed record UserRolesCollection(IEnumerable<IdentityRole> Roles)
{
    public bool HasRole(IdentityRole role) => HasRole(role.Name);

    public bool HasNotRole(IdentityRole role) => HasNotRole(role.Name);

    public static IEnumerable<RoleEventArgs> ToEventArgs(UserRolesCollection rolesCollection) =>
        rolesCollection.Roles.Select(r => r.ToEventArgs());

    public UserRolesCollection With(IdentityRole role) => new([role, .. Roles]);

    public UserRolesCollection Without(IdentityRole role) =>
        new(Roles.Where(r => r.Name != role.Name));

    private bool HasRole(RoleName name) => Roles.Any(r => r.Name == name);

    private bool HasNotRole(RoleName name) => !Roles.Any(r => r.Name == name);
}

public sealed record UserTicketsCollection(IEnumerable<UserTicket> Tickets)
{
    public static UserTicketsCollection Empty() => new([]);

    public UserTicketsCollection(UserTicketsCollection tickets, UserTicket ticket)
        : this(tickets) => Tickets = [ticket, .. Tickets];

    public UserTicketsCollection Without(UserTicket ticket)
    {
        IEnumerable<UserTicket> without = Tickets.Where(t => t.Id != ticket.Id);
        return new(without);
    }
}
