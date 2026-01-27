using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

/// <summary>
/// Требование к паролю: наличие хотя бы одного спецсимвола.
/// </summary>
public sealed class SpecialCharacterPasswordRequirement : IAccountPasswordRequirement
{
	/// <summary>
	/// Проверяет, содержит ли пароль хотя бы один спецсимвол.
	/// </summary>
	private static readonly char[] SpecialChars = "!@#$%^&*()_+-=[]{};:'\",.<>/?\\|`~".ToCharArray();

	/// <summary>
	/// Проверяет, содержит ли пароль хотя бы один спецсимвол.
	/// </summary>
	/// <param name="password">Пароль для проверки.</param>
	/// <returns>Результат проверки требования к паролю.</returns>
	public Result<Unit> Satisfies(AccountPassword password)
	{
		return !password.Value.Any(c => SpecialChars.Contains(c))
			? (Result<Unit>)Error.Validation("Пароль должен содержать хотя бы один спецсимвол.")
			: (Result<Unit>)Unit.Value;
	}
}
