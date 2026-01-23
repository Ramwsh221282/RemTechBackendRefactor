using FluentValidation;
using ParsersControl.Core.Common;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.SetWorkTime;

public sealed class SetWorkTimeValidator : AbstractValidator<SetWorkTimeCommand>
{
	public SetWorkTimeValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.TotalElapsedSeconds).MustBeValid(ParsingWorkTime.FromTotalElapsedSeconds);
	}
}
