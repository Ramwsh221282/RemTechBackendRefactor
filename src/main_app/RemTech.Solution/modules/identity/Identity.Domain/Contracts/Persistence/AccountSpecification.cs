namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Спецификация для поиска аккаунтов.
/// </summary>
public sealed class AccountSpecification
{
	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public Guid? Id { get; private set; }

	/// <summary>
	/// Электронная почта аккаунта.
	/// </summary>
	public string? Email { get; private set; }

	/// <summary>
	/// Логин аккаунта.
	/// </summary>
	public string? Login { get; private set; }

	/// <summary>
	/// Флаг, указывающий на необходимость блокировки аккаунта при выборке.
	/// </summary>
	public bool LockRequired { get; private set; }

	/// <summary>
	/// Токен обновления аккаунта.
	/// </summary>
	public string? RefreshToken { get; private set; }

	/// <summary>
	/// Устанавливает токен обновления для спецификации.
	/// </summary>
	/// <param name="refreshToken">Токен обновления.</param>
	/// <returns>Обновленная спецификация аккаунта.</returns>
	public AccountSpecification WithRefreshToken(string refreshToken)
	{
		if (!string.IsNullOrEmpty(RefreshToken))
			return this;
		RefreshToken = refreshToken;
		return this;
	}

	/// <summary>
	/// Устанавливает идентификатор аккаунта для спецификации.
	/// </summary>
	/// <param name="id">Идентификатор аккаунта.</param>
	/// <returns>Обновленная спецификация аккаунта.</returns>
	public AccountSpecification WithId(Guid id)
	{
		if (Id.HasValue)
			return this;
		Id = id;
		return this;
	}

	/// <summary>
	/// Устанавливает электронную почту аккаунта для спецификации.
	/// </summary>
	/// <param name="email">Электронная почта аккаунта.</param>
	/// <returns>Обновленная спецификация аккаунта.</returns>
	public AccountSpecification WithEmail(string email)
	{
		if (!string.IsNullOrEmpty(Email))
			return this;
		Email = email;
		return this;
	}

	/// <summary>
	/// Устанавливает логин аккаунта для спецификации.
	/// </summary>
	/// <param name="login">Логин аккаунта.</param>
	/// <returns>Обновленная спецификация аккаунта.</returns>
	public AccountSpecification WithLogin(string login)
	{
		if (!string.IsNullOrEmpty(Login))
			return this;
		Login = login;
		return this;
	}

	/// <summary>
	/// Устанавливает флаг необходимости блокировки аккаунта при выборке.
	/// </summary>
	/// <returns>Обновленная спецификация аккаунта.</returns>
	public AccountSpecification WithLock()
	{
		LockRequired = true;
		return this;
	}
}
