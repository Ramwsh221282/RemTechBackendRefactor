using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.Accounts;

public interface IAccount
{
    Task<Result<IAccount>> Register(
        IAccountCryptography cryptography,
        IAccountsStorage storage,
        CancellationToken ct = default
    );

    Task<Result<IAccount>> ChangeEmail(
        string newEmail,
        IAccountsStorage storage, 
        CancellationToken ct = default
        );
    
    Task<Result<IAccount>> ChangePassword(
        string newPassword,
        IAccountsStorage storage,
        IAccountCryptography cryptography,
        CancellationToken ct = default
        );
    
    Task<Result<Unit>> RequireAccountActivation(
        IOnAccountActivationRequiredListener listener, 
        CancellationToken ct = default
        );

    Task<Result<Unit>> RequirePasswordReset(
        IOnAccountPasswordResetRequiredListener listener,
        CancellationToken ct = default
        );
    
    Task<Result<IAccount>> Activate(
        IAccountsStorage storage, 
        CancellationToken ct
        );

    AccountData Represent();

    bool IsActivated();
}