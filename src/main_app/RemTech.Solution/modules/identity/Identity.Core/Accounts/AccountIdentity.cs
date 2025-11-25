using RemTech.SharedKernel.Core.PrimitivesModule;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Identity.Core.Accounts;

public class AccountIdentity(string email, string name)
{
    protected internal string Email = email;
    protected internal string Name = name;

    public AccountIdentity ChangeEmail(string newEmail)
    {
        AccountIdentity @new = new(newEmail, Name);
        @new.ValidateState();
        return @new;
    }
    
    private void ValidateState()
    {
        const string emailProperty = "Почта учетной записи";
        const string nameProperty = "Название учетной записи";
        const int maxNameLength = 256;
        if (string.IsNullOrWhiteSpace(Name)) throw ErrorException.ValueNotSet(nameProperty);
        if (Name.Length > maxNameLength) throw ErrorException.ValueExcess(nameProperty, maxNameLength);
        if (string.IsNullOrWhiteSpace(Email)) throw ErrorException.ValueNotSet(emailProperty);
        if (!EmailString.Create(Email).IsValid()) throw ErrorException.ValueInvalidFormat(emailProperty);
    }
}