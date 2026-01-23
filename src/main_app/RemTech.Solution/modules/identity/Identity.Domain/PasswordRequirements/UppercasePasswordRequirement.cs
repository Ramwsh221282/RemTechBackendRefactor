using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public sealed class UppercasePasswordRequirement : IAccountPasswordRequirement
{
	public Result<Unit> Satisfies(AccountPassword password) =>
		!password.Value.Any(char.IsUpper)
			? Error.Validation("Пароль должен содержать хотя бы одну заглавную букву.")
			: Unit.Value;
}
