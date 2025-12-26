using RemTech.Core.Shared.Cqrs;

namespace RemTech.ContainedItems.Module.Features.RemoveContainedItem;

internal sealed record RemoveContainedItemCommand(ItemCleanedMessage Message) : ICommand;
