using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

/// <summary>
/// Интерфейс требования к паролю аккаунта.
/// </summary>
public interface IAccountPasswordRequirement
{
	/// <summary>
	/// Проверяет, удовлетворяет ли пароль требованию.
	/// </summary>
	/// <param name="password">Пароль для проверки.</param>
	/// <returns>Результат проверки требования к паролю.</returns>
	Result<Unit> Satisfies(AccountPassword password);
}
