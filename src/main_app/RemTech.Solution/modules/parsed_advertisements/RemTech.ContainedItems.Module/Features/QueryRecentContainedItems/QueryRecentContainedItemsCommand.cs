using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal record QueryRecentContainedItemsCommand(int Page) : ICommand;
