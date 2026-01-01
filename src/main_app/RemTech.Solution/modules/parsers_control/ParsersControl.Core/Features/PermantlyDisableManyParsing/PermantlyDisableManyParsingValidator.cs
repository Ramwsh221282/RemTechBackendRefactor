using FluentValidation;
using ParsersControl.Core.Features.PermantlyStartManyParsing;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyDisableManyParsing;

public sealed class PermantlyDisableManyParsingValidator : AbstractValidator<PermantlyStartManyParsingCommand>
{
    public PermantlyDisableManyParsingValidator()
    {
        RuleFor(x => x.Identifiers).EachMustFollow([SubscribedParserId.Create]);
    }
}