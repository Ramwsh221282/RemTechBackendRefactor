using Identity.Application.Accounts;
using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.Accounts.Decorators;

public sealed class LoggingAccount(Serilog.ILogger logger, IAccount origin) : AccountEnvelope(origin)
{
    private readonly IAccount _origin = origin;

    public override async Task<Result<Unit>> Register(IAccountEncrypter encrypter, IAccountPersister persister, CancellationToken ct = default)
    {
        logger.Information("Registering account.");
        Result<Unit> result = await _origin.Register(encrypter, persister, ct);
        if (result.IsFailure)
        {
            logger.Error("Account was not registered. ERROR: {message}", result.Error.Message);
            return result;
        }
        logger.Information("Account has been registered");
        LogAccountInfo();
        return result;
    }

    public override async Task<Result<IAccount>> Activate(IAccountPersister persister, CancellationToken ct)
    {
        logger.Information("Activating account.");
        Result<IAccount> result = await _origin.Activate(persister, ct);
        if (result.IsFailure)
        {
            logger.Error("Account was not activated. ERROR: {message}", result.Error.Message);
            return result;
        }
        
        logger.Information("Account has been activated");
        LogAccountInfo();
        return result;
    }

    public override async Task<Result<IAccount>> ChangeEmail(string newEmail, IAccountPersister persister, CancellationToken ct = default)
    {
        logger.Information("Changing account email");
        Result<IAccount> result = await _origin.ChangeEmail(newEmail, persister, ct);
        if (result.IsFailure)
        {
            logger.Error("Account email was not changed. ERROR: {message}", result.Error.Message);
            return result;
        }
        
        logger.Information("Account has been changed");
        LogAccountInfo();
        return result;
    }

    public override async Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountPersister persister, 
        IAccountEncrypter encrypter,
        CancellationToken ct = default)
    {
        logger.Information("Changing account password");
        Result<IAccount> result = await _origin.ChangePassword(newPassword, persister, encrypter, ct);
        if (result.IsFailure)
        {
            logger.Error("Account password was not changed. ERROR: {message}", result.Error.Message);
            return result;
        }
        
        logger.Information("Account has been changed");
        LogAccountInfo();
        return result;
    }

    public override async Task<Result<Unit>> RequireAccountActivation(IAccountMessagePublisher publisher, CancellationToken ct = default)
    {
        logger.Information("Requireing account activation");
        Result<Unit> result = await _origin.RequireAccountActivation(publisher, ct);
        if (result.IsFailure)
        {
            logger.Error("Account activation requirement failed. ERROR: {message}", result.Error.Message);
            return result;
        }
        
        logger.Information("Account has been activated");
        LogAccountInfo();
        return result;
    }

    public override async Task<Result<Unit>> RequirePasswordReset(IAccountMessagePublisher publisher, CancellationToken ct = default)
    {
        logger.Information("Requireing password reset");
        Result<Unit> result = await _origin.RequirePasswordReset(publisher, ct);
        if (result.IsFailure)
        {
            logger.Error("Account password reset requirement failed. ERROR: {message}", result.Error.Message);
            return result;
        }
        
        logger.Information("Account has been reset");
        LogAccountInfo();
        return result;
    }

    private void LogAccountInfo()
    {
        IAccountRepresentation representation = _origin.Represent(AccountRepresentation.Empty());
        IAccountData data = representation.Data;
        object[] parameters = [data.Id, data.Name, data.Email, data.Activated];
        logger.Information("""
                           ACCOUNT INFO:
                           ID: {Id}
                           Name: {Name},
                           Email: {Email}
                           Activated: {Activated}
                           """, parameters);
    }
}