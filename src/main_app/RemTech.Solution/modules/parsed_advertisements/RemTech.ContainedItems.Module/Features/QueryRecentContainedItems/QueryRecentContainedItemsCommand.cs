using RemTech.Core.Shared.Cqrs;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal record QueryRecentContainedItemsCommand(int Page) : ICommand;
