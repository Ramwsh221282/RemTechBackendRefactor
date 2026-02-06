using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

/// <summary>
/// Требование к паролю: наличие хотя бы одной заглавной буквы.
/// </summary>
public sealed class UppercasePasswordRequirement : IAccountPasswordRequirement
{
	/// <summary>
	/// Проверяет, содержит ли пароль хотя бы одну заглавную букву.
	/// </summary>
	/// <param name="password">Пароль для проверки.</param>
	/// <returns>Результат проверки требования к паролю.</returns>
	public Result<Unit> Satisfies(AccountPassword password)
	{
		return !password.Value.Any(char.IsUpper)
			? Error.Validation("Пароль должен содержать хотя бы одну заглавную букву.")
			: Unit.Value;
	}
}
