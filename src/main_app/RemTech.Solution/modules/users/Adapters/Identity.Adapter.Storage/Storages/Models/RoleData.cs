using Dapper.FluentMap.Mapping;

namespace Identity.Adapter.Storage.Storages.Models;

internal sealed class RoleData
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

internal sealed class RoleDataMap : EntityMap<RoleData>
{
    public RoleDataMap()
    {
        Map(r => r.Id).ToColumn("role_id");
        Map(r => r.Name).ToColumn("role_name");
    }
}
