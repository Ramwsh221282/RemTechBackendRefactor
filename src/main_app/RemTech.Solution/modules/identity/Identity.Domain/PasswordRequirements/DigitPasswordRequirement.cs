using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

/// <summary>
/// Требование к паролю: наличие хотя бы одной цифры.
/// </summary>
public sealed class DigitPasswordRequirement : IAccountPasswordRequirement
{
	/// <summary>
	/// Проверяет, содержит ли пароль хотя бы одну цифру.
	/// </summary>
	/// <param name="password">Пароль для проверки.</param>
	/// <returns>Результат проверки требования к паролю.</returns>
	public Result<Unit> Satisfies(AccountPassword password)
	{
		return !password.Value.Any(char.IsDigit)
			? Error.Validation("Пароль должен содержать хотя бы одну цифру.")
			: Unit.Value;
	}
}
