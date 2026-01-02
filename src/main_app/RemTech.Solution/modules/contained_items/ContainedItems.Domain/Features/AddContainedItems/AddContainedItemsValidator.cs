using ContainedItems.Domain.Models;
using FluentValidation;
using RemTech.SharedKernel.Core.Handlers;

namespace ContainedItems.Domain.Features.AddContainedItems;

public sealed class AddContainedItemsValidator : AbstractValidator<AddContainedItemsCommand>
{
    public AddContainedItemsValidator()
    {
        RuleFor(x => x.Items)
            .EachMustFollow(
            [
                i => ServiceItemId.Create(i.ServiceItemId),
                i => ServiceCreatorInfo.Create(i.CreatorId, i.CreatorType, i.CreatorDomain),
                i => ContainedItemInfo.Create(i.Content),
            ]);
    }
}