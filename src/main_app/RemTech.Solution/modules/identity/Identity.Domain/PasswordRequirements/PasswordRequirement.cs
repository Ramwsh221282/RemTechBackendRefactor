using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

/// <summary>
/// Комбинированное требование к паролю, состоящее из нескольких отдельных требований.
/// </summary>
public sealed class PasswordRequirement : IAccountPasswordRequirement
{
	private readonly List<IAccountPasswordRequirement> _requirements = [];

	/// <summary>
	/// Проверяет, удовлетворяет ли пароль всем требованиям.
	/// </summary>
	/// <param name="password">Пароль для проверки.</param>
	/// <returns>Результат проверки требования к паролю.</returns>
	public Result<Unit> Satisfies(AccountPassword password)
	{
		List<string> errors = [];
		foreach (IAccountPasswordRequirement requirement in _requirements)
		{
			Result<Unit> validation = requirement.Satisfies(password);
			if (validation.IsFailure)
			{
				errors.Add(validation.Error.Message);
			}
		}

		return errors.Count == 0 ? Unit.Value : Error.Validation(string.Join(", ", errors));
	}

	/// <summary>
	/// Добавляет требование к паролю в комбинированное требование.
	/// </summary>
	/// <param name="requirement">Требование к паролю для добавления.</param>
	/// <returns>Текущий экземпляр комбинированного требования к паролю.</returns>
	public PasswordRequirement Use(IAccountPasswordRequirement requirement)
	{
		_requirements.Add(requirement);
		return this;
	}

	/// <summary>
	/// Добавляет несколько требований к паролю в комбинированное требование.
	/// </summary>
	/// <param name="requirements">Требования к паролю для добавления.</param>
	/// <returns>Текущий экземпляр комбинированного требования к паролю.</returns>
	public PasswordRequirement Use(IEnumerable<IAccountPasswordRequirement> requirements)
	{
		_requirements.AddRange(requirements);
		return this;
	}
}
