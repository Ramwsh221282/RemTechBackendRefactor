using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

/// <summary>
/// Валидатор команды <see cref="PermantlyStartParsingCommand"/>.
/// </summary>
public sealed class PermantlyStartParsingValidator : AbstractValidator<PermantlyStartParsingCommand>
{
	/// <summary>
	/// Конструктор валидатора с правилами валидации.
	/// </summary>
	public PermantlyStartParsingValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
	}
}
