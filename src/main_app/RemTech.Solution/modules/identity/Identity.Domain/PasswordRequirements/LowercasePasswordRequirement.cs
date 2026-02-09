using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

/// <summary>
/// Требование к паролю: наличие хотя бы одной строчной буквы.
/// </summary>
public sealed class LowercasePasswordRequirement : IAccountPasswordRequirement
{
	/// <summary>
	/// Проверяет, содержит ли пароль хотя бы одну строчную букву.
	/// </summary>
	/// <param name="password">Пароль для проверки.</param>
	/// <returns>Результат проверки требования к паролю.</returns>
	public Result<Unit> Satisfies(AccountPassword password)
	{
		return !password.Value.Any(char.IsLower)
			? Error.Validation("Пароль должен содержать хотя бы одну строчную букву.")
			: Unit.Value;
	}
}
