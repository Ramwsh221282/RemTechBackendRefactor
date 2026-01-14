using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace Identity.Domain.Accounts.Features.ChangePassword;

[TransactionalHandler]
public sealed class ChangePasswordHandler(
    IEnumerable<IAccountPasswordRequirement> passwordRequirements,
    IPasswordHasher hasher,
    IAccountsRepository accounts,
    IAccessTokensRepository accessTokens,
    IRefreshTokensRepository refreshTokens,
    IAccountsModuleUnitOfWork unitOfWork
) : ICommandHandler<ChangePasswordCommand, Unit>
{
    public async Task<Result<Unit>> Execute(
        ChangePasswordCommand command,
        CancellationToken ct = default
    )
    {
        Result<Account> account = await GetRequiredAccount(command, ct);
        Result<Unit> verification = VerifyCurrentPassword(account, command.CurrentPassword);
        if (verification.IsFailure)
            return verification.Error;

        Result<Unit> change = ChangePassword(command, account);
        Result<Unit> logout = await Logout(account, change, command, ct);
        return await SaveChanges(account, change, logout, ct);
    }

    private Result<Unit> VerifyCurrentPassword(Result<Account> account, string currentPassword)
    {
        if (account.IsFailure)
            return account.Error;
        return account.Value.VerifyPassword(currentPassword, hasher);
    }

    private async Task<Result<Unit>> SaveChanges(
        Result<Account> account,
        Result<Unit> change,
        Result<Unit> logout,
        CancellationToken ct
    )
    {
        if (account.IsFailure)
            return account.Error;
        if (change.IsFailure)
            return change.Error;
        if (logout.IsFailure)
            return logout.Error;
        await unitOfWork.Save(account.Value, ct);
        return Unit.Value;
    }

    private async Task<Result<Unit>> Logout(
        Result<Account> account,
        Result<Unit> change,
        ChangePasswordCommand command,
        CancellationToken ct
    )
    {
        if (account.IsFailure)
            return account.Error;
        if (change.IsFailure)
            return change.Error;
        Result<AccessToken> accessToken = await accessTokens.Get(
            command.AccessToken,
            withLock: true,
            ct
        );
        Result<RefreshToken> refreshToken = await refreshTokens.Get(
            command.RefreshToken,
            withLock: true,
            ct
        );
        if (accessToken.IsSuccess)
            await accessTokens.Remove(accessToken.Value, ct);
        if (refreshToken.IsSuccess)
            await refreshTokens.Delete(refreshToken.Value, ct);
        return Unit.Value;
    }

    private Result<Unit> ChangePassword(ChangePasswordCommand command, Result<Account> account)
    {
        if (account.IsFailure)
            return account.Error;
        AccountPassword password = AccountPassword.Create(command.NewPassword);
        return account.Value.ChangePassword(password, hasher, passwordRequirements);
    }

    private async Task<Result<Account>> GetRequiredAccount(
        ChangePasswordCommand command,
        CancellationToken ct
    )
    {
        AccountSpecification specification = new AccountSpecification()
            .WithId(command.Id)
            .WithLock();
        return await accounts.Get(specification, ct);
    }
}
