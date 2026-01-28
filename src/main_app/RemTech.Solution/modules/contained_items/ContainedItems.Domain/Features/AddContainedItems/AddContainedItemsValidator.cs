using ContainedItems.Domain.Models;
using FluentValidation;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ContainedItems.Domain.Features.AddContainedItems;

/// <summary>
/// Валидатор команды для добавления содержащихся элементов.
/// </summary>
public sealed class AddContainedItemsValidator : AbstractValidator<AddContainedItemsCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AddContainedItemsValidator"/>.
	/// </summary>
	public AddContainedItemsValidator()
	{
		RuleFor(x => x.Items)
			.EachMustFollow([
				i => ServiceItemId.Create(i.ServiceItemId),
				i => ServiceCreatorInfo.Create(i.CreatorId, i.CreatorType, i.CreatorDomain),
				i => ContainedItemInfo.Create(i.Content),
			]);
	}
}
