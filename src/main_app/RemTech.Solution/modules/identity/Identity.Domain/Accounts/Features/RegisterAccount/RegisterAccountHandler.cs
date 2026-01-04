using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed class RegisterAccountHandler(
    IAccountsRepository accounts,
    IAccountTicketsRepository tickets,
    IEnumerable<IAccountPasswordRequirement> passwordRequirements,
    IPasswordCryptography cryptography,
    IAccountModuleOutbox outbox) 
    : ICommandHandler<RegisterAccountCommand, Unit>
{
    public async Task<Result<Unit>> Execute(RegisterAccountCommand command, CancellationToken ct = default)
    {
        Result<Unit> approval = await ApproveRegistration(command, ct);
        if (approval.IsFailure) return approval.Error;
        Result<AccountPassword> password = ApprovePassword(command);
        if (password.IsFailure) return password.Error;
        
        AccountPassword encrypted = await password.Value.Encrypt(cryptography, ct);
        Account account = CreateAccount(encrypted, command);
        AccountTicket ticket = account.CreateTicket(OutboxMessageTypes.EmailConfirmation);
        OutboxMessage<RegisteredOutboxMessagePayload> outboxMessagePayload = CreateOutboxMessage(account, ticket);
        
        await accounts.Add(account, ct);
        await tickets.Add(ticket, ct);
        await outbox.Add(outboxMessagePayload, ct);
        
        return Unit.Value;
    }

    private Result<AccountPassword> ApprovePassword(RegisterAccountCommand command)
    {
        AccountPassword password = AccountPassword.Create(command.Password);
        Result<Unit> satisfies = password.Satisfies(new PasswordRequirement().Use(passwordRequirements));
        if (satisfies.IsFailure) return satisfies.Error;
        return password;
    }
    
    private async Task<Result<Unit>> ApproveRegistration(RegisterAccountCommand command, CancellationToken ct)
    {
        Result<Unit> emailCheck = await CheckAccountEmailDuplicate(command.Email, ct);
        if (emailCheck.IsFailure) return emailCheck.Error;
        Result<Unit> loginCheck = await CheckAccountLoginDuplicate(command.Login, ct);
        if (loginCheck.IsFailure) return loginCheck.Error;
        return Unit.Value;
    }
    
    private Account CreateAccount(AccountPassword password, RegisterAccountCommand command)
    {
        AccountEmail email = AccountEmail.Create(command.Email);
        AccountLogin login = AccountLogin.Create(command.Login);
        Account account = Account.New(email, login, password);
        return account;
    }

    private OutboxMessage<RegisteredOutboxMessagePayload> CreateOutboxMessage(Account account, AccountTicket ticket)
    {
        RegisteredOutboxMessagePayload payload = new(account.Id.Value, ticket.TicketId, account.Email.Value, account.Login.Value);
        return OutboxMessage<RegisteredOutboxMessagePayload>.CreateNew(OutboxMessageTypes.EmailConfirmation, 0, payload);
    }
    
    private async Task<Result<Unit>> CheckAccountEmailDuplicate(string email, CancellationToken ct)
    {
        AccountSpecification specification = new AccountSpecification().WithEmail(email);
        bool exists = await accounts.Exists(specification, ct);
        if (exists) return Error.Conflict("Учетная запись с таким email уже существует.");
        return Result.Success(Unit.Value);
    }
    
    private async Task<Result<Unit>> CheckAccountLoginDuplicate(string login, CancellationToken ct)
    {
        AccountSpecification specification = new AccountSpecification().WithLogin(login);
        bool exists = await accounts.Exists(specification, ct);
        if (exists) return Error.Conflict("Учетная запись с таким логином уже существует.");
        return Result.Success(Unit.Value);
    }
}