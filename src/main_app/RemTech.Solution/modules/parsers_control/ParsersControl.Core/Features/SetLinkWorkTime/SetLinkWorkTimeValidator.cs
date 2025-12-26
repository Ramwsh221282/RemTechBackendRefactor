using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetLinkWorkTime;

public sealed class SetLinkWorkTimeValidator : AbstractValidator<SetLinkWorkingTimeCommand>
{
    public SetLinkWorkTimeValidator()
    {
        RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
        RuleFor(x => x.LinkId).MustBeValid(SubscribedParserLinkId.From);
    }
}