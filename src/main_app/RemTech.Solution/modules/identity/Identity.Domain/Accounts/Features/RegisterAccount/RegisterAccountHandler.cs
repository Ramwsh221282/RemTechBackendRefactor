using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

/// <summary>
/// Обработчик команды для регистрации нового аккаунта пользователя.
/// </summary>
/// <param name="accounts">Репозиторий аккаунтов.</param>
/// <param name="passwordRequirements">Требования к паролю аккаунта.</param>
/// <param name="hasher">Сервис хеширования паролей.</param>
/// <param name="tickets">Репозиторий тикетов аккаунтов.</param>
/// <param name="outbox">Сервис исходящих сообщений модуля аккаунтов.</param>
public sealed class RegisterAccountHandler(
	IAccountsRepository accounts,
	IEnumerable<IAccountPasswordRequirement> passwordRequirements,
	IPasswordHasher hasher,
	IAccountTicketsRepository tickets,
	IAccountModuleOutbox outbox
) : ICommandHandler<RegisterAccountCommand, Unit>
{
	/// <summary>
	/// Выполняет регистрацию нового аккаунта пользователя по команде.
	/// </summary>
	/// <param name="command">Команда регистрации аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<Unit>> Execute(RegisterAccountCommand command, CancellationToken ct = default)
	{
		Result<Unit> approval = await ApproveRegistration(command, ct);
		if (approval.IsFailure)
		{
			return approval.Error;
		}

		Result<AccountPassword> password = ApprovePassword(command);
		if (password.IsFailure)
		{
			return password.Error;
		}

		AccountPassword encrypted = password.Value.HashBy(hasher);
		Account account = CreateAccount(encrypted, command);
		AccountTicket ticket = AccountTicket.New(account.Id.Value, AccountTicketPurposes.EMAIL_CONFIRMATION_REQUIRED);
		IdentityOutboxMessage message = CreateOutboxMessage(ticket, account);

		await accounts.Add(account, ct);
		await tickets.Add(ticket, ct);
		await outbox.Add(message, ct);
		return Unit.Value;
	}

	private static Account CreateAccount(AccountPassword password, RegisterAccountCommand command)
	{
		AccountEmail email = AccountEmail.Create(command.Email);
		AccountLogin login = AccountLogin.Create(command.Login);
		return Account.New(email, login, password);
	}

	private static IdentityOutboxMessage CreateOutboxMessage(AccountTicket ticket, Account account)
	{
		NewAccountRegisteredOutboxMessagePayload payload = new(
			account.Id.Value,
			ticket.TicketId,
			account.Email.Value,
			account.Login.Value
		);

		return IdentityOutboxMessage.CreateNew(AccountOutboxMessageTypes.NEW_ACCOUNT_CREATED, payload);
	}

	private Result<AccountPassword> ApprovePassword(RegisterAccountCommand command)
	{
		AccountPassword password = AccountPassword.Create(command.Password);
		Result<Unit> satisfies = password.Satisfies(new PasswordRequirement().Use(passwordRequirements));
		return satisfies.IsFailure ? (Result<AccountPassword>)satisfies.Error : (Result<AccountPassword>)password;
	}

	private async Task<Result<Unit>> ApproveRegistration(RegisterAccountCommand command, CancellationToken ct)
	{
		Result<Unit> emailCheck = await CheckAccountEmailDuplicate(command.Email, ct);
		if (emailCheck.IsFailure)
		{
			return emailCheck.Error;
		}

		Result<Unit> loginCheck = await CheckAccountLoginDuplicate(command.Login, ct);
		return loginCheck.IsFailure ? (Result<Unit>)loginCheck.Error : (Result<Unit>)Unit.Value;
	}

	private async Task<Result<Unit>> CheckAccountEmailDuplicate(string email, CancellationToken ct)
	{
		AccountSpecification specification = new AccountSpecification().WithEmail(email);
		bool exists = await accounts.Exists(specification, ct);
		return exists ? Error.Conflict("Учетная запись с таким email уже существует.") : Result.Success(Unit.Value);
	}

	private async Task<Result<Unit>> CheckAccountLoginDuplicate(string login, CancellationToken ct)
	{
		AccountSpecification specification = new AccountSpecification().WithLogin(login);
		bool exists = await accounts.Exists(specification, ct);
		return exists ? Error.Conflict("Учетная запись с таким логином уже существует.") : Result.Success(Unit.Value);
	}
}
