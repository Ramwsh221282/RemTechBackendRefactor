﻿using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.DropVehicles;

internal sealed record DropVehiclesCommand(
    string Password,
    string? Email = null,
    string? Name = null
) : ICommand;
