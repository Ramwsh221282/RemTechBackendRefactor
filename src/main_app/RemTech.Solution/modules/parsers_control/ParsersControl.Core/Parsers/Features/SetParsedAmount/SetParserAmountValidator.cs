using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.SetParsedAmount;

public sealed class SetParserAmountValidator : AbstractValidator<SetParsedAmountCommand>
{
    public SetParserAmountValidator()
    {
        RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
    }
}