using Users.Module.Features.UserPasswordRecoveryConfirmation.Exceptions;

namespace Users.Module.Features.UserPasswordRecoveryConfirmation.Core;

internal sealed class UserPasswordRecoveryTicket
{
    private readonly Guid _key;

    private UserPasswordRecoveryTicket(Guid key)
    {
        _key = key;
    }

    public void Print(out Guid key)
    {
        key = _key;
    }

    public static UserPasswordRecoveryTicket Create(Guid key)
    {
        return key == Guid.Empty
            ? throw new UserRecoveryPasswordTicketEmptyException()
            : new UserPasswordRecoveryTicket(key);
    }

    public static UserPasswordRecoveryTicket Create(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new UserRecoveryPasswordTicketEmptyException();
        return !Guid.TryParse(key, out Guid parsedKey)
            ? throw new UserRecoveryPasswordTicketNotValidException()
            : Create(parsedKey);
    }
}
