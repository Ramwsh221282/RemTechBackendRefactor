using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.SetLinkParsedAmount;

public sealed class SetLinkParsedAmountValidator : AbstractValidator<SetLinkParsedAmountCommand>
{
    public SetLinkParsedAmountValidator()
    {
        RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
        RuleFor(x => x.LinkId).MustBeValid(SubscribedParserLinkId.From);
    }
}
