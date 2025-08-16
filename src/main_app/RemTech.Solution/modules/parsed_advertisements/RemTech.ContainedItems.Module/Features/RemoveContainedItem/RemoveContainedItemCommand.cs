using Cleaners.Module.Contracts.ItemCleaned;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.RemoveContainedItem;

internal sealed record RemoveContainedItemCommand(ItemCleanedMessage Message) : ICommand;
