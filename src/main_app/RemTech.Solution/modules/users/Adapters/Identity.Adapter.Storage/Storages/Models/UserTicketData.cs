using Dapper.FluentMap.Mapping;

namespace Identity.Adapter.Storage.Storages.Models;

public sealed class UserTicketData
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string Type { get; init; }
    public required DateTime Created { get; init; }
    public required DateTime Expired { get; init; }
    public required DateTime? Deleted { get; init; }
}

public sealed class UserTicketDataMap : EntityMap<UserTicketData>
{
    public UserTicketDataMap()
    {
        Map(t => t.Id).ToColumn("ticket_id");
        Map(x => x.UserId).ToColumn("ticket_user_id");
        Map(t => t.Type).ToColumn("ticket_type");
        Map(t => t.Created).ToColumn("ticket_created");
        Map(t => t.Expired).ToColumn("ticket_expired");
        Map(t => t.Deleted).ToColumn("ticket_deleted");
    }
}
