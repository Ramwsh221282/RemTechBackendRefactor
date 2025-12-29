using FluentValidation;
using RemTech.SharedKernel.Core.Handlers;
using Spares.Domain.Models;

namespace Spares.Domain.Features;

public sealed class AddSpareCommandValidator : AbstractValidator<AddSparesCommand>
{
    public AddSpareCommandValidator()
    {
        RuleFor(x => x.Spares)
            .EachMustFollow(
            [
                s => SpareId.Create(s.SpareId),
                s => ContainedItemId.Create(s.ContainedItemId),
                s => SpareOem.Create(s.Oem),
                s => SpareTextInfo.Create(s.Title),
                s => SparePrice.Create(s.Price, s.IsNds),
                s => SparePhotoCollection.Create((IEnumerable<string>)s.PhotoPaths)
            ]);
    }
}