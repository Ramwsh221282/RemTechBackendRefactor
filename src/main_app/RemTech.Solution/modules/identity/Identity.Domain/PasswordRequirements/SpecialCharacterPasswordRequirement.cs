using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public sealed class SpecialCharacterPasswordRequirement : IAccountPasswordRequirement
{
	private static readonly char[] SpecialChars = "!@#$%^&*()_+-=[]{};:'\",.<>/?\\|`~".ToCharArray();

	public Result<Unit> Satisfies(AccountPassword password)
	{
		if (!password.Value.Any(c => SpecialChars.Contains(c)))
			return Error.Validation("Пароль должен содержать хотя бы один спецсимвол.");
		return Unit.Value;
	}
}
