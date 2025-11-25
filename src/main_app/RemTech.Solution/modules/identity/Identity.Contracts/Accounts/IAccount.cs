using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.Accounts;

public abstract record AccountMessage;

public interface IAccountMessagePublisher
{
    Task Publish<TMessage>(TMessage message, CancellationToken ct = default) 
        where TMessage : AccountMessage;
}

public interface IAccount
{
    Task<Result<Unit>> Register(
        IAccountEncrypter encrypter,
        IAccountPersister persister,
        CancellationToken ct = default
    );

    Task<Result<IAccount>> ChangeEmail(
        string newEmail,
        IAccountPersister persister, 
        CancellationToken ct = default
        );
    
    Task<Result<IAccount>> ChangePassword(
        string newPassword,
        IAccountPersister persister,
        IAccountEncrypter encrypter,
        CancellationToken ct = default
        );
    
    Task<Result<Unit>> RequireAccountActivation(
        IAccountMessagePublisher publisher, 
        CancellationToken ct = default
        );

    Task<Result<Unit>> RequirePasswordReset(
        IAccountMessagePublisher publisher,
        CancellationToken ct = default
        );
    
    Task<Result<IAccount>> Activate(
        IAccountPersister persister, 
        CancellationToken ct
        );

    IAccountRepresentation Represent(
        IAccountRepresentation representation
        );

    bool IsActivated();
}