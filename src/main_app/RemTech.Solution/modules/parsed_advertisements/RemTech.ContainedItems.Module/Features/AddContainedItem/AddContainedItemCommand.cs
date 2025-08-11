using RemTech.ContainedItems.Module.Types;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.AddContainedItem;

internal sealed record AddContainedItemCommand(IContainedItem Item) : ICommand;
