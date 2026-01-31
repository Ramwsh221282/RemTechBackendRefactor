using Identity.Domain.Accounts.Models.Events;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.DomainEvents;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Аккаунт пользователя.
/// </summary>
/// <param name="id">Идентификатор аккаунта.</param>
/// <param name="email">Электронная почта аккаунта.</param>
/// <param name="password">Пароль аккаунта.</param>
/// <param name="login">Логин аккаунта.</param>
/// <param name="activationStatus">Статус активации аккаунта.</param>
/// <param name="permissions">Коллекция разрешений аккаунта.</param>
public sealed class Account(
	AccountId id,
	AccountEmail email,
	AccountPassword password,
	AccountLogin login,
	AccountActivationStatus activationStatus,
	AccountPermissionsCollection permissions
) : IDomainEventBearer
{
	private List<IDomainEvent> _events = [];

	private Account(Account account)
		: this(
			account.Id,
			account.Email,
			account.Password,
			account.Login,
			account.ActivationStatus,
			account.Permissions.Clone()
		) { }

	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public AccountId Id { get; } = id;

	/// <summary>
	/// Электронная почта аккаунта.
	/// </summary>
	public AccountEmail Email { get; private set; } = email;

	/// <summary>
	/// Пароль аккаунта.
	/// </summary>
	public AccountPassword Password { get; private set; } = password;

	/// <summary>
	/// Логин аккаунта.
	/// </summary>
	public AccountLogin Login { get; } = login;

	/// <summary>
	/// Статус активации аккаунта.
	/// </summary>
	public AccountActivationStatus ActivationStatus { get; private set; } = activationStatus;

	/// <summary>
	/// Коллекция разрешений аккаунта.
	/// </summary>
	public AccountPermissionsCollection Permissions { get; } = permissions;

	/// <summary>
	/// Количество разрешений аккаунта.
	/// </summary>
	public int PermissionsCount => Permissions.Count;

	/// <summary>
	/// Список разрешений аккаунта.
	/// </summary>
	public IReadOnlyList<Permission> PermissionsList => Permissions.Permissions;

	/// <summary>
	/// События домена, связанные с аккаунтом.
	/// </summary>
	public IReadOnlyList<IDomainEvent> Events => _events;

	/// <summary>
	/// Создает аккаунт с заданным статусом активации.
	/// </summary>
	/// <param name="email">Электронная почта аккаунта.</param>
	/// <param name="login">Логин аккаунта.</param>
	/// <param name="password">Пароль аккаунта.</param>
	/// <param name="status">Статус активации аккаунта.</param>
	/// <returns>Созданный аккаунт.</returns>
	public static Account Create(
		AccountEmail email,
		AccountLogin login,
		AccountPassword password,
		AccountActivationStatus status
	)
	{
		Account account = New(email, login, password);
		account.ActivationStatus = status;
		return account;
	}

	/// <summary>
	/// Создает новый аккаунт.
	/// </summary>
	/// <param name="email">Электронная почта аккаунта.</param>
	/// <param name="login">Логин аккаунта.</param>
	/// <param name="password">Пароль аккаунта.</param>
	/// <returns>Созданный аккаунт.</returns>
	public static Account New(AccountEmail email, AccountLogin login, AccountPassword password)
	{
		AccountId id = AccountId.New();
		AccountActivationStatus activationStatus = AccountActivationStatus.NotActivated();
		AccountPermissionsCollection permissions = AccountPermissionsCollection.Empty(id);
		Account account = new(id, email, password, login, activationStatus, permissions);
		account._events.Add(new NewAccountCreatedEvent(account));
		return account;
	}

	/// <summary>
	/// Активирует аккаунт.
	/// </summary>
	/// <returns>Результат операции активации аккаунта.</returns>
	public Result<Unit> Activate()
	{
		if (ActivationStatus.IsActivated())
		{
			return Error.Conflict("Учетная запись уже активирована.");
		}

		ActivationStatus = AccountActivationStatus.Activated();
		_events.Add(new AccountActivatedEvent(this));
		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Создает тикет для аккаунта.
	/// </summary>
	/// <param name="purpose">Цель создания тикета.</param>
	/// <returns>Результат создания тикета для аккаунта.</returns>
	public Result<AccountTicket> CreateTicket(string purpose)
	{
		return AccountTicket.New(this, purpose);
	}

	/// <summary>
	/// Добавить разрешения к аккаунту.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для добавления.</param>
	/// <returns>Результат операции добавления разрешений.</returns>
	public Result<Unit> AddPermissions(IEnumerable<Permission> permissions)
	{
		List<string> errors = [];
		foreach (Permission permission in permissions)
		{
			Result<Unit> add = Permissions.Add(permission);
			if (add.IsFailure)
			{
				errors.Add(add.Error.Message);
			}
		}

		return errors.Count != 0 ? (Result<Unit>)Error.Conflict(string.Join(", ", errors)) : Result.Success(Unit.Value);
	}

	/// <summary>
	/// Добавить разрешение к аккаунту.
	/// </summary>
	/// <param name="permission">Разрешение для добавления.</param>
	/// <returns>Результат операции добавления разрешения.</returns>
	public Result<Unit> AddPermission(Permission permission)
	{
		Result<Unit> add = Permissions.Add(permission);
		return add.IsFailure ? add.Error : Unit.Value;
	}

	/// <summary>
	/// Закрывает тикет аккаунта.
	/// </summary>
	/// <param name="ticket">Тикет для закрытия.</param>
	/// <returns>Результат операции закрытия тикета.</returns>
	public Result<Unit> CloseTicket(AccountTicket ticket)
	{
		Result<Unit> finishing = ticket.FinishBy(Id.Value);
		if (finishing.IsFailure)
		{
			return finishing.Error;
		}

		_events.Add(new AccountClosedTicketEvent(this, ticket));
		return Unit.Value;
	}

	/// <summary>
	/// Изменяет пароль аккаунта.
	/// </summary>
	/// <param name="password">Новый пароль аккаунта.</param>
	/// <param name="hasher">Хешер для пароля.</param>
	/// <param name="requirements">Требования к паролю.</param>
	/// <returns>Результат операции изменения пароля.</returns>
	public Result<Unit> ChangePassword(
		AccountPassword password,
		IPasswordHasher hasher,
		IEnumerable<IAccountPasswordRequirement> requirements
	)
	{
		Result<Unit> validation = new PasswordRequirement().Use(requirements).Satisfies(password);
		if (validation.IsFailure)
		{
			return validation.Error;
		}

		Password = password.HashBy(hasher);
		return Unit.Value;
	}

	/// <summary>
	/// Проверяет пароль аккаунта.
	/// </summary>
	/// <param name="input">Входной пароль для проверки.</param>
	/// <param name="hasher">Хешер для проверки пароля.</param>
	/// <returns>Результат проверки пароля.</returns>
	public Result<Unit> VerifyPassword(string input, IPasswordHasher hasher)
	{
		bool verified = Password.Verify(input, hasher);
		return verified ? Result.Success(Unit.Value) : Error.Validation("Неверный пароль.");
	}

	/// <summary>
	/// Изменяет электронную почту аккаунта.
	/// </summary>
	/// <param name="email">Новая электронная почта аккаунта.</param>
	public void ChangeEmail(AccountEmail email)
	{
		Email = email;
	}

	/// <summary>
	/// Проверяет, можно ли сбросить пароль для аккаунта.
	/// </summary>
	/// <returns>Результат проверки возможности сброса пароля.</returns>
	public Result<Unit> CanResetPassword()
	{
		return ActivationStatus.IsActivated()
			? Result.Success(Unit.Value)
			: Error.Validation("Сброс пароля невозможен для неактивированной учетной записи.");
	}

	/// <summary>
	/// Создает копию аккаунта.
	/// </summary>
	/// <returns>Копия аккаунта.</returns>
	public Account Copy()
	{
		return new(this) { _events = [.. _events] };
	}
}
