using ContainedItems.Domain.Contracts;
using ContainedItems.Domain.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ContainedItems.Domain.Features.AddContainedItems;

[TransactionalHandler]
public sealed class AddContainedItemsHandler(IContainedItemsRepository repository)
	: ICommandHandler<AddContainedItemsCommand, int>
{
	public async Task<Result<int>> Execute(AddContainedItemsCommand command, CancellationToken ct = default)
	{
		IEnumerable<ContainedItem> items = ConvertToContainedItems(command.Items);
		return await repository.AddMany(items, ct);
	}

	private static IEnumerable<ContainedItem> ConvertToContainedItems(IEnumerable<AddContainedItemsBody> bodies) =>
		bodies.Select(ConvertToContainedItem);

	private static ContainedItem ConvertToContainedItem(AddContainedItemsBody body)
	{
		ContainedItemId id = ContainedItemId.New();
		ServiceItemId serviceItemId = ServiceItemId.Create(body.ServiceItemId);
		ServiceCreatorInfo creatorInfo = ServiceCreatorInfo.Create(
			body.CreatorId,
			body.CreatorType,
			body.CreatorDomain
		);
		ContainedItemInfo info = ContainedItemInfo.Create(body.Content);
		return ContainedItem.PendingToSave(id, serviceItemId, creatorInfo, info);
	}
}
