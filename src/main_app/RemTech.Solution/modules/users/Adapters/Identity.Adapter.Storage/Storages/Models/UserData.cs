namespace Identity.Adapter.Storage.Storages.Models;

internal sealed class UserData
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required bool EmailConfirmed { get; init; }
    public required string Password { get; init; }
    public required List<RoleData> Roles { get; init; } = [];

    public required List<UserTicketData> Tickets { get; init; } = [];
}
