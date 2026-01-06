using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed class RegisterAccountHandler(
    IAccountsRepository accounts,
    IEnumerable<IAccountPasswordRequirement> passwordRequirements,
    IPasswordHasher hasher,
    DomainEventsDispatcher eventsDispatcher) 
    : ICommandHandler<RegisterAccountCommand, Unit>
{
    public async Task<Result<Unit>> Execute(RegisterAccountCommand command, CancellationToken ct = default)
    {
        Result<Unit> approval = await ApproveRegistration(command, ct);
        if (approval.IsFailure) return approval.Error;
        Result<AccountPassword> password = ApprovePassword(command);
        if (password.IsFailure) return password.Error;
        
        AccountPassword encrypted = password.Value.HashBy(hasher, ct);
        Account account = CreateAccount(encrypted, command);
        
        await accounts.Add(account, ct);
        await eventsDispatcher.Dispatch(account.Events, ct);
        
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