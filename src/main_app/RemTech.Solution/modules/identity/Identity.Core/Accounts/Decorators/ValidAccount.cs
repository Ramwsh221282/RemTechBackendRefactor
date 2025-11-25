using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Identity.Core.Accounts.Decorators;

public sealed class ValidAccount(Account origin) : Account(origin)
{
    private readonly Account _account = origin;

    public override void Register()
    {
        Account valid = Valid();
        valid.Register();
    }

    public override Account ChangePassword(string password)
    {
        ValidAccount updated = new(_account.ChangePassword(password));
        return updated.Valid();
    }

    public override Account ChangeEmail(string newEmail)
    {
        ValidAccount updated = new(_account.ChangeEmail(newEmail));
        return updated.Valid();
    }

    public Account Valid()
    {
        const string idName = "Идентификатор учетной записи.";
        const string passwordName = "Пароль не указан.";
        if (origin.Id == Guid.Empty) throw ErrorException.ValueNotSet(idName);
        if (string.IsNullOrWhiteSpace(origin.Password)) throw ErrorException.ValueNotSet(passwordName);
        return origin;
    }
}