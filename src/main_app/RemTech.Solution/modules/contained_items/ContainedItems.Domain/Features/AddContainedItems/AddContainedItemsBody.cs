namespace ContainedItems.Domain.Features.AddContainedItems;

/// <summary>
/// Тело запроса для добавления содержащихся элементов.
/// </summary>
/// <param name="ServiceItemId">Идентификатор сервисного элемента.</param>
/// <param name="ItemType">Тип содержащихся элементов.</param>
/// <param name="CreatorId">Идентификатор создателя.</param>
/// <param name="CreatorDomain">Домен создателя.</param>
/// <param name="CreatorType">Тип создателя.</param>
/// <param name="Content">Содержимое элементов.</param>
public sealed record AddContainedItemsBody(
	string ServiceItemId,
	string ItemType,
	Guid CreatorId,
	string CreatorDomain,
	string CreatorType,
	string Content
);
