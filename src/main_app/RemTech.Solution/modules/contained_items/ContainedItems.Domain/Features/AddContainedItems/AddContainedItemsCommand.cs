using RemTech.SharedKernel.Core.Handlers;

namespace ContainedItems.Domain.Features.AddContainedItems;

public sealed record AddContainedItemsCommand(IEnumerable<AddContainedItemsBody> Items) : ICommand;