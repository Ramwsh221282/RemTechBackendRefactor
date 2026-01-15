using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.ChangeLinkActivity;

public sealed class ChangeLinkActivityLinkValidator : AbstractValidator<ChangeLinkActivityCommand>
{
    public ChangeLinkActivityLinkValidator()
    {
        RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
        RuleFor(x => x.LinkId).MustBeValid(SubscribedParserLinkId.From);
    }
}
