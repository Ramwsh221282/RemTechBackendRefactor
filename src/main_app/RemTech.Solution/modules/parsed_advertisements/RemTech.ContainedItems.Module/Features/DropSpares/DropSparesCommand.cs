using RemTech.Core.Shared.Cqrs;

namespace RemTech.ContainedItems.Module.Features.DropSpares;

internal sealed record DropSparesCommand(string Password, string? Email = null, string? Name = null)
    : ICommand;
