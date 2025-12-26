using RemTech.ContainedItems.Module.Types;
using RemTech.Core.Shared.Cqrs;

namespace RemTech.ContainedItems.Module.Features.AddContainedItem;

internal sealed record AddContainedItemCommand(IContainedItem Item) : ICommand;
