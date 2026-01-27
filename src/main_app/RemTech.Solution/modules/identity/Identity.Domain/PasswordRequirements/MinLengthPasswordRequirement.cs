using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

/// <summary>
/// Требование к паролю: минимальная длина.
/// </summary>
public sealed class MinLengthPasswordRequirement : IAccountPasswordRequirement
{
	private const int Length = 8;

	/// <summary>
	/// Проверяет, соответствует ли пароль минимальной длине.
	/// </summary>
	/// <param name="password">Пароль для проверки.</param>
	/// <returns>Результат проверки требования к паролю.</returns>
	public Result<Unit> Satisfies(AccountPassword password) =>
		password.Value.Length < Length
			? Error.Validation($"Пароль должен быть длиннее {Length} символов.")
			: Unit.Value;
}
